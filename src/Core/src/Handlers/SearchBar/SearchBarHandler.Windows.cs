#nullable enable
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.Maui.Handlers
{
	public partial class SearchBarHandler : ViewHandler<ISearchBar, AutoSuggestBox>
	{
		bool _set;

		public AutoSuggestBox? QueryEditor => null;

		protected override AutoSuggestBox CreatePlatformView() => new AutoSuggestBox
		{
			AutoMaximizeSuggestionArea = false,
			QueryIcon = new SymbolIcon(Symbol.Find),
		};

		protected override void ConnectHandler(AutoSuggestBox platformView)
		{
			platformView.Loaded += OnLoaded;
			platformView.QuerySubmitted += OnQuerySubmitted;
			platformView.TextChanged += OnTextChanged;
			//In ViewHandler.Windows, FocusManager.GotFocus and LostFocus are handled for other controls. 
			// However, for AutoSuggestBox, when handling the GotFocus or LostFocus methods, tapping the AutoSuggestBox causes e.NewFocusedElement and e.OldFocusedElement to be a TextBox (which receives the focus). 
			// As a result, when comparing the PlatformView with the appropriate handler in FocusManagerMapping, the condition is not satisfied, causing the focus and unfocus methods to not work correctly. 
			// To address this, I have specifically handled the focus and unfocus events for AutoSuggestBox here. 
			platformView.GotFocus += OnGotFocus;
			platformView.LostFocus += OnLostFocus;
		}

		protected override void DisconnectHandler(AutoSuggestBox platformView)
		{
			platformView.Loaded -= OnLoaded;
			platformView.QuerySubmitted -= OnQuerySubmitted;
			platformView.TextChanged -= OnTextChanged;
			platformView.GotFocus -= OnGotFocus;
			platformView.LostFocus -= OnLostFocus;

			if (_set)
			{
				var queryTextBox = platformView.GetFirstDescendant<TextBox>();
				if (queryTextBox is not null)
				{
					queryTextBox.SelectionChanged -= OnSelectionChanged;
				}
			}

			_set = false;
		}

		public static void MapBackground(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateBackground(searchBar);
		}

		public static void MapIsEnabled(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateIsEnabled(searchBar);
		}

		internal static void MapCursorPosition(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateCursorPosition(searchBar);
		}

		internal static void MapSelectionLength(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateSelectionLength(searchBar);
		}

		public static void MapText(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateText(searchBar);
		}

		public static void MapPlaceholder(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdatePlaceholder(searchBar);
		}

		public static void MapVerticalTextAlignment(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateVerticalTextAlignment(searchBar);
		}

		public static void MapPlaceholderColor(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdatePlaceholderColor(searchBar);
		}

		public static void MapHorizontalTextAlignment(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateHorizontalTextAlignment(searchBar);
		}

		public static void MapFont(ISearchBarHandler handler, ISearchBar searchBar)
		{
			var fontManager = handler.GetRequiredService<IFontManager>();

			handler.PlatformView?.UpdateFont(searchBar, fontManager);
		}

		public static void MapCharacterSpacing(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateCharacterSpacing(searchBar);
		}

		public static void MapTextColor(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler?.PlatformView?.UpdateTextColor(searchBar);
		}

		public static void MapIsTextPredictionEnabled(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateIsTextPredictionEnabled(searchBar);
		}

		public static void MapIsSpellCheckEnabled(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateIsSpellCheckEnabled(searchBar);
		}

		public static void MapMaxLength(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateMaxLength(searchBar);
		}

		public static void MapIsReadOnly(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateIsReadOnly(searchBar);
		}

		public static void MapCancelButtonColor(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateCancelButtonColor(searchBar);
		}

		internal static void MapSearchIconColor(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateSearchIconColor(searchBar);
		}

		public static void MapKeyboard(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateKeyboard(searchBar);
		}

		public static void MapReturnType(ISearchBarHandler handler, ISearchBar searchBar)
		{
			handler.PlatformView?.UpdateReturnType(searchBar);
		}

		void OnLoaded(object sender, UI.Xaml.RoutedEventArgs e)
		{
			if (VirtualView != null)
			{
				// Subscribe to SelectionChanged event on the inner TextBox
				if (!_set)
				{
					var queryTextBox = PlatformView?.GetFirstDescendant<TextBox>();
					if (queryTextBox is not null)
					{
						queryTextBox.SelectionChanged += OnSelectionChanged;
					}

					_set = true;
				}

				PlatformView?.UpdateTextColor(VirtualView);
				PlatformView?.UpdatePlaceholderColor(VirtualView);
				PlatformView?.UpdateHorizontalTextAlignment(VirtualView);
				PlatformView?.UpdateMaxLength(VirtualView);
				PlatformView?.UpdateIsReadOnly(VirtualView);
				PlatformView?.UpdateIsTextPredictionEnabled(VirtualView);
				PlatformView?.UpdateIsSpellCheckEnabled(VirtualView);
				PlatformView?.UpdateCancelButtonColor(VirtualView);
				PlatformView?.UpdateSearchIconColor(VirtualView);
				PlatformView?.UpdateKeyboard(VirtualView);
				PlatformView?.UpdateReturnType(VirtualView);
				PlatformView?.UpdateCursorPosition(VirtualView);
				PlatformView?.UpdateSelectionLength(VirtualView);
			}
		}

		void OnQuerySubmitted(AutoSuggestBox? sender, AutoSuggestBoxQuerySubmittedEventArgs e)
		{
			if (VirtualView == null)
				return;

			// Modifies the text of the control if it does not match the query.
			// This is possible because OnTextChanged is fired with a delay
			if (e.QueryText != VirtualView.Text)
				VirtualView.Text = e.QueryText;

			VirtualView.SearchButtonPressed();
		}

		void OnTextChanged(AutoSuggestBox? sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			if (e.Reason == AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
				return;

			if (VirtualView == null || sender == null)
				return;

			VirtualView.Text = sender.Text;
		}

		void OnGotFocus(object sender, UI.Xaml.RoutedEventArgs e)
		{
			UpdateIsFocused(true);
		}

		void OnLostFocus(object sender, UI.Xaml.RoutedEventArgs e)
		{
			UpdateIsFocused(false);
		}

		void OnSelectionChanged(object sender, UI.Xaml.RoutedEventArgs e)
		{
			if (sender is not TextBox textBox)
			{
				return;
			}

			var cursorPosition = textBox.GetCursorPosition();
			var selectedTextLength = textBox.SelectionLength;

			if (VirtualView.CursorPosition != cursorPosition)
			{
				VirtualView.CursorPosition = cursorPosition;
			}

			if (VirtualView.SelectionLength != selectedTextLength)
			{
				VirtualView.SelectionLength = selectedTextLength;
			}
		}
	}
}
