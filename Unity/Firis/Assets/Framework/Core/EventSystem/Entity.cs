using System.Collections.Generic;
using System;
using System.Linq;

namespace Firis
{
    public abstract class Entity : ISystem, IDisposable
    {
        public long ID { get; internal set; }
        public Entity Parent { get; private set; }
        public bool IsDisposed { get; private set; }
        public int ChildCount => m_Children.Count;
        public int ComponentCount => m_Components.Count;

        private SortedDictionary<long, Entity> m_Children = new SortedDictionary<long, Entity>();
        private SortedDictionary<string, Component> m_Components = new SortedDictionary<string, Component>();

        /// <summary>
        /// 已有Component实例，将其附在此entity上（如component已有父entity，则会先移除原关系）
        /// 若组件 是 DisallowMultipleComponent 目标如果已经拥有此组件 则操作无效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public T AddComponent<T>(T component) where T : Component
        {
            Type type = typeof(T);
            if (component == null)
            {
                Log.Error($"--- 传入 {type.Name} 组件 不允许为空 --- ");
                return null;
            }

            var have = m_Components.ContainsKey(typeof(T).FullName);
            if (have)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }

            Entity raw = component.Entity;
            raw.RemoveComponent(component);

            m_Components.Add(typeof(T).FullName, component);
            component.Entity = this;
            return component;
        }

        public Component AddComponent(Type type)
        {
            Component component;
            var have = m_Components.ContainsKey(type.FullName);
            if (have)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }

            component = this.Create(type);
            m_Components.Add(type.FullName, component);
            return component;
        }

        public T AddComponent<T>() where T : Component
        {
            Type type = typeof(T);
            var have = m_Components.ContainsKey(type.FullName);
            if (have)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }

            var component = this.Create<T>();
            m_Components.Add(type.FullName, component);
            return component as T;
        }

        public T AddComponent<T, A>(A a) where T : Component
        {
            Type type = typeof(T);
            var have = m_Components.ContainsKey(type.FullName);
            if (have)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }

            var component = this.Create<T, A>(a);
            m_Components.Add(type.FullName, component);
            return component as T;
        }
        public T AddComponent<T, A, B>(A a, B b) where T : Component
        {
            Type type = typeof(T);
            var have = m_Components.ContainsKey(type.FullName);
            if (have)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }

            var component = this.Create<T, A, B>(a, b);
            m_Components.Add(type.FullName, component);
            return component as T;
        }
        public T AddComponent<T, A, B, C>(A a, B b, C c) where T : Component
        {
            Type type = typeof(T);
            var have = m_Components.ContainsKey(type.FullName);
            if (have)
            {
                Log.Error($" --- {type.Name} 组件 不允许挂载多个 --- ");
                return null;
            }

            var component = this.Create<T, A, B, C>(a, b, c);
            m_Components.Add(type.FullName, component);
            return component as T;
        }

        public Component GetComponent(Type type)
        {
            var have = m_Components.ContainsKey(type.FullName);

            if (!have)
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return null;
            }

            return m_Components[type.FullName];
        }

        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            var have = m_Components.ContainsKey(type.FullName);

            if (!have)
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return null;
            }

            return m_Components[type.FullName] as T;
        }

        public bool RemoveComponent(Type type)
        {
            var have = m_Components.ContainsKey(type.FullName);

            if (!have)
            {
                Log.Warn($" --- {type.Name} 组件 不存在 --- ");
                return false;
            }

            m_Components.Remove(type.FullName);
            return true;
        }

        public bool RemoveComponent<T>() where T : Component
        {
            var type = typeof(T);
            return RemoveComponent(type);
        }

        public bool RemoveComponent(Component component)
        {
            var type = component.GetType();
            return RemoveComponent(type);
        }

        public T AddChild<T>() where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T>();
            entity.Parent = this;
            m_Children.Add(entity.ID, entity);

            return entity as T;
        }

        public T AddChild<T, A>(A a) where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T, A>(a);
            entity.Parent = this;
            m_Children.Add(entity.ID, entity);

            return entity as T;
        }

        public T AddChild<T, A, B>(A a, B b) where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T, A, B>(a, b);
            entity.Parent = this;
            m_Children.Add(entity.ID, entity);

            return entity as T;
        }

        public T AddChild<T, A, B, C>(A a, B b, C c) where T : Entity
        {
            Entity entity;
            entity = EntityFactory.Creat<T, A, B, C>(a, b, c);
            entity.Parent = this;
            m_Children.Add(entity.ID, entity);

            return entity as T;
        }

        /// <summary>
        /// 使用时注意
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddChild<T>(T child) where T : Entity
        {
            if (child == null)
            {
                Log.Warn($" --- 不能为空! --- ");
                return false;
            }

            if (m_Children.ContainsKey(child.ID))
            {
                Log.Warn($" --- 原本 {child} 就是 子物体! --- ");
                return false;
            }

            Entity parent = this.Parent;
            while (parent != null)
            {
                if (parent == child)
                {
                    Log.Error("---禁止将上级Entity作下级Entity的子物体---");
                    return false;
                }
                parent = parent.Parent;
            }

            if (child.Parent != null)
            {
                Entity raw = child.Parent;
                raw.RemoveChild(child);
            }

            child.Parent = this;
            m_Children.Add(child.ID, child);
            return true;
        }

        /// <summary>
        /// 使用时注意 可能设及子父物体绑定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddChild(Entity child)
        {
            if (child == null)
            {
                Log.Warn($" --- 不能为空! --- ");
                return false;
            }

            if (m_Children.ContainsKey(child.ID))
            {
                Log.Warn($" --- 原本 {child} 就是 子物体! --- ");
                return false;
            }

            Entity parent = this.Parent;
            while (parent != null)
            {
                if (parent == child)
                {
                    Log.Error("---禁止将上级Entity作下级Entity的子物体---");
                    return false;
                }
                parent = parent.Parent;
            }

            if (child.Parent != null)
            {
                Entity raw = child.Parent;
                raw.RemoveChild(child);
            }

            child.Parent = this;
            m_Children.Add(child.ID, child);
            return true;
        }

        public Entity GetChild(int index)
        {
            if (index < 0 || index >= ChildCount)
            {
                Log.Error(" --- 索引异常 --- ");
                return null;
            }
            return m_Children[index];
        }

        public T GetChild<T>() where T : Entity
        {
            foreach (var child in m_Children.Values)
            {
                if (child is T)
                {
                    return (T)child;
                }
            }
            return null;
        }

        public Entity GetChild(Type type)
        {
            foreach (var child in m_Children.Values)
            {
                if (child.GetType() == type)
                {
                    return child;
                }
            }
            return null;
        }

        public IEnumerable<T> GetChildren<T>() where T : Entity
        {
            foreach (var child in m_Children.Values)
            {
                if (child is T)
                {
                    yield return child as T;
                }
            }
        }

        public IEnumerable<Entity> GetChildren(Type type)
        {
            foreach (var child in m_Children.Values)
            {
                if (child.GetType() == type)
                {
                    yield return child;
                }
            }
        }

        public void RemoveChild(Entity entity)
        {
            m_Children.Remove(entity.ID);
        }

        public virtual void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            if (Parent != null)
            {
                Parent.RemoveChild(this);
                Parent = null;
            }

            var cKeys = m_Components.Keys.ToList();
            foreach (var key in cKeys)
            {
                m_Components[key].Dispose();
            }

            var eKeys = m_Children.Keys.ToList();
            foreach (var key in eKeys)
            {
                m_Children[key].Dispose();
            }

            this.m_Components.Clear();
            this.m_Children.Clear();

            EventSystem.Instance.Remove(this);
        }
    }

}