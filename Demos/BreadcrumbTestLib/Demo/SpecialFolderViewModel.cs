namespace BreadcrumbTestLib.Demo
{
    public class SpecialFolderViewModel
    {
        public SpecialFolderViewModel(System.Environment.SpecialFolder folder)
        {
            this.EnumSpecialFolder = folder;
        }

        public System.Environment.SpecialFolder EnumSpecialFolder { get; private set; }

        public string FolderName
        {
            get
            {
                return this.EnumSpecialFolder.ToString();
            }
        }
    }
}
