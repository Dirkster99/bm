using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Tools
{
    public class FileWorkLogger : IWorkLogger
    {
        protected class logLine
        {
            public DateTime Time { get; private set; }
            public logType Type { get; private set; }
            public string Message { get; private set; }

            public logLine(logType type, string message)
            {
                Time = DateTime.Now;
                Type = type;
                Message = message;
            }
        }

        #region Constructor
        public FileWorkLogger(string fileName)
        {
            _fileName = fileName;
        }

        #endregion

        #region Methods

        public void Log(IWork work, logType type, string message)
        {
            lock (_logList)
            {
                if (_lastLog.Type != type || _lastLog.Message != message)
                    _logList.Add(_lastLog = new logLine(type, message));


                if (!String.IsNullOrEmpty(_fileName) && (type == logType.error || type == logType.success))
                    using (StreamWriter sw = new StreamWriter(File.OpenWrite(_fileName)))
                        foreach (var line in _logList)
                            sw.WriteLine(String.Format("[{0}] {1} - {2}", line.Time, line.Type.ToString(), line.Message));
            }
        }

        #endregion

        #region Data

        private List<logLine> _logList = new List<logLine>();
        private string _fileName;
        private logLine _lastLog = new logLine(logType.unknown, "");

        #endregion

        #region Public Properties

        public string LogFileName { get { return _fileName; } }

        #endregion

    }
}
