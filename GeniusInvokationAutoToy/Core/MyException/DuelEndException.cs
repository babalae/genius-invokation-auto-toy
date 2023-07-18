using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Core.MyException
{
    public class DuelEndException: System.Exception
    {
        public DuelEndException(string message) : base(message)
        {
        }
    }
}
