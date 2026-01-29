#nullable disable
using Android.Widget;
using Microsoft.Maui.Controls.Internals;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using MaterialSearchBar = Google.Android.Material.Search.SearchBar;

namespace Microsoft.Maui.Controls.Platform
{
	public static class SearchViewExtensions
	{
		public static void UpdateText(this SearchView searchView, InputView inputView)
		{
			var oldQuery = searchView.Query ?? string.Empty;
			var newQuery = TextTransformUtilities.GetTransformedText(inputView.Text, inputView.TextTransform);

			if (oldQuery != newQuery)
				searchView.SetQuery(newQuery, false);
		}

		// Material3 SearchBar - apply TextTransform and update text directly
		internal static void UpdateText(this MaterialSearchBar searchBar, InputView inputView)
		{
			var text = inputView.Text ?? string.Empty;

			// Non-Material platform view
			if (searchBar is not MauiMaterialSearchBar mauiSearchBar)
			{
				if (searchBar.Text != text)
				{
					searchBar.Text = text;
				}

				return;
			}

			var textView = mauiSearchBar._queryEditor;
			if (textView is null)
			{
				if (searchBar.Text != text)
				{
					searchBar.Text = text;
				}

				return;
			}

			// Apply TextTransform using Controls utilities
			var transformedText = TextTransformUtilities.GetTransformedText(text, inputView.TextTransform);
			var currentText = textView.Text ?? string.Empty;

			if (currentText != transformedText)
			{
				textView.SetText(transformedText, TextView.BufferType.Editable);
			}
		}
	}
}