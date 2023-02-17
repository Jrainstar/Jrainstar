
namespace Firis
{
    public static class Log
    {
        public static ILogger Logger;

        public static void Error(string error)
        {
            Logger.Error(error);
        }

        public static void Info(string info)
        {   
            Logger.Info(info);
        }

        public static void Warn(string warn)
        {
            Logger.Warn(warn);
        }
    }
}