using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Jrainstar
{
    public class TcpTransport : IKcpTransport
    {
        private readonly TService tService;

        private readonly DoubleMap<long, EndPoint> idEndpoints = new();

        private readonly Queue<(EndPoint, MemoryBuffer)> channelRecvDatas = new();

        private readonly Dictionary<long, long> readWriteTime = new();

        private readonly Queue<long> channelIds = new();

        public TcpTransport(AddressFamily addressFamily)
        {
            tService = new TService(addressFamily, ServiceType.Outer);
            tService.ErrorCallback = OnError;
            tService.ReadCallback = OnRead;
        }

        public TcpTransport(IPEndPoint ipEndPoint)
        {
            tService = new TService(ipEndPoint, ServiceType.Outer);
            tService.AcceptCallback = OnAccept;
            tService.ErrorCallback = OnError;
            tService.ReadCallback = OnRead;
        }

        private void OnAccept(long id, IPEndPoint ipEndPoint)
        {
            TChannel channel = tService.Get(id);
            long timeNow = TimeComponent.Instance.ClientFrameTime();
            readWriteTime[id] = timeNow;
            channelIds.Enqueue(id);
            idEndpoints.Add(id, channel.RemoteAddress);
        }

        public void OnError(long id, int error)
        {
            Log.Warning($"IKcpTransport tcp error: {id} {error}");
            tService.Remove(id, error);
            idEndpoints.RemoveByKey(id);
            readWriteTime.Remove(id);
        }

        private void OnRead(long id, MemoryBuffer memoryBuffer)
        {
            long timeNow = TimeComponent.Instance.ClientFrameTime();
            readWriteTime[id] = timeNow;
            TChannel channel = tService.Get(id);
            channelRecvDatas.Enqueue((channel.RemoteAddress, memoryBuffer));
        }

        public void Send(byte[] bytes, int index, int length, EndPoint endPoint)
        {
            long id = idEndpoints.GetKeyByValue(endPoint);
            if (id == 0)
            {
                id = SnowFlake.Instance.NextId();
                tService.Create(id, endPoint.ToString());
                idEndpoints.Add(id, endPoint);
                channelIds.Enqueue(id);
            }
            MemoryBuffer memoryBuffer = tService.Fetch();
            memoryBuffer.Write(bytes, index, length);
            memoryBuffer.Seek(0, SeekOrigin.Begin);
            tService.Send(id, memoryBuffer);

            long timeNow = TimeComponent.Instance.ClientFrameTime();
            readWriteTime[id] = timeNow;
        }

        public int Recv(byte[] buffer, ref EndPoint endPoint)
        {
            return RecvNonAlloc(buffer, ref endPoint);
        }

        public int RecvNonAlloc(byte[] buffer, ref EndPoint endPoint)
        {
            (EndPoint e, MemoryBuffer memoryBuffer) = channelRecvDatas.Dequeue();
            endPoint = e;
            int count = memoryBuffer.Read(buffer);
            tService.Recycle(memoryBuffer);
            return count;
        }

        public int Available()
        {
            return channelRecvDatas.Count;
        }

        public void Update()
        {
            // 检查长时间不读写的TChannel, 超时断开, 一次update检查10个
            long timeNow = TimeComponent.Instance.ClientFrameTime();
            const int MaxCheckNum = 10;
            int n = channelIds.Count < MaxCheckNum ? channelIds.Count : MaxCheckNum;
            for (int i = 0; i < n; ++i)
            {
                long id = channelIds.Dequeue();
                if (!readWriteTime.TryGetValue(id, out long rwTime))
                {
                    continue;
                }
                if (timeNow - rwTime > 30 * 1000)
                {
                    OnError(id, ErrorCore.ERR_KcpReadWriteTimeout);
                    continue;
                }
                channelIds.Enqueue(id);
            }

            tService.Update();
        }

        public void Dispose()
        {
            tService?.Dispose();
        }
    }
}
