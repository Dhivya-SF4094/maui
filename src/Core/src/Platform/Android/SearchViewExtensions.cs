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
using Google.Android.Material.TextField;
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
				SafeSetTint(searchMagIconImage, color);
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
				SafeSetTint(searchMagIconImage, color);
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
				if (searchBar.CancelButtonColor is not null)
					SafeSetTint(image, searchBar.CancelButtonColor.ToPlatform());
				else if (TryGetDefaultStateColor(searchView, AAttribute.TextColorPrimary, out var color))
					SafeSetTint(image, color);
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
					{
						SafeSetTint(image, searchBar.SearchIconColor.ToPlatform());
					}
					else if (TryGetDefaultStateColor(searchView, AAttribute.TextColorPrimary, out var color))
					{
						SafeSetTint(image, color);
					}
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

		/// <summary>
		/// Safely applies tint to an ImageView's drawable by mutating it first.
		/// This prevents crashes when the drawable is shared across multiple views.
		/// </summary>
		/// <remarks>
		/// Android shares Drawable resources for memory efficiency. Modifying a shared
		/// drawable without calling Mutate() first causes race conditions and crashes.
		/// See: https://developer.android.com/reference/android/graphics/drawable/Drawable#mutate()
		/// </remarks>
		internal static void SafeSetTint(ImageView? imageView, Color color)
		{
			if (imageView?.Drawable is not Drawable drawable)
				return;

			var safe = drawable.Mutate();
			safe.SetTint(color);
			imageView?.SetImageDrawable(safe);
		// material3 searchbar extension methods
		// TODO: material3 - make it public in .net 11
		internal static void UpdateText(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			editText ??= textInputLayout.EditText;

			if (editText is null)
			{
				return;
			}

			// Check if text is already the same to prevent unnecessary updates and TextWatcher loops
			var currentText = editText.Text ?? string.Empty;
			var newText = virtualSearchBar.Text ?? string.Empty;

			if (currentText == newText)
			{
				return;
			}

			editText.Text = newText;

			// Update close button visibility based on whether text exists (Material2 behavior)
			if (textInputLayout is MauiMaterialTextInputLayout materialLayout)
			{
				materialLayout.UpdateCloseButtonVisibility(!string.IsNullOrEmpty(newText));
			}
		}

		internal static void UpdateBackground(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar)
		{
			var background = virtualSearchBar.Background;

			if (background is Microsoft.Maui.Graphics.SolidPaint solidPaint)
			{
				// For Material 3 filled TextInputLayout, set the box background color
				textInputLayout.BoxBackgroundColor = solidPaint.Color.ToPlatform();
			}
			else
			{
				// For other background types, use the standard ViewExtensions approach
				ViewExtensions.UpdateBackground(textInputLayout, virtualSearchBar);
			}
		}

		internal static void UpdatePlaceholder(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar)
		{
			// Set hint on the EditText for traditional placeholder behavior (not floating label)
			var editText = textInputLayout.EditText;
			editText?.Hint = virtualSearchBar.Placeholder;
		}

		internal static void UpdatePlaceholderColor(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar)
		{
			var hintTextView = textInputLayout.EditText;

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
			else if (TryGetDefaultStateColor(textInputLayout, AAttribute.TextColorHint, out var color))
			{
				hintTextView.SetHintTextColor(color);

				var searchMagIconImage = textInputLayout.FindViewById<ImageView>(Resource.Id.search_mag_icon);
				searchMagIconImage?.Drawable?.SetTint(color);
			}
		}

		internal static void UpdateIsEnabled(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			textInputLayout.Enabled = virtualSearchBar.IsEnabled;

			editText ??= textInputLayout.EditText;
			if (editText is not null)
			{
				editText.Enabled = virtualSearchBar.IsEnabled;

				// Clear focus and hide keyboard when disabled
				if (!virtualSearchBar.IsEnabled && editText.HasFocus)
				{
					editText.ClearFocus();
				}
			}
		}

		internal static void UpdateMaxLength(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			textInputLayout.UpdateMaxLength(virtualSearchBar.MaxLength, editText);
		}

		internal static void UpdateMaxLength(this TextInputLayout textInputLayout, int maxLength, EditText? editText)
		{
			editText ??= textInputLayout.EditText;
			editText?.SetLengthFilter(maxLength);

			var text = editText?.Text?.ToString() ?? string.Empty;
			var trimmedText = text.TrimToMaxLength(maxLength);

			if (text != trimmedText)
			{
				editText?.Text = trimmedText;
			}
		}

		internal static void UpdateFont(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, IFontManager fontManager, EditText? editText = null)
		{
			editText ??= textInputLayout.EditText;

			if (editText is null)
			{
				return;
			}

			editText.UpdateFont(virtualSearchBar, fontManager);
		}

		internal static void UpdateTextColor(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			editText ??= textInputLayout.EditText;

			if (editText is null)
			{
				return;
			}

			editText.UpdateTextColor(virtualSearchBar);
		}

		internal static void UpdateSearchIconColor(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar)
		{
			// For TextInputLayout, the search icon is the start icon
			if (virtualSearchBar.SearchIconColor is not null)
			{
				var color = virtualSearchBar.SearchIconColor.ToPlatform();
				textInputLayout.SetStartIconTintList(ColorStateList.ValueOf(color));
			}
			else if (TryGetDefaultStateColor(textInputLayout, AAttribute.TextColorPrimary, out var defaultColor))
			{
				// Restore default theme color
				textInputLayout.SetStartIconTintList(ColorStateList.ValueOf(defaultColor));
			}
		}

		internal static void UpdateCancelButtonColor(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar)
		{
			// For TextInputLayout, the cancel/clear button is the end icon
			if (virtualSearchBar.CancelButtonColor is not null)
			{
				var color = virtualSearchBar.CancelButtonColor.ToPlatform();
				textInputLayout.SetEndIconTintList(ColorStateList.ValueOf(color));
			}
			else if (TryGetDefaultStateColor(textInputLayout, AAttribute.TextColorPrimary, out var defaultColor))
			{
				// Restore default theme color
				textInputLayout.SetEndIconTintList(ColorStateList.ValueOf(defaultColor));
			}
		}

		static bool TryGetDefaultStateColor(TextInputLayout textInputLayout, int attribute, out Color color)
		{
			color = default;

			if (!OperatingSystem.IsAndroidVersionAtLeast(23))
			{
				return false;
			}

			if (textInputLayout.Context?.Theme is not Theme theme)
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

			var state = textInputLayout.Enabled ? s_enabledState : s_disabledState;
			color = new Color(cs.GetColorForState(state, Color.Black));
			return true;
		}

		internal static void UpdateIsTextPredictionEnabled(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			editText ??= textInputLayout.EditText;

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

		internal static void UpdateIsSpellCheckEnabled(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			editText ??= textInputLayout.EditText;

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

		internal static void UpdateKeyboard(this TextInputLayout textInputLayout, ISearchBar virtualSearchBar, EditText? editText = null)
		{
			editText ??= textInputLayout.EditText;

			if (editText is null)
			{
				return;
			}

			editText.SetInputType(virtualSearchBar);
		}

		internal static void UpdateReturnType(this TextInputLayout textInputLayout, ISearchBar searchBar, EditText? editText = null)
		{
			editText ??= textInputLayout.EditText;
			if (editText is null)
			{
				return;
			}

			editText.SetInputType(searchBar);
			editText.ImeOptions = (ImeAction)(int)searchBar.ReturnType.ToPlatform();
		}
	}
}
