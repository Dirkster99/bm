namespace WSF
{
    using System.Diagnostics;

    /// <summary>
    /// Class implements base services for opening and working with folders and files in Windows.
    /// </summary>
    public static class FileSystemCommands
    {
        /// <summary>
        /// Convinience method to open Windows Explorer with a selected file (if it exists).
        /// Otherwise, Windows Explorer is opened in the location where the file should be at.
        /// Returns falsem if neither file nor given directory exist.
        /// </summary>
        /// <param name="sFileName"></param>
        /// <param name="sParent"></param>
        /// <returns></returns>
        public static bool OpenContainingFolder(string sFileName, string sParent = null)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return false;

            try
            {
                // combine the arguments together it doesn't matter if there is a space after ','
                string argument = @"/select, " + sFileName;

                System.Diagnostics.Process.Start("explorer.exe", argument);
                return true;
            }
            catch
            {
                try
                {
                    if (sParent != null)
                    {
                        // combine the arguments together it doesn't matter if there is a space after ','
                        string argument = @"/select, " + sParent;
                        System.Diagnostics.Process.Start("explorer.exe", argument);

                        return true;
                    }
                }
                catch { }

                return false;
            }
        }

        /// <summary>
        /// Opens a file with the current Windows default application.
        /// </summary>
        /// <param name="sFileName"></param>
        public static void OpenInWindows(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return;

            try
            {
                Process.Start(new ProcessStartInfo(sFileName));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Copies the given string into the Windows clipboard.
        /// </summary>
        /// <param name="sFileName"></param>
        public static void CopyPath(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return;

            try
            {
                System.Windows.Clipboard.SetText(sFileName);
            }
            catch
            {
            }
        }
    }
}
