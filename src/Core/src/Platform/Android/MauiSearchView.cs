using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.IO;
using SearchView = AndroidX.AppCompat.Widget.SearchView;

namespace Microsoft.Maui.Platform
{
	public class MauiSearchView : SearchView
	{
		internal EditText? _queryEditor;
		internal event EventHandler? SelectionChanged;
		int _previousSelectionStart = -1;
		int _previousSelectionEnd = -1;
		Java.Lang.Runnable? _checkSelectionRunnable;

		public MauiSearchView(Context context) : base(context)
		{
			Initialize();
		}

		void Initialize()
		{
			SetIconifiedByDefault(false);
			MaxWidth = int.MaxValue;

			_queryEditor = this.GetFirstChildOfType<EditText>();

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

			// Set up touch listener to detect cursor position changes
			if (_queryEditor is not null)
			{
				_checkSelectionRunnable = new Java.Lang.Runnable(() => CheckSelectionChanged());
				_queryEditor.Touch += OnQueryEditorTouch;
			}
		}

		void OnQueryEditorTouch(object? sender, TouchEventArgs e)
		{
			if (e.Event?.Action == MotionEventActions.Up)
			{
				// Post a check after the touch is processed and cursor position is updated
				_queryEditor?.PostDelayed(_checkSelectionRunnable, 50);
			}
			e.Handled = false;
		}

		internal void CheckSelectionChanged()
		{
			if (_queryEditor is not null)
			{
				int selStart = _queryEditor.SelectionStart;
				int selEnd = _queryEditor.SelectionEnd;

				if (selStart != _previousSelectionStart || selEnd != _previousSelectionEnd)
				{
					_previousSelectionStart = selStart;
					_previousSelectionEnd = selEnd;
					SelectionChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		internal void StartMonitoringSelection()
		{
			if (_queryEditor is not null)
			{
				_queryEditor.Touch += OnQueryEditorTouch;
			}
		}

		internal void StopMonitoringSelection()
		{
			if (_queryEditor is not null)
			{
				_queryEditor.Touch -= OnQueryEditorTouch;
				_queryEditor.RemoveCallbacks(_checkSelectionRunnable);
			}
		}
	}
}
