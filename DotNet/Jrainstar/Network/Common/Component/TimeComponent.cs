﻿namespace Jrainstar
{
    public class TimeComponent : Component, IAwake, IUpdate
    {
        public static TimeComponent Instance { get; private set; }

        private int timeZone;

        public int TimeZone
        {
            get
            {
                return timeZone;
            }
            set
            {
                timeZone = value;
                dt = dt1970.AddHours(TimeZone);
            }
        }

        private DateTime dt1970;
        private DateTime dt;

        // ping消息会设置该值，原子操作
        public long ServerMinusClientTime { private get; set; }

        public long FrameTime { get; private set; }

        public void Awake()
        {
            Instance = this;
            dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            FrameTime = ClientNow();
        }

        public void Update()
        {
            // 赋值long型是原子操作，线程安全
            FrameTime = ClientNow();
        }

        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }

        // 线程安全
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - dt1970.Ticks) / 10000;
        }

        public long ServerNow()
        {
            return ClientNow() + ServerMinusClientTime;
        }

        public long ClientFrameTime()
        {
            return FrameTime;
        }

        public long ServerFrameTime()
        {
            return FrameTime + ServerMinusClientTime;
        }

        public long Transition(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 10000;
        }
    }
}
