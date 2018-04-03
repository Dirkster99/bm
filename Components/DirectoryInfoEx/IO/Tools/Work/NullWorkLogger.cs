namespace DirectoryInfoExLib.IO.Tools.Work
{
    using DirectoryInfoExLib.IO.Tools.Interface;

    public class NullWorkLogger : IWorkLogger
    {
        public void Log(IWork work, logType type, string message)
        {
        }
    }

}
