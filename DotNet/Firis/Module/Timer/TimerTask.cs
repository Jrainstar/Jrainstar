using System;
using System.Collections;
using System.Collections.Generic;

namespace Firis
{
    public class TimerTask
    {
        public long TaskID;
        public int Delay;            // 第一次触发要隔多少时间; 以毫秒为单位
        public int Interval;         // 定时器触发的时间间隔;   以毫秒为单位
        public bool IsLoop;          // 是否重复触发
        public int LoopNum;          // 你要触发的次数;
        public long NextTime;        // 下次触发的时间
        public Action TimeAction;    // 用户要传的参数

        public TimerTask(int Delay, Action TimeAction)
        {
            this.Delay = Delay;
            this.IsLoop = false;
            this.TimeAction = TimeAction;
        }

        public TimerTask(int Delay, bool IsLoop, int Interval, int LoopNum, Action TimeAction)
        {
            this.Delay = Delay;
            this.Interval = Interval;
            this.IsLoop = IsLoop;
            this.LoopNum = LoopNum;
            this.TimeAction = TimeAction;
        }
    }
}