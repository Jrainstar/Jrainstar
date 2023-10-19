using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jrainstar
{
    public class Timer
    {
        private List<TimerTask> ToBeAdds;
        private List<TimerTask> Removeds;

        private long CurrentID;
        private Dictionary<long, TimerTask> TimerTasks;

        private Stopwatch Clock;
        public long CurrentTime => Clock.ElapsedMilliseconds;

        public long lastTime { get; set; }
        public long deltaTime { get; set; }

        public Timer()
        {
            ToBeAdds = new List<TimerTask>();
            Removeds = new List<TimerTask>();
            TimerTasks = new Dictionary<long, TimerTask>();
            Clock = new Stopwatch();
            Clock.Start();
        }

        public long Add(TimerTask task)
        {
            task.NextTime = CurrentTime + task.Interval + task.Delay;
            task.TaskID = CurrentID++;
            ToBeAdds.Add(task);
            return task.TaskID;
        }

        public void Remove(long taskID)
        {
            if (!TimerTasks.ContainsKey(taskID))
            {
                Log.Error("Task不存在");
                return;
            }
            TimerTasks.Remove(taskID);
        }

        public TimerTask Get(long taskID)
        {
            if (!TimerTasks.ContainsKey(taskID))
            {
                Log.Error("Task不存在");
                return null;
            }
            return TimerTasks[taskID];
        }

        public void Update()
        {
            deltaTime = CurrentTime - lastTime;
            lastTime = CurrentTime;
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

