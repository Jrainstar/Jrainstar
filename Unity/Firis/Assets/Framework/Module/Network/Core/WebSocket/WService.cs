using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Firis
{
    public class WService : AService
    {
        private long idGenerater = 200000000;

        private HttpListener httpListener;

        private readonly Dictionary<long, WChannel> channels = new Dictionary<long, WChannel>();

        public MainThreadSynchronizationContext ThreadSynchronizationContext;

        public WService(MainThreadSynchronizationContext threadSynchronizationContext, IEnumerable<string> prefixs)
        {
            this.ThreadSynchronizationContext = threadSynchronizationContext;

            this.httpListener = new HttpListener();

            StartAccept(prefixs);
        }

        public WService(MainThreadSynchronizationContext threadSynchronizationContext)
        {
            this.ThreadSynchronizationContext = threadSynchronizationContext;
        }

        private long GetId
        {
            get
            {
                return ++this.idGenerater;
            }
        }

        public WChannel Create(string address, long id)
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            WChannel channel = new WChannel(id, webSocket, address, this);
            this.channels[channel.Id] = channel;
            return channel;
        }

        public void Remove(long id, int error = 0)
        {
            WChannel channel;
            if (!this.channels.TryGetValue(id, out channel))
            {
                return;
            }

            channel.Error = error;

            this.channels.Remove(id);
            channel.Dispose();
        }

        public override bool IsDispose()
        {
            return this.ThreadSynchronizationContext == null;
        }

        public override void Dispose()
        {
            this.ThreadSynchronizationContext = null;
            this.httpListener?.Close();
            this.httpListener = null;
        }

        private async void StartAccept(IEnumerable<string> prefixs)
        {
            try
            {
                foreach (string prefix in prefixs)
                {
                    this.httpListener.Prefixes.Add(prefix);
                }

                httpListener.Start();

                while (true)
                {
                    try
                    {
                        HttpListenerContext httpListenerContext = await this.httpListener.GetContextAsync();

                        HttpListenerWebSocketContext webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);

                        WChannel channel = new WChannel(this.GetId, webSocketContext, this);

                        this.channels[channel.Id] = channel;

                        OnAccept(channel.Id, channel.RemoteAddress);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == 5)
                {
                    throw new Exception($"CMD管理员中输入: netsh http add urlacl url=http://*:8080/ user=Everyone", e);
                }

                Log.Error(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public override void Remove(long id)
        {
            WChannel channel;
            if (!this.channels.TryGetValue(id, out channel))
            {
                return;
            }

            this.channels.Remove(id);
            channel.Dispose();
        }

        public void Get(long id, string address)
        {
            if (!this.channels.TryGetValue(id, out _))
            {
                this.Create(address, id);
            }
        }

        protected override void Get(long id, IPEndPoint address)
        {
            if (!this.channels.TryGetValue(id, out _))
            {
                this.Create($"http://{address}/", id);
            }
        }

        protected override void Send(long channelId, MemoryStream stream)
        {
            try
            {
                this.channels.TryGetValue(channelId, out WChannel channel);
                if (channel == null)
                {
                    return;
                }
                channel.Send(stream);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public override void Update()
        {
            this.ThreadSynchronizationContext.Update();
        }
    }
}