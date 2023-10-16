using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace Firis
{
    public class EntityFactory
    {
        //static SnowFlake SnowFlake { get; set; } = new SnowFlake(1, 1);

        public static T Creat<T>() where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = SnowFlake.Instance.NextID();
            EventSystem.Instance.Awake(entity);
            return entity;
        }
        public static Entity Creat(Type type)
        {
            Entity entity = Activator.CreateInstance(type) as Entity;
            entity.ID = SnowFlake.Instance.NextID();
            EventSystem.Instance.Awake(entity);
            return entity;
        }
        public static T CreatWithID<T>(long id) where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = id;
            EventSystem.Instance.Awake(entity);
            return entity;
        }
        public static Entity CreatWithID(Type type, long id)
        {
            Entity entity = Activator.CreateInstance(type) as Entity;
            entity.ID = id;
            EventSystem.Instance.Awake(entity);
            return entity;
        }

        public static T Creat<T, A>(A a) where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = SnowFlake.Instance.NextID();
            EventSystem.Instance.Awake(entity, a);
            return entity;
        }
        public static T CreatWithID<T, A>(long id, A a) where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = id;
            EventSystem.Instance.Awake(entity, a);
            return entity;
        }

        public static T Creat<T, A, B>(A a, B b) where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = SnowFlake.Instance.NextID();
            EventSystem.Instance.Awake(entity, a, b);
            return entity;
        }
        public static T CreatWithID<T, A, B>(long id, A a, B b) where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = id;
            EventSystem.Instance.Awake(entity, a, b);
            return entity;
        }

        public static T Creat<T, A, B, C>(A a, B b, C c) where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = SnowFlake.Instance.NextID();
            EventSystem.Instance.Awake(entity, a, b, c);
            return entity;
        }
        public static T CreatWithID<T, A, B, C>(long id, A a, B b, C c) where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = id;
            EventSystem.Instance.Awake(entity, a, b, c);
            return entity;
        }
    }
}
