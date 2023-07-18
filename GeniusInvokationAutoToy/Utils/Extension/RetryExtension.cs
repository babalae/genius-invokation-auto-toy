using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Utils.Extension
{
    public static class RetryExtension
    {
        public static void InvokeWithRetries(this Action @this, ushort numberOfRetries, TimeSpan sleepPeriod, string throwsMessage)
        {
            try
            {
                @this();
            }
            catch
            {
                if (numberOfRetries == 0)
                {
                    throw new Exception(throwsMessage);
                }

                Thread.Sleep(sleepPeriod);

                InvokeWithRetries(@this, --numberOfRetries, sleepPeriod, throwsMessage);
            }
        }
    }
}
