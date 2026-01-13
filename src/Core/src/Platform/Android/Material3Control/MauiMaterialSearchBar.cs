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
internal class MauiMaterialSearchBar : MaterialSearchBar
{
    MaterialSearchView? _searchView;
    internal EditText? _queryEditor;
    bool _isExpanded;
    bool _isAttachedToRoot;

    public MaterialSearchView? MaterialSearchView => _searchView;
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

        // Create the Material SearchView (expanded state overlay)
        _searchView = new MaterialSearchView(context);

        // Set up the anchor relationship between this SearchBar and SearchView
        // This tells SearchView where to animate from/to
        _searchView.SetupWithSearchBar(this);

        // Extract the EditText from SearchView for direct text manipulation
        _queryEditor = _searchView.GetFirstChildOfType<EditText>();

        // Listen to SearchView transitions to track expanded/collapsed state
        _searchView.AddTransitionListener(new SearchViewTransitionListener(this));

        // Handle SearchBar click to expand SearchView
        Click += OnSearchBarClicked;

        _isExpanded = false;
        _isAttachedToRoot = false;
    }

    void OnSearchBarClicked(object? sender, EventArgs e)
    {
        ExpandSearch();
    }

    void AttachSearchViewToRoot()
    {
        if (_isAttachedToRoot || _searchView is null)
            return;

        var context = Context;
        if (context is null)
            return;

        var activity = context.GetActivity();
        if (activity is null)
            return;

        var root = activity.FindViewById<ViewGroup>(global::Android.Resource.Id.Content);
        if (root is null)
            return;

        // Check if SearchView is already in the hierarchy
        if (_searchView.Parent is ViewGroup parent)
        {
            parent.RemoveView(_searchView);
        }

        // Add SearchView to Activity root with full-screen layout
        var layoutParams = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);

        root.AddView(_searchView, layoutParams);
        _isAttachedToRoot = true;
    }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        // Attach SearchView to Activity root once we're in the window
        AttachSearchViewToRoot();
    }

    protected override void OnDetachedFromWindow()
    {
        base.OnDetachedFromWindow();

        // Clean up SearchView from Activity root
        if (_searchView?.Parent is ViewGroup parent)
        {
            parent.RemoveView(_searchView);
            _isAttachedToRoot = false;
        }
    }

    public void ExpandSearch()
    {
        if (_isExpanded || _searchView is null)
            return;

        // Ensure SearchView is attached to Activity root
        if (!_isAttachedToRoot)
            AttachSearchViewToRoot();

        // Material SearchView will animate and overlay the entire screen
        _searchView.Show();
        _isExpanded = true;
    }

    public void CollapseSearch()
    {
        if (!_isExpanded || _searchView is null)
            return;

        // Material SearchView will animate away
        _searchView.Hide();
        _isExpanded = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Click -= OnSearchBarClicked;

            if (_searchView is not null)
            {
                if (_searchView.Parent is ViewGroup parent)
                {
                    parent.RemoveView(_searchView);
                }
                _searchView.Dispose();
                _searchView = null;
            }
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
            // Material 3 automatically handles EditText focus and keyboard display
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