#nullable disable
using Android.Widget;
using Google.Android.Material.TextField;
using Microsoft.Maui.Controls.Internals;
using SearchView = AndroidX.AppCompat.Widget.SearchView;

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
		internal static void UpdateText(this TextInputLayout textInputLayout, InputView inputView)
		{
			var editText = textInputLayout.GetFirstChildOfType<EditText>();
			if (editText is null)
			{
				return;
			}

			var oldQuery = editText.Text?.ToString() ?? string.Empty;
			var newQuery = TextTransformUtilities.GetTransformedText(inputView.Text, inputView.TextTransform);

			if (oldQuery != newQuery)
			{
				// Preserve cursor position when applying text transform
				var cursorPosition = editText.SelectionStart;
				editText.Text = newQuery;

				// Restore cursor position, ensuring it doesn't exceed new text length
				if (cursorPosition >= 0 && cursorPosition <= newQuery?.Length)
				{
					editText.SetSelection(cursorPosition);
				}
			}
		}
	}
}