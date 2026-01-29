#nullable disable
using System;
using Android.Text;
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
			// Non-Material platform view fallback
			if (searchBar is not Microsoft.Maui.Platform.MauiMaterialSearchBar mauiSearchBar)
			{
				searchBar.Text = inputView.Text;
				return;
			}

			var textView = mauiSearchBar._queryEditor;
			if (textView is null)
			{
				searchBar.Text = inputView.Text;
				return;
			}

			// Apply TextTransform and check if update is needed
			var transformedText = TextTransformUtilities.GetTransformedText(inputView.Text, inputView.TextTransform);
			var currentText = textView.Text ?? string.Empty;

			if (currentText == transformedText)
			{
				return;
			}

			// Preserve cursor position across text update
			var editable = textView.EditableText;
			var cursorPosition = editable != null ? Selection.GetSelectionEnd(editable) : 0;

			textView.SetText(transformedText, TextView.BufferType.Editable);

			// Restore cursor position, clamped to valid range
			var newEditable = textView.EditableText;
			if (newEditable != null)
			{
				var textLength = transformedText?.Length ?? 0;
				// If cursor was at position 0 and there's text, place it at the end (initial focus case)
				// Otherwise preserve the current position
				var targetPosition = (cursorPosition == 0 && textLength > 0) ? textLength : Math.Min(cursorPosition, textLength);
				Selection.SetSelection(newEditable, targetPosition);
			}
		}
	}
}