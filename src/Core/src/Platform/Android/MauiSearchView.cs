using System;
using Android.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using SearchView = AndroidX.AppCompat.Widget.SearchView;

namespace Microsoft.Maui.Platform;

public class MauiSearchView : SearchView
{
	internal MauiSearchViewEditText? _queryEditor;

	public MauiSearchView(Context context) : base(context)
	{
		Initialize();
	}

	void Initialize()
	{
		SetIconifiedByDefault(false);
		MaxWidth = int.MaxValue;

		// Get the original EditText from SearchView
		var originalEditText = this.GetFirstChildOfType<EditText>();

		if (originalEditText?.Parent is ViewGroup parent && Context is not null)
		{
			// Create our custom EditText that fires SelectionChanged
			_queryEditor = new MauiSearchViewEditText(Context)
			{
				Id = originalEditText.Id,
				LayoutParameters = originalEditText.LayoutParameters,
				Text = originalEditText.Text,
				Hint = originalEditText.Hint,
			};

			// Copy text appearance
			_queryEditor.SetTextColor(originalEditText.TextColors);
			_queryEditor.SetHintTextColor(originalEditText.HintTextColors);

			// Transfer visual styling from original EditText
			_queryEditor.Background = originalEditText.Background;
			_queryEditor.SetPadding(
			 originalEditText.PaddingLeft,
			 originalEditText.PaddingTop,
			 originalEditText.PaddingRight,
			 originalEditText.PaddingBottom);
			_queryEditor.SetSingleLine(true);

			// Find index and replace
			var index = parent.IndexOfChild(originalEditText);
			parent.RemoveView(originalEditText);
			parent.AddView(_queryEditor, index);

			// Subscribe to editor events to sync with SearchView's internal state
			_queryEditor.TextChanged += OnQueryEditorTextChanged;
		}

		if (_queryEditor?.LayoutParameters is LinearLayout.LayoutParams layoutParams)
		{
			layoutParams.Height = LinearLayout.LayoutParams.MatchParent;
			layoutParams.Gravity = GravityFlags.FillVertical;
		}

		var searchCloseButtonIdentifier = Resource.Id.search_close_btn;
		if (searchCloseButtonIdentifier > 0)
		{
			var image = FindViewById<ImageView>(searchCloseButtonIdentifier);
			image?.SetMinimumWidth((int?)Context?.ToPixels(44) ?? 0);
		}
	}

	internal void SyncQueryToEditor()
	{
		if (_queryEditor is not null)
		{
			_queryEditor.TextChanged -= OnQueryEditorTextChanged;
			_queryEditor.Text = Query;
			_queryEditor.TextChanged += OnQueryEditorTextChanged;

			// Place cursor at end after sync
			if (_queryEditor.Text is not null && _queryEditor.IsFocused)
			{
				_queryEditor.SetSelection(_queryEditor.Text.Length);
			}
		}
	}

	void OnQueryEditorTextChanged(object? sender, TextChangedEventArgs e)
	{
		if (_queryEditor?.Text is not null)
		{
			base.SetQuery(_queryEditor.Text, false);
		}
	}
}

// Custom EditText for SearchView that exposes SelectionChanged event.
internal class MauiSearchViewEditText : EditText
{
	public event EventHandler? SelectionChanged;

	public MauiSearchViewEditText(Context context) : base(context)
	{
	}

	public MauiSearchViewEditText(Context context, IAttributeSet? attrs) : base(context, attrs)
	{
	}

	public MauiSearchViewEditText(Context context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
	{
	}

	protected override void OnSelectionChanged(int selStart, int selEnd)
	{
		base.OnSelectionChanged(selStart, selEnd);
		SelectionChanged?.Invoke(this, EventArgs.Empty);
	}
}