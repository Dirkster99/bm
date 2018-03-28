using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Tools
{
    public class NullWorkLogger : IWorkLogger
    {
        public void Log(IWork work, logType type, string message)
        {
        }
    }

}
