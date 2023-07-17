using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeniusInvokationAutoToy.Utils
{

    public class MyLogger
    {
        static Logger _logger => LogManager.GetCurrentClassLogger();

        public static void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }
        public static void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }
        public static void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }
        public static void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }
        public static void Fatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }
        public static void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

    }
}
