using System;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views.InputMethods;
using Android.Widget;
using AView = Android.Views.View;

namespace Microsoft.Maui.Handlers;

// TODO: material3 - make it public in .net 11
internal class SearchBarHandler2 : ViewHandler<ISearchBar, MauiMaterialSearchBar>
{
    public static PropertyMapper<ISearchBar, SearchBarHandler2> Mapper =
   new(ViewMapper)
   {
       [nameof(ISearchBar.Background)] = MapBackground,
       [nameof(ISearchBar.CharacterSpacing)] = MapCharacterSpacing,
       [nameof(ISearchBar.Font)] = MapFont,
       [nameof(ITextAlignment.HorizontalTextAlignment)] = MapHorizontalTextAlignment,
       [nameof(ITextAlignment.VerticalTextAlignment)] = MapVerticalTextAlignment,
       [nameof(ISearchBar.IsReadOnly)] = MapIsReadOnly,
       [nameof(ISearchBar.IsTextPredictionEnabled)] = MapIsTextPredictionEnabled,
       [nameof(ISearchBar.IsSpellCheckEnabled)] = MapIsSpellCheckEnabled,
       [nameof(ISearchBar.MaxLength)] = MapMaxLength,
       [nameof(ISearchBar.Placeholder)] = MapPlaceholder,
       [nameof(ISearchBar.PlaceholderColor)] = MapPlaceholderColor,
       [nameof(ISearchBar.Text)] = MapText,
       [nameof(ISearchBar.TextColor)] = MapTextColor,
       [nameof(ISearchBar.CancelButtonColor)] = MapCancelButtonColor,
       [nameof(ISearchBar.SearchIconColor)] = MapSearchIconColor,
       [nameof(ISearchBar.Keyboard)] = MapKeyboard,
       [nameof(ISearchBar.ReturnType)] = MapReturnType,
       [nameof(ISearchBar.FlowDirection)] = MapFlowDirection,
       [nameof(ISearchBar.IsEnabled)] = MapIsEnabled,
   };

    public static CommandMapper<ISearchBar, SearchBarHandler2> CommandMapper =
      new(ViewCommandMapper)
      {
          [nameof(ISearchBar.Focus)] = MapFocus
      };

    FocusChangeListener FocusListener { get; } = new FocusChangeListener();
    TextWatcher TextListener { get; } = new TextWatcher();

    static ColorStateList? DefaultPlaceholderTextColors { get; set; }

    MauiMaterialSearchBar? _platformSearchView;

    public TextView? QueryEditor => _platformSearchView?._queryEditor;

    public SearchBarHandler2() : base(Mapper, CommandMapper)
    {
    }

    public SearchBarHandler2(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null)
        : base(mapper ?? Mapper, commandMapper ?? CommandMapper)
    {
    }

    protected override MauiMaterialSearchBar CreatePlatformView()
    {
        _platformSearchView = new MauiMaterialSearchBar(Context);
        return _platformSearchView;
    }

    protected override void ConnectHandler(MauiMaterialSearchBar platformView)
    {
        FocusListener.Handler = this;
        TextListener.Handler = this;

        // Set up text change listener
        _platformSearchView?.SetTextChangedListener(TextListener);

        // Focus change listener for the internal TextView
        if (QueryEditor is TextView editor)
        {
            editor.FocusChange += OnFocusChange;
            editor.EditorAction += OnEditorAction;
        }

        // Subscribe to close button click and set initial visibility
        _platformSearchView?.CloseButtonClicked += OnCloseButtonClicked;

        var hasText = !string.IsNullOrEmpty(VirtualView?.Text);
        _platformSearchView?.UpdateCloseButtonVisibility(hasText);
    }

    protected override void DisconnectHandler(MauiMaterialSearchBar platformView)
    {
        FocusListener.Handler = null;
        TextListener.Handler = null;
        _platformSearchView?.SetTextChangedListener(null);

        if (QueryEditor is TextView editor)
        {
            editor.FocusChange -= OnFocusChange;
            editor.EditorAction -= OnEditorAction;
        }

        _platformSearchView?.CloseButtonClicked -= OnCloseButtonClicked;
    }

    public static void MapBackground(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateBackground(searchBar);
    }

    public static void MapIsEnabled(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView.UpdateIsEnabled(searchBar, handler.QueryEditor);
    }

    public static void MapText(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateText(searchBar);
    }

    public static void MapPlaceholder(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdatePlaceholder(searchBar);
    }

    public static void MapPlaceholderColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdatePlaceholderColor(searchBar, DefaultPlaceholderTextColors, handler.QueryEditor);
    }

    internal static void MapFlowDirection(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        if (searchBar.FlowDirection == FlowDirection.MatchParent && searchBar.Parent is not null && searchBar.Parent is IView parentView)
        {
            // When FlowDirection is MatchParent, respect the parent's FlowDirection
            if (handler.PlatformView is AView platformView)
            {
                platformView.UpdateFlowDirection(parentView);
            }

            if (handler.QueryEditor is TextView textView)
            {
                textView.UpdateFlowDirection(parentView);
            }
        }
        else
        {
            // Otherwise, use the SearchBar's own FlowDirection
            handler.PlatformView?.UpdateFlowDirection(searchBar);
            handler.QueryEditor?.UpdateFlowDirection(searchBar);
        }
    }

    public static void MapFont(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        var fontManager = handler.GetRequiredService<IFontManager>();
        handler.QueryEditor?.UpdateFont(searchBar, fontManager);
    }

    public static void MapHorizontalTextAlignment(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateHorizontalTextAlignment(searchBar);
    }

    public static void MapVerticalTextAlignment(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        // Material 3 SearchBar uses TextView internally, update it directly
        handler.QueryEditor?.UpdateVerticalAlignment(searchBar.VerticalTextAlignment, TextAlignment.Center.ToVerticalGravityFlags());
    }

    public static void MapCharacterSpacing(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateCharacterSpacing(searchBar);
    }

    public static void MapTextColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateTextColor(searchBar);
    }

    public static void MapIsTextPredictionEnabled(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateIsTextPredictionEnabled(searchBar, handler.QueryEditor);
    }

    public static void MapIsSpellCheckEnabled(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateIsSpellCheckEnabled(searchBar, handler.QueryEditor);
    }

    public static void MapMaxLength(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView.UpdateMaxLength(searchBar, handler.QueryEditor);
    }

    public static void MapIsReadOnly(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateIsReadOnly(searchBar, handler.QueryEditor);
    }

    public static void MapCancelButtonColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateCancelButtonColor(searchBar);
    }

    internal static void MapSearchIconColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateSearchIconColor(searchBar);
    }

    public static void MapKeyboard(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.UpdateValue(nameof(ISearchBar.Text));

        handler.PlatformView?.UpdateKeyboard(searchBar);
    }

    public static void MapFocus(SearchBarHandler2 handler, ISearchBar _, object? args)
    {
        if (args is FocusRequest request)
        {
            handler.QueryEditor?.Focus(request);
        }
    }

    public static void MapReturnType(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateReturnType(searchBar);
    }

    void OnEditorAction(object? sender, TextView.EditorActionEventArgs e)
    {
        if (e.ActionId == ImeAction.Search || e.ActionId == ImeAction.Done)
        {
            VirtualView?.SearchButtonPressed();
            e.Handled = true;
        }
        else
        {
            e.Handled = false;
        }
    }

    void OnFocusChange(object? sender, AView.FocusChangeEventArgs e)
    {
        if (VirtualView is ISearchBar searchBar)
        {
            searchBar.IsFocused = e.HasFocus;
        }
    }

    void OnCloseButtonClicked(object? sender, EventArgs e)
    {
        if (VirtualView is ISearchBar searchBar)
        {
            // Clear text will trigger text change which updates close button visibility
            searchBar.Text = string.Empty;
        }
    }

    class TextWatcher : Java.Lang.Object, ITextWatcher
    {
        public SearchBarHandler2? Handler { get; set; }

        public void AfterTextChanged(IEditable? s)
        {
        }

        public void BeforeTextChanged(Java.Lang.ICharSequence? s, int start, int count, int after)
        {
        }

        public void OnTextChanged(Java.Lang.ICharSequence? s, int start, int before, int count)
        {
            if (Handler?.VirtualView is ISearchBar searchBar)
            {
                var newText = s?.ToString() ?? string.Empty;
                searchBar.UpdateText(newText);

                // Update close button visibility based on whether text exists
                Handler._platformSearchView?.UpdateCloseButtonVisibility(!string.IsNullOrEmpty(newText));
            }
        }
    }

    class FocusChangeListener : Java.Lang.Object, AView.IOnFocusChangeListener
    {
        public SearchBarHandler2? Handler { get; set; }

        public void OnFocusChange(AView? v, bool hasFocus)
        {
            if (Handler?.VirtualView is ISearchBar searchBar)
            {
                searchBar.IsFocused = hasFocus;
            }
        }
    }
}