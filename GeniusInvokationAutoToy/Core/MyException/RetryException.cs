using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Core.MyException
{
    public class RetryException : System.Exception
    {
        public RetryException(string message) : base(message)
        {
        }
    }
}
