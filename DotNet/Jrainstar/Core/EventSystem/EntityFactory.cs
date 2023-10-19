using System;
using System.Collections.Generic;
using System.Linq;

namespace Jrainstar
{
    public abstract class Entity : ISystem, IDisposable
    {
        public long ID { get; set; } // 预留ID 若数据库信息不以Entity保存 则不需要 
        public Entity Parent { get; set; }
        public bool IsDisposed { get; set; }
        public int childCount => Children.Count;

        private List<Entity> Children = new List<Entity>();
        private Dictionary<Type, List<Component>> Components = new Dictionary<Type, List<Component>>();

        public Component AddComponent(Type type)
        {
            Component component;
            if (!Components.ContainsKey(type))
            {
                Components.Add(type, new List<Component>());
            }
            bool allowMult = true;
            var attributes = type.GetCustomAttributes(typeof(DisallowMultipleComponentAttribute), true);
            if (attributes.Count() != 0)
            {
                allowMult = false;
            }
            var components = Components[type];
            if (components.Count != 0 && !allowMult)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }
            component = ComponentFactory.Create(type);
            component.Entity = this;
            components.Add(component);
            return component;
        }

        public T AddComponent<T>() where T : Component
        {
            Type type = typeof(T);
            return AddComponent(type) as T;
        }

        /// <summary>
        /// 已有Component实例，将其附在此entity上（如component已有父entity，则会先移除原关系）
        /// 若组件 是 DisallowMultipleComponent 目标如果已经拥有此组件
        /// isSafe 等于 false 放弃
        /// isSafe 等于 true 将目标的该组件移除 再顶替
        /// 不推荐使用此种方式 因为 dispose 调用后 数据 任然会消失
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="isSafe"></param>
        /// <returns></returns>
        public T AddComponent<T>(T component, bool isSafe) where T : Component
        {
            Type type = typeof(T);
            if (component == null)
            {
                Log.Error($"--- 传入 {type.Name} 组件 不允许为空 --- ");
                return null;
            }

            bool allowMult = true;
            var attributes = type.GetCustomAttributes(true);
            if (attributes.Contains(typeof(DisallowMultipleComponentAttribute)))
            {
                allowMult = false;
            }

            var components = Components[type];
            if (components.Count != 0 && !allowMult)
            {
                if (isSafe)
                {
                    Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                    return null;
                }
                RemoveComponent<T>(component);
            }

            Entity raw = component.Entity;
            raw.RemoveComponent<T>(component, false);

            components.Add(component);
            component.Entity = this;
            return null;
        }

        public T AddComponent<T, A>(A a) where T : Component
        {
            Type type = typeof(T);
            Component component;
            if (!Components.ContainsKey(type))
            {
                Components.Add(type, new List<Component>());
            }
            bool allowMult = true;
            var attributes = type.GetCustomAttributes(true);
            if (attributes.Contains(typeof(DisallowMultipleComponentAttribute)))
            {
                allowMult = false;
            }
            var components = Components[type];
            if (components.Count != 0 && !allowMult)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }
            component = ComponentFactory.Create<T, A>(a);
            component.Entity = this;
            components.Add(component);
            return component as T;
        }
        public T AddComponent<T, A, B>(A a, B b) where T : Component
        {
            Type type = typeof(T);
            Component component;
            if (!Components.ContainsKey(type))
            {
                Components.Add(type, new List<Component>());
            }
            bool allowMult = true;
            var attributes = type.GetCustomAttributes(true);
            if (attributes.Contains(typeof(DisallowMultipleComponentAttribute)))
            {
                allowMult = false;
            }
            var components = Components[type];
            if (components.Count != 0 && !allowMult)
            {
                Console.WriteLine($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }
            component = ComponentFactory.Create<T, A, B>(a, b);
            component.Entity = this;
            components.Add(component);
            return component as T;
        }
        public T AddComponent<T, A, B, C>(A a, B b, C c) where T : Component
        {
            Type type = typeof(T);
            Component component;
            if (!Components.ContainsKey(type))
            {
                Components.Add(type, new List<Component>());
            }
            bool allowMult = true;
            var attributes = type.GetCustomAttributes(true);
            if (attributes.Contains(typeof(DisallowMultipleComponentAttribute)))
            {
                allowMult = false;
            }
            var components = Components[type];
            if (components.Count != 0 && !allowMult)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }
            component = ComponentFactory.Create<T, A, B, C>(a, b, c);
            component.Entity = this;
            components.Add(component);
            return component as T;
        }

        public Component GetComponent(Type type)
        {
            if (!Components.ContainsKey(type))
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return null;
            }
            var components = Components[type];
            if (components.Count == 0)
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return null;
            }
            return components[0];
        }
        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            return GetComponent(type) as T;
        }

        public List<Component> GetComponents(Type type)
        {
            if (!Components.ContainsKey(type))
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return null;
            }
            var components = Components[type];
            return components;
        }
        public List<T> GetComponents<T>() where T : Component
        {
            Type type = typeof(T);
            var components = GetComponents(type);
            return components as List<T>;
        }

        public void RemoveComponent<T>() where T : Component
        {
            Type type = typeof(T);
            RemoveComponent(type);
        }
        public void RemoveComponent(Type type)
        {
            if (!Components.ContainsKey(type))
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return;
            }
            var components = Components[type];
            if (components != null && components.Count != 0)
            {
                components[0].Dispose();
                components.RemoveAt(0);
            }
        }

        public void RemoveComponent<T>(T component, bool dispose = true) where T : Component
        {
            RemoveComponent(component, dispose);
        }

        public void RemoveComponent(Component component, bool dispose)
        {
            Type type = component.GetType();
            if (!Components.ContainsKey(type))
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return;
            }
            var components = Components[type];
            if (!components.Contains(component))
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return;
            }
            components.Remove(component);
            if (dispose) component.Dispose();
        }

        public T AddChild<T>() where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T>();
            entity.Parent = this;
            Children.Add(entity);

            return entity as T;
        }

        public T AddChild<T, A>(A a) where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T, A>(a);
            entity.Parent = this;
            Children.Add(entity);

            return entity as T;
        }

        public T AddChild<T, A, B>(A a, B b) where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T, A, B>(a, b);
            entity.Parent = this;
            Children.Add(entity);

            return entity as T;
        }

        public T AddChild<T, A, B, C>(A a, B b, C c) where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T, A, B, C>(a, b, c);
            entity.Parent = this;
            Children.Add(entity);

            return entity as T;
        }

        /// <summary>
        /// 使用时注意 可能设及子父物体绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T AddChild<T>(T child) where T : Entity
        {
            if (child.Parent != null)
            {
                Entity raw = child.Parent;
                raw.RemoveChild(child);
            }

            if (Children.Contains(child))
            {
                Log.Warn($" --- 原本 {child} 就是 子物体! --- ");
                return null;
            }
            else
            {
                child.Parent = this;
                Children.Add(child);
            }
            return child as T;
        }

        /// <summary>
        /// 使用时注意 可能设及子父物体绑定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Entity AddChild(Entity child)
        {
            if (child.Parent != null)
            {
                Entity raw = child.Parent;
                raw.RemoveChild(child);
            }

            if (Children.Contains(child))
            {
                Log.Warn($" --- 原本 {child} 就是 子物体! --- ");
                return null;
            }
            else
            {
                child.Parent = this;
                Children.Add(child);
            }
            return child;
        }

        public Entity GetChild(int index)
        {
            if (index < 0 || index >= childCount)
            {
                Log.Error(" --- 索引异常 --- ");
                return null;
            }
            return Children[index];
        }

        public T GetChild<T>() where T : Entity
        {
            var child = Children.Where(entity => entity is T).First();
            return child as T;
        }

        public Entity GetChild(Type type)
        {
            var child = Children.Where(entity => entity.GetType() == type).First();
            return child;
        }

        public List<T> GetChildren<T>() where T : Entity
        {
            var children = Children.Where(entity => entity is T);
            return children.ToList() as List<T>;
        }

        public List<Entity> GetChildren(Type type)
        {
            var children = Children.Where(entity => entity.GetType() == type);
            return children.ToList();
        }

        public void RemoveChild(Entity entity, bool dispose = true)
        {
            Children.Remove(entity);
            if (dispose) entity.Dispose();
        }

        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            this.IsDisposed = true;
            foreach (var comps in Components.Values)
            {
                foreach (var comp in comps)
                {
                    EventSystem.Instance.Remove(comp);
                    comp.Entity = null;
                    comp.Dispose();
                }
            }
            foreach (Entity entity in Children)
            {
                EventSystem.Instance.Remove(entity);
                entity.Parent = null;
                entity.Dispose();
            }

            this.Components.Clear();
            this.Children.Clear();
            this.Parent?.RemoveChild(this);
            EventSystem.Instance.Remove(this);
        }
    }

    public class EntityFactory
    {
        //static SnowFlake SnowFlake { get; set; } = new SnowFlake(1, 1);

        public static T Creat<T>() where T : Entity
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = SnowFlake.Instance.NextId();
            EventSystem.Instance.Awake(entity);
            return entity;
        }
        public static Entity Creat(Type type)
        {
            Entity entity = Activator.CreateInstance(type) as Entity;
            entity.ID = SnowFlake.Instance.NextId();
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
            entity.ID = SnowFlake.Instance.NextId();
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
            entity.ID = SnowFlake.Instance.NextId();
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
            entity.ID = SnowFlake.Instance.NextId();
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
