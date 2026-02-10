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
		SelectionMonitor? _selectionMonitor;
		bool _initialized;

		public MauiSearchView(Context context) : base(context)
		{
			SetIconifiedByDefault(false);
			MaxWidth = int.MaxValue;
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			// Initialize when attached to window - EditText is guaranteed to exist
			if (!_initialized)
			{
				Initialize();
			}
		}

		void Initialize()
		{
			_initialized = true;

			_queryEditor = this.GetFirstChildOfType<EditText>();
			if (_queryEditor is not null)
			{
				if (_queryEditor.LayoutParameters is LinearLayout.LayoutParams layoutParams)
				{
					layoutParams.Height = LinearLayout.LayoutParams.MatchParent;
					layoutParams.Gravity = GravityFlags.FillVertical;
				}
				_selectionMonitor = new SelectionMonitor(this, _queryEditor);
			}

			var searchCloseButtonIdentifier = Resource.Id.search_close_btn;
			if (searchCloseButtonIdentifier > 0)
			{
				var image = FindViewById<ImageView>(searchCloseButtonIdentifier);

				image?.SetMinimumWidth((int?)Context?.ToPixels(44) ?? 0);
			}
		}

		internal void DisconnectSelectionMonitor()
		{
			_selectionMonitor?.Disconnect();
			_selectionMonitor?.Dispose();
			_selectionMonitor = null;
		}

		internal void InvokeSelectionChanged()
		{
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		class SelectionMonitor : Java.Lang.Object, View.IOnKeyListener
		{
			readonly MauiSearchView _searchView;
			readonly EditText _editText;
			int _lastSelectionStart = -1;
			int _lastSelectionEnd = -1;

			public SelectionMonitor(MauiSearchView searchView, EditText editText)
			{
				_searchView = searchView;
				_editText = editText;

				// Monitor key events for keyboard-based selection (Shift+arrows)
				_editText.SetOnKeyListener(this);

				// Monitor touch events for mouse/touch-based selection
				_editText.Touch += OnTouch;

				// Monitor IME actions for composition-based input (Asian languages, voice input)
				_editText.EditorAction += OnEditorAction;
			}

			public void Disconnect()
			{
				// Unwire event handlers and listeners
				if (_editText is not null)
				{
					_editText.Touch -= OnTouch;
					_editText.EditorAction -= OnEditorAction;
					_editText.SetOnKeyListener(null);
				}
			}

			void OnTouch(object? sender, View.TouchEventArgs e)
			{
				e.Handled = false; // Don't consume the event

				// Check selection after touch event completes
				_editText.Post(CheckSelectionChanged);
			}

			void OnEditorAction(object? sender, TextView.EditorActionEventArgs e)
			{
				e.Handled = false; // Don't consume the event

				// Check selection after IME action completes
				_editText.Post(CheckSelectionChanged);
			}

			public bool OnKey(View? v, Keycode keyCode, KeyEvent? e)
			{
				// Monitor key events (especially arrow keys with Shift for selection)
				if (e?.Action == KeyEventActions.Up)
				{
					// Check selection after key is processed
					_editText.Post(CheckSelectionChanged);
				}
				return false; // Don't consume the key event
			}

			void CheckSelectionChanged()
			{
				int selStart = _editText.SelectionStart;
				int selEnd = _editText.SelectionEnd;

				// Only fire event if selection actually changed
				if (selStart != _lastSelectionStart || selEnd != _lastSelectionEnd)
				{
					_lastSelectionStart = selStart;
					_lastSelectionEnd = selEnd;
					_searchView.InvokeSelectionChanged();
				}
			}
		}
	}
}
