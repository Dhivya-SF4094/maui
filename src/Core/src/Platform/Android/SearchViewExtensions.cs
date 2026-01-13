using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Google.Android.Material.Search;
using static Android.Content.Res.Resources;
using AAttribute = Android.Resource.Attribute;
using SearchView = AndroidX.AppCompat.Widget.SearchView;

namespace Microsoft.Maui.Platform
{
	public static class SearchViewExtensions
	{
		public static void UpdateText(this SearchView searchView, ISearchBar searchBar)
		{
			searchView.SetQuery(searchBar.Text, false);
		}

		public static void UpdatePlaceholder(this SearchView searchView, ISearchBar searchBar)
		{
			searchView.QueryHint = searchBar.Placeholder;
		}

		public static void UpdatePlaceholderColor(this SearchView searchView, ISearchBar searchBar, ColorStateList? defaultPlaceholderColor, EditText? editText = null)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();

			if (editText is null)
				return;

			if (searchBar?.PlaceholderColor is Graphics.Color placeholderTextColor)
			{
				if (PlatformInterop.CreateEditTextColorStateList(editText.HintTextColors, placeholderTextColor.ToPlatform()) is ColorStateList c)
				{
					editText.SetHintTextColor(c);
				}
			}
			else if (TryGetDefaultStateColor(searchView, AAttribute.TextColorHint, out var color))
			{
				editText.SetHintTextColor(color);

				var searchMagIconImage = searchView.FindViewById<ImageView>(Resource.Id.search_mag_icon);
				searchMagIconImage?.Drawable?.SetTint(color);
			}
		}

		internal static void UpdateTextColor(this SearchView searchView, ITextStyle entry)
		{
			if (TryGetDefaultStateColor(searchView, AAttribute.TextColorPrimary, out var color) &&
				searchView.GetFirstChildOfType<EditText>() is EditText editText)
			{
				if (entry.TextColor is null)
					editText.SetTextColor(color);

				var searchMagIconImage = searchView.FindViewById<ImageView>(Resource.Id.search_mag_icon);
				searchMagIconImage?.Drawable?.SetTint(color);
			}
		}

		public static void UpdateFont(this SearchView searchView, ISearchBar searchBar, IFontManager fontManager, EditText? editText = null)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();

			if (editText == null)
				return;

			editText.UpdateFont(searchBar, fontManager);
		}

		public static void UpdateVerticalTextAlignment(this SearchView searchView, ISearchBar searchBar)
		{
			searchView.UpdateVerticalTextAlignment(searchBar, null);
		}

		public static void UpdateVerticalTextAlignment(this SearchView searchView, ISearchBar searchBar, EditText? editText)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();

			if (editText == null)
				return;

			editText.UpdateVerticalAlignment(searchBar.VerticalTextAlignment, TextAlignment.Center.ToVerticalGravityFlags());
		}

		public static void UpdateMaxLength(this SearchView searchView, ISearchBar searchBar)
		{
			searchView.UpdateMaxLength(searchBar.MaxLength, null);
		}

		public static void UpdateMaxLength(this SearchView searchView, ISearchBar searchBar, EditText? editText)
		{
			searchView.UpdateMaxLength(searchBar.MaxLength, editText);
		}

		public static void UpdateMaxLength(this SearchView searchView, int maxLength, EditText? editText)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();
			editText?.SetLengthFilter(maxLength);

			var query = searchView.Query;
			var trimmedQuery = query.TrimToMaxLength(maxLength);

			if (query != trimmedQuery)
			{
				searchView.SetQuery(trimmedQuery, false);
			}
		}

		public static void UpdateIsReadOnly(this EditText editText, ISearchBar searchBar)
		{
			bool isReadOnly = !searchBar.IsReadOnly;

			editText.FocusableInTouchMode = isReadOnly;
			editText.Focusable = isReadOnly;
			editText.SetCursorVisible(isReadOnly);
		}

		public static void UpdateCancelButtonColor(this SearchView searchView, ISearchBar searchBar)
		{
			if (searchView.Resources == null)
				return;

			var searchCloseButtonIdentifier = Resource.Id.search_close_btn;

			if (searchCloseButtonIdentifier > 0)
			{
				var image = searchView.FindViewById<ImageView>(searchCloseButtonIdentifier);

				if (image is not null && image.Drawable is Drawable drawable)
				{
					if (searchBar.CancelButtonColor is not null)
						drawable.SetColorFilter(searchBar.CancelButtonColor, FilterMode.SrcIn);
					else if (TryGetDefaultStateColor(searchView, AAttribute.TextColorPrimary, out var color))
						drawable.SetColorFilter(color, FilterMode.SrcIn);
				}
			}
		}

		internal static void UpdateSearchIconColor(this SearchView searchView, ISearchBar searchBar)
		{
			if (searchView.Resources is null)
				return;

			var searchIconIdentifier = Resource.Id.search_mag_icon;

			if (searchIconIdentifier > 0)
			{
				var image = searchView.FindViewById<ImageView>(searchIconIdentifier);

				if (image?.Drawable is not null)
				{
					if (searchBar.SearchIconColor is not null)
						image.Drawable.SetColorFilter(searchBar.SearchIconColor, FilterMode.SrcIn);
					else
						image.Drawable.ClearColorFilter();
				}
			}
		}

		public static void UpdateIsTextPredictionEnabled(this SearchView searchView, ISearchBar searchBar, EditText? editText = null)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();

			if (editText == null)
				return;

			if (searchBar.IsTextPredictionEnabled)
				editText.InputType |= InputTypes.TextFlagAutoCorrect;
			else
				editText.InputType &= ~InputTypes.TextFlagAutoCorrect;
		}

		public static void UpdateIsSpellCheckEnabled(this SearchView searchView, ISearchBar searchBar, EditText? editText = null)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();

			if (editText == null)
				return;

			if (!searchBar.IsSpellCheckEnabled)
				editText.InputType |= InputTypes.TextFlagNoSuggestions;
			else
				editText.InputType &= ~InputTypes.TextFlagNoSuggestions;
		}

		public static void UpdateIsEnabled(this SearchView searchView, ISearchBar searchBar, EditText? editText = null)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();

			if (editText == null)
				return;

			editText?.Enabled = searchBar.IsEnabled;
		}

		public static void UpdateKeyboard(this SearchView searchView, ISearchBar searchBar)
		{
			searchView.SetInputType(searchBar);
		}

		public static void UpdateReturnType(this SearchView searchView, ISearchBar searchBar)
		{
			searchView.SetInputType(searchBar);
			searchView.ImeOptions = (int)searchBar.ReturnType.ToPlatform();
		}

		internal static void SetInputType(this SearchView searchView, ISearchBar searchBar, EditText? editText = null)
		{
			editText ??= searchView.GetFirstChildOfType<EditText>();

			if (editText == null)
				return;

			editText.SetInputType(searchBar);
		}

		static bool TryGetDefaultStateColor(SearchView searchView, int attribute, out Color color)
		{
			color = default;

			if (!OperatingSystem.IsAndroidVersionAtLeast(23))
				return false;

			if (searchView.Context?.Theme is not Theme theme)
				return false;

			int[] s_disabledState = [-AAttribute.StateEnabled];
			int[] s_enabledState = [AAttribute.StateEnabled];

			using var ta = theme.ObtainStyledAttributes([attribute]);
			var cs = ta.GetColorStateList(0);
			if (cs is null)
				return false;

			var state = searchView.Enabled ? s_enabledState : s_disabledState;
			color = new Color(cs.GetColorForState(state, Color.Black));
			return true;
		}

		// material3 searchbar extension methods
		// TODO: material3 - make it public in .net 11
		internal static void UpdateText(this SearchBar searchBar, ISearchBar virtualSearchBar)
		{
			searchBar.Text = virtualSearchBar.Text;
		}

		internal static void UpdatePlaceholder(this SearchBar searchBar, ISearchBar virtualSearchBar)
		{
			searchBar.Hint = virtualSearchBar.Placeholder;
		}

		internal static void UpdatePlaceholderColor(this SearchBar searchBar, ISearchBar virtualSearchBar, ColorStateList? defaultPlaceholderColor, EditText? editText = null)
		{
			var hintTextView = searchBar.GetFirstChildOfType<TextView>();

			if (hintTextView is null)
			{
				return;
			}

			if (virtualSearchBar?.PlaceholderColor is Graphics.Color placeholderTextColor)
			{
				if (PlatformInterop.CreateEditTextColorStateList(hintTextView.HintTextColors, placeholderTextColor.ToPlatform()) is ColorStateList c)
				{
					hintTextView.SetHintTextColor(c);
				}
			}
			else if (TryGetDefaultStateColor(searchBar, AAttribute.TextColorHint, out var color))
			{
				hintTextView.SetHintTextColor(color);

				var searchMagIconImage = searchBar.FindViewById<ImageView>(Resource.Id.search_mag_icon);
				searchMagIconImage?.Drawable?.SetTint(color);
			}
		}

		internal static void UpdateFont(this SearchBar searchBar, ISearchBar virtualSearchBar, IFontManager fontManager, EditText? editText = null)
		{
			// Material 3 SearchBar doesn't have EditText as direct child - it's in SearchView
			editText?.UpdateFont(virtualSearchBar, fontManager);

			// Update the hint TextView font(SearchBar) as well
			var hintTextView = searchBar.GetFirstChildOfType<TextView>();
			if (hintTextView is not null)
			{
				hintTextView.UpdateFont(virtualSearchBar, fontManager);
			}
		}

		internal static void UpdateVerticalTextAlignment(this SearchBar searchBar, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			// Material 3 SearchBar: Update vertical alignment for both collapsed and expanded states

			// Update TextView in SearchBar (collapsed state)
			var hintTextView = searchBar.GetFirstChildOfType<TextView>();
			if (hintTextView is not null)
			{
				hintTextView.UpdateVerticalAlignment(virtualSearchBar.VerticalTextAlignment, TextAlignment.Center.ToVerticalGravityFlags());
			}

			// Update EditText in SearchView (expanded state)
			// Note: editText parameter should be passed from handler (QueryEditor from SearchView)
			if (editText is not null)
			{
				editText.UpdateVerticalAlignment(virtualSearchBar.VerticalTextAlignment, TextAlignment.Center.ToVerticalGravityFlags());
			}
		}

		internal static void UpdateHorizontalTextAlignment(this SearchBar searchBar, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			var verticalGravity = virtualSearchBar.VerticalTextAlignment.ToVerticalGravityFlags();
			var horizontalGravity = virtualSearchBar.HorizontalTextAlignment.ToHorizontalGravityFlags();
			var combinedGravity = verticalGravity | horizontalGravity;

			// Update all TextViews in SearchBar (handles both text and hint)
			var textViews = searchBar.GetChildrenOfType<TextView>();
			foreach (var textView in textViews)
			{
				// Set full width and gravity in Toolbar.LayoutParams (needed for alignment to be visible)
				if (textView.LayoutParameters is AndroidX.AppCompat.Widget.Toolbar.LayoutParams toolbarParams)
				{
					toolbarParams.Width = ViewGroup.LayoutParams.MatchParent;
					toolbarParams.Gravity = (int)combinedGravity;
					textView.LayoutParameters = toolbarParams; // Reassign to trigger layout update
				}

				// Use existing extension method for text alignment and justification
				textView.UpdateHorizontalTextAlignment(virtualSearchBar);
			}

			// Update EditText in SearchView (expanded state)
			editText?.UpdateHorizontalAlignment(virtualSearchBar.HorizontalTextAlignment);
		}

		internal static void UpdateMaxLength(this SearchBar searchBar, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			searchBar.UpdateMaxLength(virtualSearchBar.MaxLength, editText);
		}

		internal static void UpdateMaxLength(this SearchBar searchBar, int maxLength, EditText? editText)
		{
			editText ??= searchBar.GetFirstChildOfType<EditText>();
			editText?.SetLengthFilter(maxLength);

			var text = editText?.Text?.ToString() ?? string.Empty;
			var trimmedText = text.TrimToMaxLength(maxLength);

			if (text != trimmedText)
			{
				editText?.Text = trimmedText;
			}
		}

		internal static void UpdateSearchIconColor(this SearchBar searchBar, ISearchBar virtualSearchBar)
		{
			// Material 3 SearchBar: Search icon is the navigation icon (Toolbar)
			// Follow the same pattern as ToolbarExtensions.UpdateIconColor for navigation icons
			if (searchBar.NavigationIcon is not Drawable navigationIcon)
			{
				return;
			}

			if (virtualSearchBar.SearchIconColor is not null)
			{
				var platformColor = virtualSearchBar.SearchIconColor.ToPlatform();
				navigationIcon.SetColorFilter(platformColor, FilterMode.SrcAtop);
			}
			else
			{
				navigationIcon.ClearColorFilter();
			}
		}

		internal static void UpdateCancelButtonColor(this SearchBar searchBar, ISearchBar virtualSearchBar)
		{
			// Material 3: Clear button is in SearchView (expanded overlay), not SearchBar (collapsed)
			var materialSearchView = (searchBar as MauiMaterialSearchBar)?.MaterialSearchView;

			if (materialSearchView is null)
			{
				return;
			}

			var searchCloseButtonIdentifier = Resource.Id.open_search_view_clear_button;

			if (searchCloseButtonIdentifier > 0)
			{
				var image = materialSearchView.FindViewById<ImageView>(searchCloseButtonIdentifier);

				if (image is not null && image.Drawable is Drawable drawable)
				{
					if (virtualSearchBar.CancelButtonColor is not null)
					{
						drawable.SetColorFilter(virtualSearchBar.CancelButtonColor, FilterMode.SrcIn);
					}
					else if (TryGetDefaultStateColor(searchBar, AAttribute.TextColorPrimary, out var color))
					{
						drawable.SetColorFilter(color, FilterMode.SrcIn);
					}
				}
			}
		}

		static ImageView? FindClearButtonInHierarchy(ViewGroup viewGroup)
		{
			// Recursively search for ImageView that looks like a clear/close button
			for (int i = 0; i < viewGroup.ChildCount; i++)
			{
				var child = viewGroup.GetChildAt(i);

				if (child is ImageView imageView && imageView.ContentDescription?.ToString()?.Contains("clear", StringComparison.OrdinalIgnoreCase) == true)
				{
					return imageView;
				}

				if (child is ViewGroup childGroup)
				{
					var result = FindClearButtonInHierarchy(childGroup);
					if (result is not null)
						return result;
				}
			}

			return null;
		}

		static bool TryGetDefaultStateColor(SearchBar searchBar, int attribute, out Color color)
		{
			color = default;

			if (!OperatingSystem.IsAndroidVersionAtLeast(23))
			{
				return false;
			}

			if (searchBar.Context?.Theme is not Theme theme)
			{
				return false;
			}

			int[] s_disabledState = [-AAttribute.StateEnabled];
			int[] s_enabledState = [AAttribute.StateEnabled];

			using var ta = theme.ObtainStyledAttributes([attribute]);
			var cs = ta.GetColorStateList(0);
			if (cs is null)
			{
				return false;
			}

			var state = searchBar.Enabled ? s_enabledState : s_disabledState;
			color = new Color(cs.GetColorForState(state, Color.Black));
			return true;
		}

		internal static void UpdateIsTextPredictionEnabled(this SearchBar searchBar, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			editText ??= searchBar.GetFirstChildOfType<EditText>();

			if (editText is null)
			{
				return;
			}

			if (!virtualSearchBar.IsSpellCheckEnabled)
			{
				editText.InputType |= InputTypes.TextFlagNoSuggestions;
			}
			else
			{
				editText.InputType &= ~InputTypes.TextFlagNoSuggestions;
			}
		}

		internal static void UpdateIsSpellCheckEnabled(this SearchBar searchBar, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			editText ??= searchBar.GetFirstChildOfType<EditText>();

			if (editText is null)
			{
				return;
			}

			if (!virtualSearchBar.IsSpellCheckEnabled)
			{
				editText.InputType |= InputTypes.TextFlagNoSuggestions;
			}
			else
			{
				editText.InputType &= ~InputTypes.TextFlagNoSuggestions;
			}

		}

		internal static void UpdateKeyboard(this SearchBar searchBar, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			if (editText is null)
			{
				return;
			}

			editText.SetInputType(virtualSearchBar);
		}

		internal static void UpdateReturnType(this Google.Android.Material.Search.SearchView materialSearchView, ISearchBar searchBar)
		{
			var editText = materialSearchView.GetFirstChildOfType<EditText>();
			if (editText is not null)
			{
				editText.SetInputType(searchBar);
				editText.ImeOptions = searchBar.ReturnType.ToPlatform();
			}
		}
	}
}