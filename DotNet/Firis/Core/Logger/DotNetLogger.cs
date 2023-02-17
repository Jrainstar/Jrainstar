using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firis
{
    public class DotNetLogger : ILogger
    {
        public static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
        public void Error(string error)
        {
            logger.Error(error);
            Console.WriteLine(error);

        }

        public void Info(string info)
        {
            //logger.Info(info);
            Console.WriteLine(info);
        }

        public void Warn(string warn)
        {
            logger.Warn(warn);
        }
    }
}
