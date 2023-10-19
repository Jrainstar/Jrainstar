
namespace Jrainstar
{
    public static class Log
    {
        public static ILogger Logger;

        public static void Error(object error)
        {
            Logger.Error(error.ToString());
        }

        public static void Info(string info)
        {   
            Logger.Info(info);
        }

        public static void Warn(string warn)
        {
            Logger.Warn(warn);
        }

        public static void Warning(string warn)
        {
            Logger.Warn(warn);
        }
    }
}