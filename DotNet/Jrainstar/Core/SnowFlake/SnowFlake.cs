using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrainstar
{

    /**
     * twitter的snowflake算法 -- java实现
     * 
     * @author beyond
     * @date 2016/11/26
     */
    public class SnowFlake
    {

        /**
         * 起始的时间戳
         */
        private static long START_STMP = 1480166465631L;

        /**
         * 每一部分占用的位数
         */
        private static int SEQUENCE_BIT = 12; //序列号占用的位数
        private static int MACHINE_BIT = 5;   //机器标识占用的位数
        private static int DATACENTER_BIT = 5;//数据中心占用的位数

        /**
         * 每一部分的最大值
         */
        private static long MAX_DATACENTER_NUM = -1L ^ -1L << DATACENTER_BIT;   // 31 
        private static long MAX_MACHINE_NUM = -1L ^ -1L << MACHINE_BIT;         // 31 
        private static long MAX_SEQUENCE = -1L ^ -1L << SEQUENCE_BIT;

        /**
         * 每一部分向左的位移
         */
        private static int MACHINE_LEFT = SEQUENCE_BIT;
        private static int DATACENTER_LEFT = SEQUENCE_BIT + MACHINE_BIT;
        private static int TIMESTMP_LEFT = DATACENTER_LEFT + DATACENTER_BIT;

        private long datacenterId;  //数据中心
        private long machineId;     //机器标识
        private long sequence = 0L; //序列号
        private long lastStmp = -1L;//上一次时间戳

        public static SnowFlake Instance { get; set; } = new SnowFlake(1,1);

        public SnowFlake(long datacenterId, long machineId)
        {
            if (datacenterId > MAX_DATACENTER_NUM || datacenterId < 0)
            {
                throw new ArgumentException("数据中心ID不能大于最大和小于0");
            }
            if (machineId > MAX_MACHINE_NUM || machineId < 0)
            {
                throw new ArgumentException("机器码ID不能大于最大和小于0");
            }
            this.datacenterId = datacenterId;
            this.machineId = machineId;
        }

        private int acceptIdGenerator = int.MinValue;
        public uint NextUintId()
        {
            return (uint)Interlocked.Add(ref this.acceptIdGenerator, 1);
        }

        public long NextId()
        {
            long currStmp = ToUnixTimeSeconds();
            if (currStmp < lastStmp)
            {
                Console.WriteLine("时钟倒退。拒绝生成ID");
            }

            if (currStmp == lastStmp)
            {
                //相同毫秒内，序列号自增
                sequence = sequence + 1 & MAX_SEQUENCE;
                //同一毫秒的序列数已经达到最大
                if (sequence == 0L)
                {
                    currStmp = NextMill();
                }
            }
            else
            {
                //不同毫秒内，序列号置为0
                sequence = 0L;
            }

            lastStmp = currStmp;

            return currStmp - START_STMP << TIMESTMP_LEFT   //时间戳部分
                    | datacenterId << DATACENTER_LEFT       //数据中心部分
                    | machineId << MACHINE_LEFT             //机器标识部分
                    | sequence;                             //序列号部分
        }

        private long NextMill()
        {
            long mill = ToUnixTimeSeconds();
            while (mill <= lastStmp)
            {
                mill = ToUnixTimeSeconds();
            }
            return mill;
        }

        private long ToUnixTimeSeconds()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}
