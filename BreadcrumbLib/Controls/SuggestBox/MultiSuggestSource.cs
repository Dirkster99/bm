namespace BreadcrumbLib.Controls.SuggestBox
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using BreadcrumbLib.Interfaces;

	public class MultiSuggestSource : ISuggestSource
	{
		private ISuggestSource[] _suggestSources;

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="suggestSources"></param>
		public MultiSuggestSource(params ISuggestSource[] suggestSources)
		{
			this._suggestSources = suggestSources;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="source1"></param>
		/// <param name="moreSources"></param>
		public MultiSuggestSource(ISuggestSource source1, params ISuggestSource[] moreSources)
		{
			this._suggestSources = (new ISuggestSource[] { source1 }).Concat(moreSources).ToArray();
		}
		#endregion constructors

		#region methods
		public async Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
		{
			List<object> retVal = new List<object>();

			foreach (var ss in this._suggestSources)
				retVal.AddRange(await ss.SuggestAsync(data, input, helper));
			return retVal;
		}
		#endregion methods
	}
}
