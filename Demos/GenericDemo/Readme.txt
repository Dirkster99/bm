
Next items to re-factor:

1> Construct SuggestSources property in
   bm\Breadcrumb\ViewModels\BreadcrumbViewModel.cs

   The property maps into (my guess):
   ISuggestSource FileExplorer.Models.IProfile.SuggestSource { get; }
   
   ...but where is it constructed and how can we construct it
   without implementing the complete FileExplorer framework?

   See namespace FileExplorer.WPF.Models.ProfileBase

2> Enable and fix lines '////' in
   bm\Breadcrumb\ViewModels\BreadcrumbViewModel.cs

   -> Enable OnSuggestPathChanged

-> Refactoring History:

- 20180418 Introduced ProgressViewModel, Custom TaskScheduler, BrowseRequest and FinalBrowseResult

- 20180416 refactored DirectoryInfoEx to its bare bones (whats really needed for Breadcrumb)