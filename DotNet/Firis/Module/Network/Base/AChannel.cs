using System;
using System.IO;
using System.Net;

namespace Firis
{
    public enum ChannelType
    {
        Connect,
        Accept,
    }

    public abstract class AChannel : IDisposable
    {
        public long Id;

        public ChannelType ChannelType { get; protected set; }

        public int Error { get; set; }

        public IPEndPoint RemoteAddress { get; set; }


        public bool IsDisposed
        {
            get
            {
                return Id == 0;
            }
        }

        public abstract void Dispose();
    }
}