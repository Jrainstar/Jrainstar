using System.Threading.Tasks;
using Jrainstar;

namespace Jrainstar
{
    public readonly struct RpcInfo
    {
        public readonly IRequest Request;
        public readonly TaskCompletionSource<IResponse> Tcs;

        public RpcInfo(IRequest request)
        {
            Request = request;
            Tcs = new TaskCompletionSource<IResponse>();
        }
    }

    public sealed class Session : Entity, IAwake<AService>
    {
        public AService AService { get; set; }

        public int RpcId
        {
            get;
            set;
        }

        public readonly Dictionary<int, RpcInfo> requestCallbacks = new();

        public long LastRecvTime
        {
            get;
            set;
        }

        public long LastSendTime
        {
            get;
            set;
        }

        public int Error
        {
            get;
            set;
        }

        public string RemoteAddress
        {
            get;
            set;
        }

        public void Awake(AService aService)
        {
            AService = aService;
            long timeNow = TimeComponent.Instance.ClientNow();
            LastRecvTime = timeNow;
            LastSendTime = timeNow;

            requestCallbacks.Clear();

            Log.Info($"session create:  id: {ID} timeNow: {timeNow} ");
        }

        private void Destroy()
        {
            AService.Remove(ID, Error);

            foreach (RpcInfo responseCallback in requestCallbacks.Values.ToArray())
            {
                responseCallback.Tcs.SetException(new Exception($"error: {Error} session dispose: {ID} {RemoteAddress}"));
            }

            Log.Info($"session dispose: {RemoteAddress} id: {ID} ErrorCode: {Error}, please see ErrorCode.cs! {TimeComponent.Instance.ClientNow()}");

            requestCallbacks.Clear();
        }


        public void Send(IMessage message)
        {
            Send(default, message);
        }

        public void Send(ActorId actorId, IMessage message)
        {
            LastSendTime = TimeComponent.Instance.ClientNow();

            (ushort opcode, MemoryBuffer memoryBuffer) = MessageSerializeHelper.ToMemoryBuffer(AService, actorId, message);
            AService.Send(ID, memoryBuffer);
        }

        public async Task<IResponse> Call(IRequest request, int time = 0)
        {
            int rpcId = ++RpcId;
            RpcInfo rpcInfo = new(request);
            requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            Send(request);

            if (time > 0)
            {
                async Task Timeout()
                {
                    await Task.Delay(time);
                    if (!requestCallbacks.TryGetValue(rpcId, out RpcInfo action))
                    {
                        return;
                    }

                    if (!requestCallbacks.Remove(rpcId))
                    {
                        return;
                    }

                    action.Tcs.SetException(new Exception($"session call timeout: {request} {time}"));
                }

                Timeout();
            }

            return await rpcInfo.Tcs.Task;
        }

        public void OnResponse(IResponse response)
        {
            if (!requestCallbacks.Remove(response.RpcId, out var action))
            {
                return;
            }
            action.Tcs.SetResult(response);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            base.Dispose();
            Destroy();
        }
    }
}
