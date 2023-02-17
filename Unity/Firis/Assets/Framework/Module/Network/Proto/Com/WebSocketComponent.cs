using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class WebSocketComponent : Component, IAwake<IMessageDispatcher>, IAwake<List<string>, IMessageDispatcher>, IUpdate
    {
        private WService Service { get; set; }
        public Dictionary<long, Session> Sessions { get; set; } = new Dictionary<long, Session>();
        private IMessageDispatcher MessageDispatcher { get; set; }

        public void Awake(IMessageDispatcher messageDispatcher)
        {
            MessageDispatcher = messageDispatcher;
            Service = new WService(MainThreadSynchronizationContext.Instance);
            Service.ErrorCallback += (channelId, error) => OnError(channelId, error);
            Service.ReadCallback += (channelId, Memory) => OnRead(channelId, Memory);
        }

        public void Awake(List<string> address, IMessageDispatcher messageDispatcher)
        {
            MessageDispatcher = messageDispatcher;
            Service = new WService(MainThreadSynchronizationContext.Instance, address);
            Service.ErrorCallback += (channelId, error) => OnError(channelId, error);
            Service.ReadCallback += (channelId, Memory) => OnRead(channelId, Memory);
            Service.AcceptCallback += (channelId, IPAddress) => OnAccept(channelId, IPAddress);
        }

        public void Update()
        {
            Service.Update();
        }

        public void OnlyLoad(string assemblyName)
        {
            MessageDispatcher.OnlyLoad(assemblyName);
        }

        public void OnlyLoadFile(string assemblyName)
        {
            MessageDispatcher.OnlyLoadFile(assemblyName);
        }

        public void GeneLoad()
        {
            MessageDispatcher.GeneLoad();
        }

        public void Load(string assemblyName)
        {
            MessageDispatcher.Load(assemblyName);
        }

        public void LoadFile(string filePath)
        {
            MessageDispatcher.LoadFile(filePath);
        }

        private void OnAccept(long channelId, IPEndPoint endPoint)
        {
            Session session = EntityFactory.CreatWithID<Session, AService, IMessageDispatcher>(channelId, Service, MessageDispatcher);
            Sessions.Add(channelId, session);
            session.RemoteAddress = endPoint;
        }

        private void OnRead(long channelId, MemoryStream memory)
        {
            Session session;
            Sessions.TryGetValue(channelId, out session);
            if (session == null)
            {
                return;
            }

            //session.LastRecvTime = TimeHelper.ClientNow();
            MessageDispatcher.Dispatch(memory, session);
        }

        private void OnError(long channelId, int error)
        {
            Session session;
            Sessions.TryGetValue(channelId, out session);
            if (session == null)
            {
                return;
            }

            session.Dispose();
        }

        public Session Create(IPEndPoint endPoint)
        {
            Session session = EntityFactory.Creat<Session, AService, IMessageDispatcher>(Service, MessageDispatcher);
            session.RemoteAddress = endPoint;
            Service.GetOrCreate(session.ID, endPoint);
            Sessions.Add(session.ID, session);
            return session;
        }

        public Session Create(string address)
        {
            Session session = EntityFactory.Creat<Session, AService, IMessageDispatcher>(Service, MessageDispatcher);
            Service.Get(session.ID, address);
            Sessions.Add(session.ID, session);
            return session;
        }

        public Session Get(long id)
        {
            Session session;
            Sessions.TryGetValue(id, out session);
            return session;
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            base.Dispose();

            Service.Dispose();

            foreach (var session in Sessions)
            {
                session.Value.Dispose();
            }

            Sessions.Clear();
        }
    }
}
