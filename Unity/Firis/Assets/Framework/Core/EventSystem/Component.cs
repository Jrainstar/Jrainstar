using System;

namespace Firis
{
    public abstract class Component : ISystem, IDisposable
    {
        public Entity Entity { get; internal set; }
        public bool IsDisposed { get; private set; }
        public virtual void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (Entity != null)
            {
                Entity.RemoveComponent(this);
                Entity = null;
            }
            EventSystem.Instance.Remove(this);
        }
    }
}