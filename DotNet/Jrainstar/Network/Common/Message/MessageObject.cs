using System;
using System.ComponentModel;

namespace Jrainstar
{
    public abstract class MessageObject : IMessage, IDisposable
    {
        public virtual void Dispose()
        {
        }

        public bool IsFromPool { get; set; }
    }
}