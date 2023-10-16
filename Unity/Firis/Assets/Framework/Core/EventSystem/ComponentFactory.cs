using System;

namespace Firis
{
    public static class ComponentFactory
    {
        public static Component Create(this Entity entity, Type type)
        {
            Component component = Activator.CreateInstance(type) as Component;
            component.Entity = entity;
            EventSystem.Instance.Awake(component);
            return component;
        }

        public static T Create<T>(this Entity entity) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            component.Entity = entity;
            EventSystem.Instance.Awake(component);
            return component;
        }

        public static T Create<T, A>(this Entity entity, A a) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            component.Entity = entity;
            EventSystem.Instance.Awake(component, a);
            return component;
        }

        public static T Create<T, A, B>(this Entity entity, A a, B b) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            component.Entity = entity;
            EventSystem.Instance.Awake(component, a, b);
            return component;
        }

        public static T Create<T, A, B, C>(this Entity entity, A a, B b, C c) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            component.Entity = entity;
            EventSystem.Instance.Awake(component, a, b, c);
            return component;
        }

        public static T Create<T, A, B, C, D>(this Entity entity, A a, B b, C c, D d) where T : Component
        {
            T component = Activator.CreateInstance<T>();
            component.Entity = entity;
            EventSystem.Instance.Awake(component, a, b, c, d);
            return component;
        }
    }
}
