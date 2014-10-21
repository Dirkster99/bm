namespace Breadcrumb.Defines
{
	using System;

	public enum HierarchicalResult : int
	{
		Parent = 1 << 1,

		Current = 1 << 2,

		Child = 1 << 3,

		Unrelated = 1 << 4,

		Related = Parent | Current | Child,

		All = Related | Unrelated
	}

	public enum UpdateMode
	{
		Replace,
		
		Update
	}

	public enum FileAccess
	{
		Read,
		
		ReadWrite,
		
		Write
	}
}
