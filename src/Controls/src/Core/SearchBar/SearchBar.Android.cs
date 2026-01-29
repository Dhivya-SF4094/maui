#nullable disable
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;

namespace Microsoft.Maui.Controls
{
	public partial class SearchBar
	{
		public static void MapText(SearchBarHandler handler, SearchBar searchBar) =>
			MapText((ISearchBarHandler)handler, searchBar);

		public static void MapText(ISearchBarHandler handler, SearchBar searchBar)
		{
			Platform.SearchViewExtensions.UpdateText(handler.PlatformView, searchBar);
		}

		// Material3 specific overload for SearchBarHandler2
		internal static void MapText(SearchBarHandler2 handler, SearchBar searchBar)
		{
			// Use Controls layer extension that applies TextTransform
			Platform.SearchViewExtensions.UpdateText(handler.PlatformView, searchBar);
		}
	}
}
