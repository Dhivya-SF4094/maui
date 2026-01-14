using System;
using Android.Content.Res;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AView = Android.Views.View;

namespace Microsoft.Maui.Handlers;

// TODO: material3 - make it public in .net 11
internal partial class SearchBarHandler2 : ViewHandler<ISearchBar, MauiMaterialSearchBar>
{
    static ColorStateList? DefaultPlaceholderTextColors { get; set; }

    public EditText? QueryEditor => PlatformView?.QueryEditor;

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

    public SearchBarHandler2() : base(Mapper, CommandMapper)
    {
    }

    protected override MauiMaterialSearchBar CreatePlatformView()
    {
        return new MauiMaterialSearchBar(Context);
    }

    protected override void ConnectHandler(MauiMaterialSearchBar platformView)
    {
        if (QueryEditor is not null)
        {
            QueryEditor.TextChanged += OnTextChanged;
            QueryEditor.EditorAction += OnEditorAction;
        }

        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(MauiMaterialSearchBar platformView)
    {
        if (QueryEditor is not null)
        {
            QueryEditor.TextChanged -= OnTextChanged;
            QueryEditor.EditorAction -= OnEditorAction;
        }

        base.DisconnectHandler(platformView);
    }

    void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (VirtualView == null || PlatformView == null)
            return;

        // Update the virtual view text without triggering a mapper update
        VirtualView.UpdateText(QueryEditor?.Text ?? string.Empty);
    }

    void OnEditorAction(object? sender, TextView.EditorActionEventArgs e)
    {
        if (e.ActionId == ImeAction.Search)
        {
            VirtualView?.SearchButtonPressed();
            PlatformView?.CollapseSearch();
            e.Handled = true;
        }
        else
        {
            e.Handled = false;
        }
    }

    public static void MapBackground(ISearchBarHandler handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateBackground(searchBar);
    }

    public static void MapIsEnabled(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateIsEnabled(searchBar);
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
        handler.PlatformView?.UpdatePlaceholderColor(searchBar, DefaultPlaceholderTextColors);
    }

    internal static void MapFlowDirection(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        if (handler.PlatformView is null)
            return;

        if (searchBar.FlowDirection == FlowDirection.MatchParent && searchBar.Parent is IView parentView)
        {
            ((AView)handler.PlatformView).UpdateFlowDirection(parentView);
            handler.QueryEditor?.UpdateFlowDirection(parentView);
        }
        else
        {
            ((AView)handler.PlatformView).UpdateFlowDirection(searchBar);
            handler.QueryEditor?.UpdateFlowDirection(searchBar);
        }
    }

    public static void MapFont(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        var fontManager = handler.GetRequiredService<IFontManager>();
        handler.PlatformView?.UpdateFont(searchBar, fontManager, handler.QueryEditor);
        // handler.QueryEditor?.UpdateFont(searchBar, fontManager);
    }

    public static void MapHorizontalTextAlignment(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView.UpdateHorizontalTextAlignment(searchBar, handler.QueryEditor);
    }

    public static void MapVerticalTextAlignment(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateVerticalTextAlignment(searchBar, handler.QueryEditor);
    }

    public static void MapCharacterSpacing(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateCharacterSpacing(searchBar);
    }

    public static void MapTextColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateTextColor(searchBar);
        handler.PlatformView?.UpdateTextColor(searchBar);
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

    public static void MapIsReadOnly(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.QueryEditor?.UpdateIsReadOnly(searchBar);
    }

    public static void MapCancelButtonColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateCancelButtonColor(searchBar);
    }
    static void MapSearchIconColor(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.UpdateSearchIconColor(searchBar);
    }
    public static void MapKeyboard(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.UpdateValue(nameof(ISearchBar.Text));
        handler.PlatformView?.UpdateKeyboard(searchBar, handler.QueryEditor);
    }

    public static void MapFocus(SearchBarHandler2 handler, ISearchBar searchBar, object? args)
    {
        if (args is FocusRequest request && handler.PlatformView is not null)
        {
            if (request.Result && handler.PlatformView is not null)
            {
                // Expand the SearchView when focusing
                handler.PlatformView.ExpandSearch();
            }

            handler.QueryEditor?.Focus(request);
        }
    }

    public static void MapReturnType(SearchBarHandler2 handler, ISearchBar searchBar)
    {
        handler.PlatformView?.MaterialSearchView?.UpdateReturnType(searchBar);
    }
}