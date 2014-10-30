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
	using System;
	using System.Windows;

	/// <summary>
	/// Implements a static resource key object that can be used to manage resource key
	/// references (for theming of UI elements) in WPF.
	/// </summary>
	public sealed class StaticResourceKey : ResourceKey
	{
		#region fields
		private string mKey;
		private Type mType;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="type"></param>
		/// <param name="key"></param>
		public StaticResourceKey(Type type, string key)
		{
			this.mType = type;
			this.mKey = key;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets the key value displayed in the slider control.
		/// </summary>
		public string Key
		{
			get
			{
				return this.mKey;
			}
		}

		/// <summary>
		/// Gets the type of ResourceKey managed in this object.
		/// </summary>
		public Type Type
		{
			get
			{
				return this.mType;
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Gets the assembly that implements the type of resource key managed in this object.
		/// </summary>
		public override System.Reflection.Assembly Assembly
		{
			get
			{
				return this.mType.Assembly;
			}
		}
		#endregion methods
	}
}
