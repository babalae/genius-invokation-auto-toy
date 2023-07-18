using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeniusInvokationAutoToy.Core.MyException
{
    public class RetryException : System.Exception
    {
        public RetryException() : base()
        {
        }

        public RetryException(string message) : base(message)
        {
        }
    }
}