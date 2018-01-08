using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerHelper.Services
{
    public interface ILogger
    {
        void LogInfoMessage(string message);
        void LogDebugMessage(string message);
        void LogFatalMessage(string message);
    }

    public class Logger : ILogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void LogInfoMessage(string message)
        { 
            log.Info(message); 
        }
        public void LogDebugMessage(string message)
        {
            log.Debug(message); 
        }
        public void LogFatalMessage(string message)
        {
            log.Fatal(message); 
        }
        public void LogWarnMessage(string message)
        {
            log.Warn(message); 
        }
    }

    public class WrieToFiles
    {
        public static readonly Object obj =  new Object();
        public static void WriteToFile (string fileName, string message)
        {
            lock (obj)
            {
                string path = ConfigurationManager.AppSettings["logPath"];
                System.IO.File.AppendAllText(path + fileName, message + Environment.NewLine);
            }
        }
    }
}
