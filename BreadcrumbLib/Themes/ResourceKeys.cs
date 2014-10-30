/*************************************************************************************

	 Extended WPF Toolkit

	 Copyright (C) 2007-2013 Xceed Software Inc.

	 This program is provided to you under the terms of the Microsoft Public
	 License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

	 For more features, controls, and fast professional support,
	 pick up the Plus Edition at http://xceed.com/wpf_toolkit

	 Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

	***********************************************************************************/
namespace BreadcrumbLib.Themes
{
	using System.Windows;

	/// <summary>
	/// Resource key management class to keep track of all resources
	/// that can be re-styled in applications that make use of the implemented controls.
	/// </summary>
	public static class ResourceKeys
	{
		#region Brushes
		public static readonly ResourceKey ControlBackground = new StaticResourceKey(typeof(ResourceKeys), "ControlBackground");

		public static readonly ResourceKey ForegroundTextBrush = new StaticResourceKey(typeof(ResourceKeys), "ForegroundTextBrush");
		public static readonly ResourceKey ControlForeground = new StaticResourceKey(typeof(ResourceKeys), "ControlForeground");
		public static readonly ResourceKey ControlBorderBrush = new StaticResourceKey(typeof(ResourceKeys), "ControlBorderBrush");

		public static readonly ResourceKey ArrowBorderForeground = new StaticResourceKey(typeof(ResourceKeys), "ArrowBorderForeground");
		public static readonly ResourceKey ArrowFillForeground = new StaticResourceKey(typeof(ResourceKeys), "ArrowFillForeground");

		public static readonly ResourceKey ArrowFillMouseOverForeground = new StaticResourceKey(typeof(ResourceKeys), "ArrowFillMouseOverForeground");
		public static readonly ResourceKey ArrowBorderMouseOverForeground = new StaticResourceKey(typeof(ResourceKeys), "ArrowBorderMouseOverForeground");

		public static readonly ResourceKey ArrowFillCheckedForeground = new StaticResourceKey(typeof(ResourceKeys), "ArrowFillCheckedForeground");
		public static readonly ResourceKey ArrowBorderCheckedForeground = new StaticResourceKey(typeof(ResourceKeys), "ArrowBorderCheckedForeground");

		public static readonly ResourceKey ButtonSelectedColor = new StaticResourceKey(typeof(ResourceKeys), "ButtonSelectedColor");
		public static readonly ResourceKey ButtonBackgoundColor = new StaticResourceKey(typeof(ResourceKeys), "ButtonBackgoundColor");

		/// <summary>
		/// 
		/// </summary>
		public static readonly ResourceKey HotTrack_SelectedBrush = new StaticResourceKey(typeof(ResourceKeys), "HotTrack_SelectedBrush");

		/// <summary>
		/// 
		/// </summary>
		public static readonly ResourceKey HotTrack_BackgroundBrush = new StaticResourceKey(typeof(ResourceKeys), "HotTrack_BackgroundBrush");

		public static readonly ResourceKey HotTrack_HighlightBrush = new StaticResourceKey(typeof(ResourceKeys), "HotTrack_HighlightBrush");

		/// <summary>
		/// This color is applied to the drop down button (Hottrack)
		/// when it is selected and the drop down is open.
		/// </summary>
		public static readonly ResourceKey OpenDropDownButton_Background = new StaticResourceKey(typeof(ResourceKeys), "OpenDropDownButton_Background");
		#endregion Brushes

		/// <summary>
		/// Decide whether styling is using a 3D depth effect or not
		/// </summary>
		public static readonly ResourceKey ThreeDStyleBrushes = new StaticResourceKey(typeof(bool), "ThreeDStyleBrushes");		
	}
}
