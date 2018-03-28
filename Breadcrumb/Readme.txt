
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

