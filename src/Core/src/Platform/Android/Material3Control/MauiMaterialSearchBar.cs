using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MaterialSearchBar = Google.Android.Material.Search.SearchBar;
using MaterialSearchView = Google.Android.Material.Search.SearchView;

namespace Microsoft.Maui.Platform;

// TODO: material3 - make it public in .net 11
internal class MauiMaterialSearchBar : MaterialSearchView
{
    MaterialSearchBar? _anchorSearchBar;
    internal EditText? _queryEditor;
    bool _isExpanded;

    public EditText? QueryEditor => _queryEditor;
    public bool IsExpanded => _isExpanded;

    public MauiMaterialSearchBar(Context context) : base(MauiMaterialContextThemeWrapper.Create(context))
    {
        Initialize();
    }

    public MauiMaterialSearchBar(Context context, IAttributeSet? attrs) : base(context, attrs)
    {
        Initialize();
    }

    public MauiMaterialSearchBar(Context context, IAttributeSet? attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
        Initialize();
    }

    protected MauiMaterialSearchBar(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    void Initialize()
    {
        var context = Context;
        if (context is null)
            return;

        // Create a hidden anchor SearchBar for Material 3 SearchView
        // SearchView requires an anchor bar for proper Material Design behavior
        _anchorSearchBar = new MaterialSearchBar(context);
        _anchorSearchBar.Visibility = ViewStates.Gone; // Hide the anchor bar
        _anchorSearchBar.Hint = "Search";

        // Setup SearchView with the anchor SearchBar
        // This establishes the relationship between SearchView and SearchBar
        this.SetupWithSearchBar(_anchorSearchBar);

        // Extract the EditText from SearchView for direct text access
        _queryEditor = this.GetFirstChildOfType<EditText>();

        if (_queryEditor is not null)
        {
            _queryEditor.Hint = "Search";
        }

        // Listen to SearchView state transitions
        this.AddTransitionListener(new SearchViewTransitionListener(this));

        // Configure SearchView to be visible and interactive
        this.Visibility = ViewStates.Visible;
        this.Clickable = true;
        this.Focusable = true;

        _isExpanded = false;
    }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        // Add the hidden anchor SearchBar to the parent if not already added
        if (_anchorSearchBar is not null && _anchorSearchBar.Parent is null)
        {
            if (Parent is ViewGroup parentView)
            {
                var layoutParams = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);

                parentView.AddView(_anchorSearchBar, 0, layoutParams);
            }
        }

        // Show the SearchView directly
        Post(() =>
        {
            this.Show();
            _isExpanded = true;
        });
    }

    protected override void OnDetachedFromWindow()
    {
        base.OnDetachedFromWindow();

        // Remove anchor SearchBar from parent
        if (_anchorSearchBar?.Parent is ViewGroup parent)
        {
            parent.RemoveView(_anchorSearchBar);
        }
    }

    public void ExpandSearch()
    {
        if (_isExpanded)
            return;

        // Show this SearchView
        this.Show();
        _isExpanded = true;
    }

    public void CollapseSearch()
    {
        if (!_isExpanded)
            return;

        // Hide this SearchView
        this.Hide();
        _isExpanded = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_anchorSearchBar is not null)
            {
                if (_anchorSearchBar.Parent is ViewGroup parent)
                {
                    parent.RemoveView(_anchorSearchBar);
                }
                _anchorSearchBar.Dispose();
                _anchorSearchBar = null;
            }

            _queryEditor = null;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Listener for SearchView state transitions to track expanded/collapsed state.
    /// Material 3 SearchView handles focus and keyboard management automatically.
    /// </summary>
    class SearchViewTransitionListener : Java.Lang.Object, MaterialSearchView.ITransitionListener
    {
        readonly MauiMaterialSearchBar _materialSearchBar;

        public SearchViewTransitionListener(MauiMaterialSearchBar materialSearchBar)
        {
            _materialSearchBar = materialSearchBar;
        }

        public void OnStateChanged(MaterialSearchView searchView, MaterialSearchView.TransitionState previousState, MaterialSearchView.TransitionState newState)
        {
            // Update expanded state based on SearchView transitions
            if (newState == MaterialSearchView.TransitionState.Showing || newState == MaterialSearchView.TransitionState.Shown)
            {
                _materialSearchBar._isExpanded = true;
            }
            else if (newState == MaterialSearchView.TransitionState.Hidden || newState == MaterialSearchView.TransitionState.Hiding)
            {
                _materialSearchBar._isExpanded = false;
            }
        }
    }
}