namespace BreadcrumbLib.Defines
{
	using System.Windows.Input;

	public static class ExplorerCommands
	{
		/***
				public static RoutedUICommand UpOneLevel = new RoutedUICommand(Strings.strInvertSelect, "InvertSelect", typeof(ExplorerCommands));
				public static RoutedUICommand InvertSelect = new RoutedUICommand(Strings.strInvertSelect, "InvertSelect", typeof(ExplorerCommands));
				public static RoutedUICommand UnselectAll = new RoutedUICommand(Strings.strUnselectAll, "UnselectAll", typeof(ExplorerCommands));
				public static RoutedUICommand ToggleCheckBox = new RoutedUICommand(Strings.strToggleCheckBox, "ToggleCheckBox", typeof(ExplorerCommands));
				public static RoutedUICommand ToggleViewMode = new RoutedUICommand(Strings.strToggleViewMode, "ToggleViewMode", typeof(ExplorerCommands));
				public static RoutedUICommand TogglePreviewer = new RoutedUICommand(Strings.strTogglePreviewer, "TogglePreviewer", typeof(ExplorerCommands));

				public static RoutedUICommand NewFolder = new RoutedUICommand(Strings.strNewFolder, "NewFolder", typeof(ExplorerCommands));

				public static RoutedUICommand NewTab = new RoutedUICommand(Strings.strNewTab, "NewTab", typeof(ExplorerCommands));
				public static RoutedUICommand OpenTab = new RoutedUICommand(Strings.strOpenInNewTab, "OpenInNewTab", typeof(ExplorerCommands),
						new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.T, ModifierKeys.Control) }));
				public static RoutedUICommand CloseTab = new RoutedUICommand(Strings.strCloseTab, "CloseTab", typeof(ExplorerCommands),
										new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F4, ModifierKeys.Control) }));

				public static RoutedUICommand NewWindow = new RoutedUICommand(Strings.strOpenInNewWindow, "OpenInNewWindow", typeof(ExplorerCommands));
				public static RoutedUICommand CloseWindow = new RoutedUICommand(Strings.strCloseWindow, "CloseWindow", typeof(ExplorerCommands));

				public static RoutedUICommand Map = new RoutedUICommand("Map", "Map", typeof(ExplorerCommands));
				public static RoutedUICommand Unmap = new RoutedUICommand("Unmap", "Unmap", typeof(ExplorerCommands));
		***/
		public static RoutedUICommand Refresh = new RoutedUICommand(Strings.StrRefresh, "Refresh", typeof(ExplorerCommands),
				new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.R, ModifierKeys.Control) }));
		/***
				public static RoutedUICommand Rename = new RoutedUICommand(Strings.strRename, "Rename", typeof(ExplorerCommands),
						new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F2) }));

				public static RoutedUICommand ToggleBreadcrumb = new RoutedUICommand("", "ToggleBreadcrumb", typeof(ExplorerCommands),
						new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.D, ModifierKeys.Alt) }));
		 ***/
	}
}
