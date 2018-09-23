namespace SuggestBoxTestLib.ViewModels
{
    using BmLib.Controls.SuggestBox;
    using BmLib.Interfaces.SuggestBox;
    using SuggestBoxDemo.SuggestSource;
    using System;
    using System.Collections.Generic;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public AppViewModel()
        {
            SuggestBoxDummy_SuggestSources = new List<ISuggestSource>(new[] { new DummySuggestSource() });

            FakeViewModel fvm = new FakeViewModel("Root");
            for (int i = 1; i < 10; i++)
                fvm.SubDirectories.Add(new FakeViewModel("Sub" + i.ToString(), "Sub" + i.ToString() + "1", "Sub" + i.ToString() + "2"));

            // Construct SuggestBoxAuto properties
            SuggestBoxAuto_RootItems = fvm;
            SuggestBoxAuto_HierarchyHelper =
                    new PathHierarchyHelper("Parent", "Value", "SubDirectories");

            // Construct SuggestBoxAuto2 properties
            SuggestBoxAuto2_HierarchyHelper
                = new PathHierarchyHelper("Parent", "Value", "SubDirectories");

            SuggestBoxAuto2_RootItems = FakeViewModel.GenerateFakeViewModels(TimeSpan.FromSeconds(0.5));

            SuggestBoxAuto2_SuggestSources = new List<ISuggestSource>(
                new[] {
                    //This is default value, suggest based on HierarchyLister.List()
                    new AutoSuggestSource()
                }); 
        }
        #endregion constructors

        #region properties
        public List<ISuggestSource> SuggestBoxDummy_SuggestSources { get; }

        #region SuggestBoxAuto
        public PathHierarchyHelper SuggestBoxAuto_HierarchyHelper { get; }

        public FakeViewModel SuggestBoxAuto_RootItems { get; }
        #endregion SuggestBoxAuto

        #region SuggestBoxAuto2
        public PathHierarchyHelper SuggestBoxAuto2_HierarchyHelper { get; }

        public FakeViewModel SuggestBoxAuto2_RootItems { get; }

        public List<ISuggestSource>  SuggestBoxAuto2_SuggestSources { get; }
        #endregion SuggestBoxAuto2
        #endregion properties

        #region methods
        #endregion methods
    }
}
