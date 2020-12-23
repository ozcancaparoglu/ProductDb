using System;
using System.IO;


namespace ProductDb.PriceConsole
{
    public class Logger : ILogger
    {
        private static Logger logger = null;

        public static Logger GetLogger
        {
            get
            {
                if (logger == null)
                    logger = new Logger();

                return logger;
            }
        }
        public void WriteLog(string message)
        {
            File.AppendAllText(GetFilePath(), "Message => " + message + "  İşlem Tarihi =>" + DateTime.Now + Environment.NewLine );
        }

        public string GetFilePath()
        {
            return Path.Combine("C:\\PriceLogs.txt");
        }
    }
}
