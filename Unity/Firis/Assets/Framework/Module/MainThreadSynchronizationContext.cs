using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Firis
{
    public class MainThreadSynchronizationContext : SynchronizationContext
    {
        public static MainThreadSynchronizationContext Instance { get; } = new MainThreadSynchronizationContext(Thread.CurrentThread.ManagedThreadId);

        private readonly int threadId;

        // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private Action a;

        public MainThreadSynchronizationContext(int threadId)
        {
            this.threadId = threadId;
        }

        public void Update()
        {
            while (true)
            {
                if (!queue.TryDequeue(out a))
                {
                    return;
                }

                try
                {
                    a();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            Post(() => callback(state));
        }

        public void Post(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == threadId)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                return;
            }

            queue.Enqueue(action);
        }

        public void PostNext(Action action)
        {
            queue.Enqueue(action);
        }
    }
}