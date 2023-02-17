using System;

namespace Firis
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DisallowMultipleComponentAttribute : Attribute
    {

    }

    public abstract class Component : ISystem, IDisposable
    {
        public Entity Entity { get; internal set; }
        public bool IsDisposed { get; set; }
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;
            if (Entity != null)
            {
                Entity.RemoveComponent(GetType());
                Entity = null;
            }
            EventSystem.Instance.Remove(this);
        }
    }
    public class ComponentFactory
    {
        //public static Component Create(Type type)
        //{
        //    Component component = Activator.CreateInstance(type) as Component;
        //    EventSystem.Instance.Awake(component);
        //    return component;
        //}

        public static Component Create(Type type, Entity entity)
        {
            Component component = Activator.CreateInstance(type) as Component;
            component.Entity = entity;
            EventSystem.Instance.Awake(component);
            return component;
        }

        public static T Create<T>(Entity entity) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            component.Entity = entity;
            EventSystem.Instance.Awake(component);
            return component;
        }

        public static T Create<T, A>(A a) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            EventSystem.Instance.Awake(component, a);
            return component;
        }

        public static T Create<T, A, B>(A a, B b) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            EventSystem.Instance.Awake(component, a, b);
            return component;
        }

        public static T Create<T, A, B, C>(A a, B b, C c) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            EventSystem.Instance.Awake(component, a, b, c);
            return component;
        }

        public static T Create<T, A, B, C, D>(A a, B b, C c, D d) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            EventSystem.Instance.Awake(component, a, b, c, d);
            return component;
        }
    }
}
