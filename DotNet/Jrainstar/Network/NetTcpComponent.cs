using System.Net;
using System.Net.Sockets;

namespace Jrainstar
{
    public class NetTcpComponent : Component, IAwake<IPEndPoint, NetworkProtocol>, IAwake<AddressFamily, NetworkProtocol>, IUpdate
    {
        public AService AService { get; set; }

        private Dictionary<long, Session> Sessions { get; set; } = new Dictionary<long, Session>();

        public void Awake(AddressFamily addressFamily, NetworkProtocol protocol)
        {
            AService = new TService(addressFamily, ServiceType.Outer);
            AService.ReadCallback = OnRead;
            AService.ErrorCallback = OnError;
        }

        public void Awake(IPEndPoint address, NetworkProtocol protocol)
        {
            // AService = new TService(address, ServiceType.Outer);
            AService = new TService(address, ServiceType.Outer);
            AService.AcceptCallback = OnAccept;
            AService.ReadCallback = OnRead;
            AService.ErrorCallback = OnError;
        }

        private void OnAccept(long channelId, IPEndPoint point)
        {
            Session session = EntityFactory.CreatWithID<Session, AService>(channelId, AService); // self.AddChildWithId<Session, AService>(channelId, self.AService);
            Sessions.Add(session.ID, session);
            session.RemoteAddress = point.ToString();
        }

        private void OnRead(long channelId, MemoryBuffer memoryBuffer)
        {
            Session session = Sessions[channelId];
            session.LastRecvTime = TimeComponent.Instance.ClientNow();

            (ActorId _, object message) = MessageSerializeHelper.ToMessage(AService, memoryBuffer);
            Handle(session, message);
        }

        private void Handle(Session session, object message)
        {
            switch (message)
            {
                case IResponse response:
                    {
                        session.OnResponse(response);
                        break;
                    }
                case IMessage:
                    {
                        MessageSessionDispatcher.Instance.Handle(session, message);
                        break;
                    }
            }
        }


        private void OnError(long channelId, int error)
        {
            Session session = Sessions[channelId];
            if (session == null)
            {
                return;
            }

            session.Error = error;
            session.Dispose();
        }

        public void Update()
        {
            AService.Update();
        }

        public Session Create(IPEndPoint realIPEndPoint)
        {
            long channelId = SnowFlake.Instance.NextId();
            var session = EntityFactory.CreatWithID<Session, AService>(channelId, AService);
            Sessions.Add(channelId, session);
            session.RemoteAddress = realIPEndPoint.ToString();

            AService.Create(session.ID, realIPEndPoint.ToString());

            return session;
        }
    }
}