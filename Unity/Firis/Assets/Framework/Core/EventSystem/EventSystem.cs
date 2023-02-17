using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Firis
{
    #region 生命周期接口
    public interface ISystem
    {
        void Dispose();
    }
    public interface IAwake : ISystem
    {
        void Awake();
    }
    public interface IAwake<A> : ISystem
    {
        void Awake(A a);
    }
    public interface IAwake<A, B> : ISystem
    {
        void Awake(A a, B b);
    }
    public interface IAwake<A, B, C> : ISystem
    {
        void Awake(A a, B b, C c);
    }

    public interface IAwake<A, B, C, D> : ISystem
    {
        void Awake(A a, B b, C c, D d);
    }

    public interface IStart : ISystem
    {
        void Start();
    }

    public interface IUpdate : ISystem
    {
        void Update();
    }

    public interface ILateUpdate : ISystem
    {
        void LateUpdate();
    }

    #endregion
    public class EventSystem
    {
        public static EventSystem Instance { get; set; } = new EventSystem();

        #region 生命周期

        private IStart start;
        private IUpdate update;
        private ILateUpdate lateUpdate;

        public HashSet<IStart> Starts { get; private set; } = new HashSet<IStart>();
        public HashSet<IUpdate> Updates { get; private set; } = new HashSet<IUpdate>();
        public HashSet<ILateUpdate> LateUpdates { get; private set; } = new HashSet<ILateUpdate>();

        private ConcurrentQueue<IStart> ToBeAddStart { get; set; } = new ConcurrentQueue<IStart>();
        private ConcurrentQueue<IUpdate> ToBeAddUpdate { get; set; } = new ConcurrentQueue<IUpdate>();
        private ConcurrentQueue<ILateUpdate> ToBeAddLateUpdate { get; set; } = new ConcurrentQueue<ILateUpdate>();
        private ConcurrentQueue<ISystem> ToBeRemoveSet { get; set; } = new ConcurrentQueue<ISystem>();

        // 执行Awake(若有)，加入各生命周期组(若有) 外部可能是多线程调用
        public void Awake(ISystem obj)
        {
            if (obj is IAwake awaker) awaker.Awake();
            AwakeObject(obj);
        }

        public void Awake<A>(ISystem obj, A a)
        {
            if (obj is IAwake<A> awaker) awaker.Awake(a);
            AwakeObject(obj);
        }

        public void Awake<A, B>(ISystem obj, A a, B b)
        {
            if (obj is IAwake<A, B> awaker) awaker.Awake(a, b);
            AwakeObject(obj);
        }

        public void Awake<A, B, C>(ISystem obj, A a, B b, C c)
        {
            if (obj is IAwake<A, B, C> awaker) awaker.Awake(a, b, c);
            AwakeObject(obj);
        }

        public void Awake<A, B, C, D>(ISystem obj, A a, B b, C c, D d)
        {
            if (obj is IAwake<A, B, C, D> awaker) awaker.Awake(a, b, c, d);
            AwakeObject(obj);
        }

        // 外部可能多线程调用
        private void AwakeObject(ISystem obj)
        {
            if (obj is IStart start) ToBeAddStart.Enqueue(start);
            if (obj is IUpdate update) ToBeAddUpdate.Enqueue(update);
            if (obj is ILateUpdate lateupdate) ToBeAddLateUpdate.Enqueue(lateupdate);
        }

        // 加入待移除队列，生命周期 LateUpdate 处理
        public void Remove(ISystem obj)
        {
            if (obj != null) ToBeRemoveSet.Enqueue(obj);
        }

        // 服务端
        public void Run()
        {
            while (true)
            {
                try
                {
                    Update();
                    Thread.Sleep(1);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // Unity客户端 服务器 
        public void Update()
        {
            OnUpdate();
            OnLateUpdate();
            OnFrameFinish();
        }

        // 必定单线程调用
        public void OnUpdate()
        {
            //待添加项处理
            while (ToBeAddStart.Count > 0)
            {
                ToBeAddStart.TryDequeue(out start);
                Starts.Add(start);
            }
            while (ToBeAddUpdate.Count > 0)
            {
                ToBeAddUpdate.TryDequeue(out update);
                Updates.Add(update);
            }
            while (ToBeAddLateUpdate.Count > 0)
            {
                ToBeAddLateUpdate.TryDequeue(out lateUpdate);
                LateUpdates.Add(lateUpdate);
            }

            //晚于Awake而早于Update
            if (Starts.Count > 0)
            {
                foreach (IStart start in Starts)
                {
                    try
                    {
                        start.Start();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                //只执行一次，清空
                Starts.Clear();
            }

            //update
            foreach (IUpdate updater in Updates)
            {
                try
                {
                    updater.Update();
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
        }

        public void OnLateUpdate()
        {
            //update
            foreach (ILateUpdate updater in LateUpdates)
            {
                try
                {
                    updater.LateUpdate();
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
        }

        // 用于移除对象的处理
        public void OnFrameFinish()
        {
            //待移除项处理 
            while (ToBeRemoveSet.Count > 0)
            {
                ISystem toRemove;
                ToBeRemoveSet.TryDequeue(out toRemove);
                Updates.Remove(toRemove as IUpdate);
                LateUpdates.Remove(toRemove as ILateUpdate);
            }
        }

        //public static void FixedUpdate()
        //{
        //    OnFixedUpdate();
        //}

        //public static void OnFixedUpdate()
        //{

        //}

        #endregion

        #region 事件中心
        public interface IEvent { }

        public class EventAction : IEvent
        {
            public Action actions;

            public EventAction(Action action)
            {
                actions += action;
            }
        }

        public class EventAction<T> : IEvent
        {
            public Action<T> actions;

            public EventAction(Action<T> action)
            {
                actions += action;
            }

        }

        public class EventFunc<T> : IEvent
        {
            public Func<T> funcs;

            public EventFunc(Func<T> func)
            {
                funcs += func;
            }
        }

        public class EventFunc<T, K> : IEvent
        {
            public Func<T, K> funcs;

            public EventFunc(Func<T, K> func)
            {
                funcs += func;
            }
        }

        private ConcurrentDictionary<int, IEvent> events = new ConcurrentDictionary<int, IEvent>();

        #region 添加事件监听
        public void AddEventListener(int name, Action action)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventAction).actions += action;//将委托事件添加进去
            }
            else
            {
                events.TryAdd(name, new EventAction(action));
            }
        }

        public void AddEventListener<T>(int name, Action<T> action)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventAction<T>).actions += action;//将委托事件添加进去
            }
            else
            {
                events.TryAdd(name, new EventAction<T>(action));
            }
        }

        public void AddEventListener<T>(int name, Func<T> func)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventFunc<T>).funcs += func;//将委托事件添加进去
            }
            else
            {
                events.TryAdd(name, new EventFunc<T>(func));
            }
        }

        public void AddEventListener<T, K>(int name, Func<T, K> func)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventFunc<T, K>).funcs += func;//将委托事件添加进去
            }
            else
            {
                events.TryAdd(name, new EventFunc<T, K>(func));
            }
        }
        #endregion

        #region 移除事件监听
        public void RemoveEventListener(int name, Action action)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventAction).actions -= action;
                if ((events[name] as EventAction).actions == null)
                {
                    ClearEventListener(name);
                }
            }
        }

        public void RemoveEventListener<T>(int name, Action<T> action)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventAction<T>).actions -= action;
                if ((events[name] as EventAction<T>).actions == null)
                {
                    ClearEventListener(name);
                }
            }
        }

        public void RemoveEventListener<T>(int name, Func<T> func)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventFunc<T>).funcs -= func;
                if ((events[name] as EventFunc<T>).funcs == null)
                {
                    ClearEventListener(name);
                }
            }
        }

        public void RemoveEventListener<T, K>(int name, Func<T, K> func)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventFunc<T, K>).funcs -= func;
                if ((events[name] as EventFunc<T, K>).funcs == null)
                {
                    ClearEventListener(name);
                }
            }
        }
        #endregion

        #region 触发监听事件
        /// <summary>
        /// 执行事件触发
        /// </summary>
        /// <param name="name"> 事件名字</param>
        public void EventTrigger(int name)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventAction).actions?.Invoke();
            }
        }

        public void EventTrigger<T>(int name, T info)
        {
            if (events.ContainsKey(name))
            {
                (events[name] as EventAction<T>).actions?.Invoke(info);
            }
        }

        public T EventTrigger<T>(int name)
        {
            T result = default;
            if (events.ContainsKey(name))
            {
                result = (events[name] as EventFunc<T>).funcs();
            }
            return result;
        }

        public K EventTrigger<T, K>(int name, T info)
        {
            K result = default;
            if (events.ContainsKey(name))
            {
                result = (events[name] as EventFunc<T, K>).funcs.Invoke(info);
            }
            return result;
        }
        #endregion

        #region 清空监听事件
        public void ClearEventListener(int name)
        {
            if (events.ContainsKey(name)) events.Remove(name, out IEvent _);
        }
        #endregion

        #region 清空所有事件监听
        public void Clear()
        {
            events.Clear();
        }
        #endregion

        #endregion

        #region 特性中心
        private Dictionary<string, List<Type>> types { get; set; } = new Dictionary<string, List<Type>>();

        public void Load(string assemblyName)
        {
            if (types.ContainsKey(assemblyName))
            {
                Log.Warn(" 不能重复载入 ");
                return;
            }

            types.Add(assemblyName, new List<Type>());
            Assembly current = Assembly.Load(assemblyName);
            Type[] allTypes = current.GetTypes();

            foreach (Type type in allTypes)
            {
                FirisAttribute target = type.GetCustomAttribute<FirisAttribute>();
                if (target == null) continue;
                types[assemblyName].Add(type);
            }
        }

        public List<Type> GetAllTypes()
        {
            List<Type> allTypes = new List<Type>();
            foreach (var types in types.Values)
            {
                for (int i = 0; i < types.Count; i++)
                {
                    allTypes.Add(types[i]);
                }
            }
            return allTypes;
        }
        #endregion
    }
}
