using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Firis
{
    public class WChannel : AChannel
    {
        public HttpListenerWebSocketContext WebSocketContext { get; }

        private readonly WService Service;

        private readonly WebSocket webSocket;

        private readonly Queue<MemoryStream> queue = new Queue<MemoryStream>();

        private bool isSending;

        private bool isConnected;

        private readonly MemoryStream recvStream;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public WChannel(long id, HttpListenerWebSocketContext webSocketContext, WService service)
        {
            this.Id = id;
            this.Service = service;
            this.ChannelType = ChannelType.Accept;
            this.WebSocketContext = webSocketContext;
            this.webSocket = webSocketContext.WebSocket;
            this.recvStream = new MemoryStream(ushort.MaxValue);

            isConnected = true;

            this.Service.ThreadSynchronizationContext.Post(() =>
            {
                this.StartRecv();
                this.StartSend();
            });
        }

        public WChannel(long id, WebSocket webSocket, string connectUrl, WService service)
        {
            this.Id = id;
            this.Service = service;
            this.ChannelType = ChannelType.Connect;
            this.webSocket = webSocket;
            this.recvStream = new MemoryStream(ushort.MaxValue);

            isConnected = false;

            Log.Info(connectUrl);

            this.Service.ThreadSynchronizationContext.Post(() => this.ConnectAsync(connectUrl));
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            Log.Info("WChannel---Dispose");
            this.Id = 0;

            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null;

            this.webSocket.Dispose();
        }

        public async void ConnectAsync(string url)
        {
            try
            {
                await ((ClientWebSocket)this.webSocket).ConnectAsync(new Uri(url), cancellationTokenSource.Token);
                isConnected = true;

                this.StartRecv();
                this.StartSend();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                this.OnError(ErrorCore.ERR_WebsocketConnectError);
            }
        }

        public void Send(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            this.queue.Enqueue(stream);

            if (this.isConnected)
            {
                this.StartSend();
            }
        }

        public async Task StartSend()
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                if (this.isSending)
                {
                    return;
                }

                this.isSending = true;

                while (true)
                {
                    if (this.queue.Count == 0)
                    {
                        this.isSending = false;
                        return;
                    }

                    MemoryStream bytes = this.queue.Dequeue();
                    try
                    {
                        await this.webSocket.SendAsync(new ReadOnlyMemory<byte>(bytes.GetBuffer(), (int)bytes.Position, (int)(bytes.Length - bytes.Position)), WebSocketMessageType.Binary, true, cancellationTokenSource.Token);
                        if (this.IsDisposed)
                        {
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                        this.OnError(ErrorCore.ERR_WebsocketSendError);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private byte[] cache = new byte[ushort.MaxValue];

        public async Task StartRecv()
        {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                while (true)
                {
                    ValueWebSocketReceiveResult receiveResult;
                    int receiveCount = 0;
                    do
                    {
                        receiveResult = await this.webSocket.ReceiveAsync(
                            new Memory<byte>(cache, receiveCount, this.cache.Length - receiveCount),
                            cancellationTokenSource.Token);

                        if (this.IsDisposed)
                        {
                            return;
                        }

                        receiveCount += receiveResult.Count;
                    }
                    while (!receiveResult.EndOfMessage);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        this.OnError(ErrorCore.ERR_WebsocketPeerReset);
                        return;
                    }

                    if (receiveResult.Count > ushort.MaxValue)
                    {
                        await this.webSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig, $"message too big: {receiveCount}",
                            cancellationTokenSource.Token);
                        this.OnError(ErrorCore.ERR_WebsocketMessageTooBig);
                        return;
                    }

                    this.recvStream.SetLength(receiveCount);
                    this.recvStream.Seek(2, SeekOrigin.Begin);
                    Array.Copy(this.cache, 0, this.recvStream.GetBuffer(), 0, receiveCount);
                    this.OnRead(this.recvStream);
                }
            }
            catch (System.OperationCanceledException ex)
            {

            }
            catch (WebSocketException e)
            {
                long channelId = this.Id;
                this.Service.Remove(channelId);
                this.Service.OnError(channelId, ErrorCore.ERR_WebsocketRecvError);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                this.OnError(ErrorCore.ERR_WebsocketRecvError);
            }
        }

        private void OnRead(MemoryStream memoryStream)
        {
            try
            {
                long channelId = this.Id;
                this.Service.OnRead(channelId, memoryStream);
            }
            catch (Exception e)
            {
                Log.Error($"{this.RemoteAddress} {memoryStream.Length} {e}");
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                this.OnError(ErrorCore.ERR_PacketParserError);
            }
        }

        private void OnError(int error)
        {
            Log.Error($"WChannel error: {error} {this.RemoteAddress}");

            long channelId = this.Id;

            this.Service.Remove(channelId);

            this.Service.OnError(channelId, error);
        }
    }
}