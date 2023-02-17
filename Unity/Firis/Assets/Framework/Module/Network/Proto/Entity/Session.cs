using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class Session : Entity, IAwake<AService, IMessageDispatcher>, IUpdate
    {
        public long AvatarId { get; set; }

        public int RpcID { get; set; }
        public AService AService { get; set; }
        public IPEndPoint RemoteAddress { get; set; }
        IMessageDispatcher MessageDispatcher { get; set; }

        private Dictionary<int, TaskCompletionSource<IResponse>> ResponseCache { get; set; } = new Dictionary<int, TaskCompletionSource<IResponse>>();

        private List<int> WaitToRemove { get; set; } = new List<int>();

        private Dictionary<int, long> ResponseWaitCache { get; set; } = new Dictionary<int, long>();
        private Stopwatch Stopwatch { get; set; } = new Stopwatch();
        private long CurrentTime => Stopwatch.ElapsedMilliseconds;
        private long CanWaitTime => 6000;


        public void Awake(AService service, IMessageDispatcher messageDispatcher)
        {
            AService = service;
            MessageDispatcher = messageDispatcher;
            Stopwatch.Start();
        }

        public void Update()
        {
            foreach (var wait in ResponseWaitCache)
            {
                if (wait.Value < CurrentTime)
                {
                    WaitToRemove.Add(wait.Key);
                }
            }
            foreach (var wait in WaitToRemove)
            {
                ResponseCache[wait].SetResult(null);
                ResponseCache.Remove(wait);
                ResponseWaitCache.Remove(wait);
            }
            WaitToRemove.Clear();
        }

        public void Send(IMessage message, short avatarId = 0, long sessionId = 0)
        {
            var ms = MessageDispatcher.Serialize(message, avatarId, sessionId);
            Send(ms);
        }

        public void Send(MemoryStream ms)
        {
            AService.SendStream(ID, ms);
        }

        public async Task<IResponse> Call(IRequest request, short avatarId = 0, long sessionId = 0)
        {
            var rpc = RpcID++;
            request.RpcID = rpc;
            Send(request, avatarId, sessionId);
            var tcs = new TaskCompletionSource<IResponse>();
            Task<IResponse> task = tcs.Task;
            ResponseCache.TryAdd(rpc, tcs);
            ResponseWaitCache.TryAdd(rpc, CurrentTime + CanWaitTime);
            await task;
            return task.Result;
        }

        public void SetResult(int rpc, IResponse result)
        {
            if (ResponseCache.ContainsKey(rpc))
            {
                ResponseCache[rpc].SetResult(result);
                ResponseCache.Remove(rpc);
                ResponseWaitCache.Remove(rpc);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            base.Dispose();
            AService.RemoveChannel(ID);
            WaitToRemove.Clear();
            ResponseCache.Clear();
            ResponseWaitCache.Clear();
        }
    }
}
