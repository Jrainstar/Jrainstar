using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Firis
{
    public static class Timer
    {
        private static List<TimerTask> ToBeAdds;
        private static List<TimerTask> Removeds;

        private static long CurrentID;
        private static Dictionary<long, TimerTask> TimerTasks;

        private static Stopwatch Clock;

        private static long CurrentTime => Clock.ElapsedMilliseconds;

        static Timer()
        {
            ToBeAdds = new List<TimerTask>();
            Removeds = new List<TimerTask>();
            TimerTasks = new Dictionary<long, TimerTask>();
            Clock = new Stopwatch();
            Clock.Start();
        }

        public static void Add(TimerTask task)
        {
            task.NextTime = CurrentTime + task.Interval + task.Delay;
            task.TaskID = CurrentID++;
            ToBeAdds.Add(task);
        }

        public static void Remove(long taskID)
        {
            if (!TimerTasks.ContainsKey(taskID))
            {
                Log.Error("Task不存在");
                return;
            }
            TimerTasks.Remove(taskID);
        }

        public static TimerTask Get(long taskID)
        {
            if (!TimerTasks.ContainsKey(taskID))
            {
                Log.Error("Task不存在");
                return null;
            }
            return TimerTasks[taskID];
        }

        public static void Update()
        {
            TimerTask task;
            for (int i = 0; i < ToBeAdds.Count; i++)
            {
                task = ToBeAdds[i];
                TimerTasks.Add(task.TaskID, task);
            }
            ToBeAdds.Clear();

            foreach (var item in TimerTasks)
            {
                task = item.Value;
                if (task.NextTime < CurrentTime)
                {
                    task.TimeAction?.Invoke();
                    if (!task.IsLoop) Removeds.Add(task);
                    else
                    {
                        task.LoopNum--;
                        if (task.LoopNum == 0) Removeds.Add(task);
                        else task.NextTime += task.Interval;
                    }
                }
            }

            for (int i = 0; i < Removeds.Count; i++)
            {
                TimerTasks.Remove(Removeds[i].TaskID);
            }
            Removeds.Clear();
        }
    }
}

