using System;
using Android.Content.Res;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Google.Android.Material.TextField;
using AView = Android.Views.View;

namespace Microsoft.Maui.Handlers;

// TODO: material3 - make it public in .net 11
internal partial class SearchBarHandler2 : ViewHandler<ISearchBar, MauiMaterialTextInputLayout>
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

    TextInputEditText? QueryEditor => PlatformView?.GetFirstChildOfType<TextInputEditText>();
    SearchBarTextWatcher? _textWatcher;

    public SearchBarHandler2() : base(Mapper, CommandMapper)
    {
    }

    protected override MauiMaterialTextInputLayout CreatePlatformView()
    {
        var layout = new MauiMaterialTextInputLayout(Context);
        layout.BoxBackgroundMode = TextInputLayout.BoxBackgroundFilled;

        layout.AddView(new MauiMaterialTextInputEditText(layout.Context!));
        return layout;
    }

    protected override void ConnectHandler(MauiMaterialTextInputLayout platformView)
    {
        base.ConnectHandler(platformView);

        if (QueryEditor is TextInputEditText editText)
        {
            _textWatcher = new SearchBarTextWatcher(platformView)
            {
                Handler = this
            };
            editText.AddTextChangedListener(_textWatcher);
        }
    }

    protected override void DisconnectHandler(MauiMaterialTextInputLayout platformView)
    {
        if (QueryEditor is TextInputEditText editText && _textWatcher is not null)
        {
            editText.RemoveTextChangedListener(_textWatcher);
            _textWatcher.Dispose();
            _textWatcher = null;
        }

        base.DisconnectHandler(platformView);
    }

    public static void MapBackground(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateBackground(searchBar);
    }

    public static void MapCharacterSpacing(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateCharacterSpacing(searchBar);
    }

    public static void MapFont(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        var fontManager = handler.GetRequiredService<IFontManager>();
        handler.QueryEditor?.UpdateFont(searchBar, fontManager);
    }

    public static void MapHorizontalTextAlignment(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateHorizontalTextAlignment(searchBar);
    }

    public static void MapVerticalTextAlignment(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView.UpdateVerticalTextAlignment(searchBar, handler.QueryEditor);
    }

    public static void MapIsReadOnly(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateIsReadOnly(searchBar);
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
        handler.PlatformView?.UpdateMaxLength(searchBar, handler.QueryEditor);
    }

    public static void MapPlaceholder(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdatePlaceholder(searchBar);
    }

    public static void MapPlaceholderColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdatePlaceholderColor(searchBar);
    }

    public static void MapText(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateText(searchBar);
    }

    public static void MapTextColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateTextColor(searchBar);
        //  handler.PlatformView?.UpdateTextColor(searchBar);
    }

    public static void MapCancelButtonColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateCancelButtonColor(searchBar);
    }

    public static void MapSearchIconColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateSearchIconColor(searchBar);
    }

    public static void MapKeyboard(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.UpdateValue(nameof(ISearchBar.Text));

        handler.PlatformView?.UpdateKeyboard(searchBar);
    }

    public static void MapReturnType(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateReturnType(searchBar);
    }

    public static void MapFlowDirection(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        if (searchBar.FlowDirection == FlowDirection.MatchParent && searchBar.Parent != null && searchBar.Parent is IView parentView)
        {
            // When FlowDirection is MatchParent, respect the parent's FlowDirection
            if (handler.PlatformView is AView platformView)
                Microsoft.Maui.Platform.ViewExtensions.UpdateFlowDirection(platformView, parentView);

            if (handler.QueryEditor is TextView textView)
                Microsoft.Maui.Platform.TextViewExtensions.UpdateFlowDirection(textView, parentView);
        }
        else
        {
            // Otherwise, use the SearchBar's own FlowDirection
            handler.PlatformView?.UpdateFlowDirection(searchBar);
            handler.QueryEditor?.UpdateFlowDirection(searchBar);
        }
    }

    public static void MapIsEnabled(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView.UpdateIsEnabled(searchBar, handler.QueryEditor);
    }

    public static void MapFocus(SearchBarHandler2 handler, ISearchBar searchBar, object? args)
    {
        if (args is FocusRequest request)
        {
            handler.QueryEditor?.Focus(request);
        }
    }
}
class SearchBarTextWatcher : Java.Lang.Object, ITextWatcher
{
    readonly MauiMaterialTextInputLayout _layout;

    public SearchBarHandler2? Handler { get; set; }

    public SearchBarTextWatcher(MauiMaterialTextInputLayout layout)
    {
        _layout = layout;
    }

    public void AfterTextChanged(IEditable? s)
    {
        // Update close button visibility based on whether text exists
        _layout.UpdateCloseButtonVisibility(!string.IsNullOrEmpty(s?.ToString()));
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
        }
    }
}
