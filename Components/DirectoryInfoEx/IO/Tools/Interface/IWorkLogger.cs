using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Tools
{
    public enum logType { unknown, start, progress, paused, resumed, error, success }
    public interface IWorkLogger
    {
        void Log(IWork work, logType type, string message);
    }

}
