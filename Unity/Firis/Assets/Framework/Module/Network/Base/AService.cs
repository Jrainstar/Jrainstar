using System;
using System.IO;
using System.Net;

namespace Firis
{
    public abstract class AService : IDisposable
    {
        public MainThreadSynchronizationContext ThreadSynchronizationContext;

        public abstract void Update();

        public abstract void Remove(long id);

        public abstract bool IsDispose();

        protected abstract void Get(long id, IPEndPoint address);

        public virtual void Dispose() { }

        protected abstract void Send(long channelId, MemoryStream stream);

        protected void OnAccept(long channelId, IPEndPoint ipEndPoint)
        {
            AcceptCallback?.Invoke(channelId, ipEndPoint);
        }

        public void OnRead(long channelId, MemoryStream memoryStream)
        {
            ReadCallback?.Invoke(channelId, memoryStream);
        }

        public void OnError(long channelId, int e)
        {
            Remove(channelId);

            ErrorCallback?.Invoke(channelId, e);
        }


        public Action<long, IPEndPoint> AcceptCallback { get; set; }
        public Action<long, int> ErrorCallback { get; set; }
        public Action<long, MemoryStream> ReadCallback { get; set; }

        public void Destroy()
        {
            Dispose();
        }

        public void RemoveChannel(long channelId)
        {
            Remove(channelId);
        }

        public void SendStream(long channelId, MemoryStream stream)
        {
            Send(channelId, stream);
        }

        public void GetOrCreate(long id, IPEndPoint address)
        {
            Get(id, address);
        }
    }
}