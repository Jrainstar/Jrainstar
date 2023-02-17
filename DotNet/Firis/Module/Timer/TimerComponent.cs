using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class TimerComponent : Component, IAwake,IUpdate
    {
        private Timer timer;
        public long deltaTime => timer.deltaTime;
        public static TimerComponent Instance
        { get; set; }

        public void Awake()
        {
            timer = new Timer();
            Instance = this;
        }

        public void Update()
        {
            timer.Update();
        }

        public long Add(Action callback, int delay)
        {
            return timer.Add(new TimerTask(delay, callback));
        }

        public void Remove(int id)
        {
            timer.Remove(id);
        }

    }
}
