using System;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using MaterialSearchBar = Google.Android.Material.Search.SearchBar;

namespace Microsoft.Maui.Platform;

internal class MauiMaterialSearchBar : MaterialSearchBar
{
    internal TextView? _queryEditor;
    ITextWatcher? _textWatcher;
    const int CloseButtonMenuItemId = 999;
    public event EventHandler? CloseButtonClicked;

    public MauiMaterialSearchBar(Context context) : base(context)
    {
        Initialize();
    }

    void Initialize()
    {
        // Material SearchBar uses TextView internally
        _queryEditor = this.GetFirstChildOfType<TextView>();

        if (_queryEditor is not null)
        {
            // Make the TextView editable and focusable
            _queryEditor.Focusable = true;
            _queryEditor.FocusableInTouchMode = true;
            _queryEditor.Clickable = true;
            _queryEditor.LongClickable = true;

            // Enable text selection and cursor
            _queryEditor.SetTextIsSelectable(true);
            _queryEditor.SetCursorVisible(true);

            // Ensure it can receive input
            _queryEditor.InputType = InputTypes.ClassText | InputTypes.TextVariationNormal;

            // Show keyboard when the TextView receives focus
            _queryEditor.ShowSoftInputOnFocus = true;

            // Ensure text doesn't ellipsize or get cut off
            _queryEditor.Ellipsize = null;
            _queryEditor.SetSingleLine(false);
            _queryEditor.SetHorizontallyScrolling(false);
        }

        // Make the SearchBar itself clickable to pass focus to TextView
        Focusable = true;
        Clickable = true;

        // Add close button to menu
        SetupCloseButton();
    }

    public override bool OnTouchEvent(MotionEvent? e)
    {
        // Forward touch events to the TextView to enable editing only if enabled
        if (e?.Action == MotionEventActions.Down && _queryEditor is not null && _queryEditor.Enabled)
        {
            _queryEditor.RequestFocus();
            // Show the keyboard when the SearchBar is touched
            _queryEditor.PostShowSoftInput();
        }
        return base.OnTouchEvent(e);
    }

    void SetupCloseButton()
    {
        // Add close icon to the menu
        var menu = Menu;
        if (menu is not null)
        {
            var closeItem = menu.Add(0, CloseButtonMenuItemId, 0, "");
            closeItem?.SetIcon(Resource.Drawable.abc_ic_clear_material);
            closeItem?.SetShowAsAction(ShowAsAction.Always);

            // Initially hide the close button
            UpdateCloseButtonVisibility(false);
        }

        // Set menu item click listener
        SetOnMenuItemClickListener(new MenuItemClickListener(this));
    }

    public void UpdateCloseButtonVisibility(bool hasText)
    {
        var menu = Menu;
        var closeItem = menu?.FindItem(CloseButtonMenuItemId);
        closeItem?.SetVisible(hasText);
    }

    public void SetTextChangedListener(ITextWatcher? watcher)
    {
        if (_queryEditor is null)
        {
            return;
        }

        if (_textWatcher is not null)
        {
            _queryEditor.RemoveTextChangedListener(_textWatcher);
        }

        _textWatcher = watcher;

        if (_textWatcher is not null)
        {
            _queryEditor.AddTextChangedListener(_textWatcher);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_textWatcher is not null && _queryEditor is not null)
            {
                _queryEditor.RemoveTextChangedListener(_textWatcher);
            }
            _textWatcher = null;
            CloseButtonClicked = null;
        }

        base.Dispose(disposing);
    }

    class MenuItemClickListener : Java.Lang.Object, IOnMenuItemClickListener
    {
        readonly MauiMaterialSearchBar _searchBar;

        public MenuItemClickListener(MauiMaterialSearchBar searchBar)
        {
            _searchBar = searchBar;
        }

        public bool OnMenuItemClick(IMenuItem? item)
        {
            if (item?.ItemId == CloseButtonMenuItemId)
            {
                // Clear the text
                if (_searchBar._queryEditor is not null)
                {
                    _searchBar._queryEditor.Text = string.Empty;
                }

                _searchBar.CloseButtonClicked?.Invoke(_searchBar, EventArgs.Empty);
                return true;
            }
            return false;
        }
    }
}