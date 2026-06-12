#nullable disable
using System;
using System.Collections.Specialized;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Carousel;

namespace Microsoft.Maui.Controls.Handlers.Items2;

/// <summary>
/// A <see cref="Items.MauiCarouselRecyclerView"/> variant that uses the Material Design
/// <see cref="CarouselLayoutManager"/> (with <see cref="FullScreenCarouselStrategy"/>) instead of
/// <see cref="LinearLayoutManager"/>.
///
/// All MAUI CarouselView API surface (Position, CurrentItem, IsSwipeEnabled, IsBounceEnabled,
/// PeekAreaInsets, ItemsLayout) is preserved by inheriting the existing scroll and visual-state
/// machinery from <see cref="Items.MauiCarouselRecyclerView"/>.
///
/// <para>
/// <b>Looping is not supported on this handler.</b> Material's <see cref="CarouselLayoutManager"/>
/// has no concept of a virtual range, so the LoopScale (≈16384) trick used by MAUI's
/// LinearLayoutManager-based implementation does not work. Callers must keep
/// <see cref="CarouselView.Loop"/> set to <c>false</c>; <see cref="CarouselViewAdapter2.ItemCount"/>
/// is locked to <c>ItemsSource.Count</c> to guard the adapter side.
/// </para>
/// </summary>
internal class MauiCarouselRecyclerView2 :
    Items.MauiCarouselRecyclerView,
    IMauiCarouselRecyclerView2
{
    CarouselSnapHelper _carouselSnapHelper;
    bool _disposed;
    Items.ObservableItemsSource _trackedItemsSource;

    public MauiCarouselRecyclerView2(
        Context context,
        Func<IItemsLayout> getItemsLayout,
        Func<Items.ItemsViewAdapter<CarouselView, Items.IItemsViewSource>> getAdapter)
        : base(context, getItemsLayout, getAdapter)
    {
    }

    // Material's CarouselLayoutManager has no virtual-range concept, so the
    // LoopScale (16384) trick used by the legacy LinearLayoutManager path does
    // not apply here. Force every Loop-aware code path in the base class to
    // take the non-loop branch regardless of CarouselView.Loop.
    // TODO: Remove this override once a true looping mechanism is implemented
    // for the Material3 handler (e.g. edge-jump strategy, duplicate-buffer
    // adapter, or a CarouselLayoutManager fork with virtual-range support).
    protected override bool IsLoopEnabled => false;

    // -----------------------------------------------------------------------
    // Layout manager — swap LinearLayoutManager for CarouselLayoutManager
    // -----------------------------------------------------------------------

    protected override LayoutManager SelectLayoutManager(IItemsLayout layoutSpecification)
    {
        var orientation = RecyclerView.Horizontal;

        if (layoutSpecification is LinearItemsLayout linearItemsLayout)
        {
            orientation = linearItemsLayout.Orientation == ItemsLayoutOrientation.Vertical
                ? RecyclerView.Vertical
                : RecyclerView.Horizontal;
        }

        // While the EmptyView is showing, the RecyclerView holds a single non-carousel
        // item. Material's CarouselLayoutManager applies keyline masking sized for
        // full-viewport carousel items; applied to a normally-sized EmptyView the mask
        // collapses on a later layout pass, so the EmptyView appears for one frame and
        // then disappears. Use a plain LinearLayoutManager for the empty state — this
        // mirrors the LinearLayoutManager-based Handler1 path where the EmptyView renders
        // correctly. The empty-view branch in UpdateEmptyViewVisibility swaps in the
        // EmptyViewAdapter before calling SelectLayoutManager, so GetAdapter() reflects
        // the empty state here.
        if (GetAdapter() is Items.EmptyViewAdapter)
        {
            return new LinearLayoutManager(Context, orientation, false);
        }

        return new CarouselLayoutManager(CreateCarouselStrategy(), orientation);
    }

    /// <summary>
    /// Creates the <see cref="CarouselStrategy"/> to use.
    ///
    /// Currently locked to <see cref="FullScreenCarouselStrategy"/>: the other Material
    /// strategies (MultiBrowse, Hero, Uncontained) require items to be smaller than the
    /// viewport, which conflicts with how Handler2 sizes items (full RecyclerView width/
    /// height via <see cref="Items.SizedItemContentView"/>). If a future change wires up
    /// strategy-aware sizing, this can become user-selectable via an attached property.
    /// </summary>
    protected virtual CarouselStrategy CreateCarouselStrategy() => new FullScreenCarouselStrategy();

    // -----------------------------------------------------------------------
    // Snap — replace MAUI snap manager with CarouselSnapHelper
    // -----------------------------------------------------------------------

    protected override void UpdateSnapBehavior()
    {
        // Detach any previous snap helper to avoid duplicate fling listeners.
        _carouselSnapHelper?.AttachToRecyclerView(null);
        _carouselSnapHelper = null;

        // CarouselSnapHelper requires a CarouselLayoutManager. While the EmptyView is
        // showing we use a LinearLayoutManager, so don't attach the snap helper.
        if (GetLayoutManager() is not CarouselLayoutManager)
        {
            return;
        }

        // CarouselLayoutManager ships its own snap helper; attach it directly.
        // Deliberately do NOT call base.UpdateSnapBehavior() so MAUI's SnapManager
        // does not attach a conflicting snap helper.
        _carouselSnapHelper = new CarouselSnapHelper();
        _carouselSnapHelper.AttachToRecyclerView(this);
    }

    public override void UpdateLayoutManager()
    {
        base.UpdateLayoutManager();

        // base.UpdateLayoutManager() early-returns when the ItemsLayout object is
        // unchanged, which is the case on every EmptyView <-> items transition. That
        // leaves the layout manager from the previous state attached. Ensure items always
        // use the Material CarouselLayoutManager and the EmptyView uses a plain
        // LinearLayoutManager.
        var needsCarousel = GetAdapter() is not Items.EmptyViewAdapter;
        var hasCarousel = GetLayoutManager() is CarouselLayoutManager;
        if (needsCarousel != hasCarousel)
        {
            SetLayoutManager(SelectLayoutManager(ItemsLayout));
        }

        // Re-attach the CarouselSnapHelper to the current Material layout manager. The
        // helper is skipped while a LinearLayoutManager is active for the EmptyView.
        UpdateSnapBehavior();
    }

    protected override void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
    {
        // Skip the MAUI snap-manager reset (no SingleSnapHelper attached) and go straight
        // to the underlying scroll so CarouselSnapHelper continues to control snapping.
        ScrollTo(args);
    }

    // -----------------------------------------------------------------------
    // Spacing decoration — CarouselLayoutManager manages item sizes via its
    // strategy, so we use a no-op decoration. PeekAreaInsets are applied as
    // RecyclerView padding by the handler instead.
    // -----------------------------------------------------------------------

    protected override RecyclerView.ItemDecoration CreateSpacingDecoration(IItemsLayout itemsLayout)
        => new NoOpItemDecoration();

    sealed class NoOpItemDecoration : RecyclerView.ItemDecoration { }

    // -----------------------------------------------------------------------
    // Scroll listener — override to use CarouselLayoutManager-aware listener
    // -----------------------------------------------------------------------

    protected override Items.RecyclerViewScrollListener<CarouselView, Items.IItemsViewSource> CreateScrollListener()
        => new CarouselViewOnScrollListener2(Carousel, ItemsViewAdapter, () => _carouselSnapHelper);

    // -----------------------------------------------------------------------
    // Empty-view adapter — the EmptyView is shown with a plain LinearLayoutManager
    // (see SelectLayoutManager), so the empty/header/footer holders do NOT need to be
    // wrapped in a MaskableFrameLayout. Using the shared EmptyViewAdapter directly also
    // avoids the masking that MaskableFrameLayout applies when no CarouselLayoutManager
    // is present to set its mask rect (which would otherwise clip the EmptyView away).
    // -----------------------------------------------------------------------

    protected override Items.EmptyViewAdapter CreateEmptyViewAdapter()
        => new Items.EmptyViewAdapter(ItemsView);

    // -----------------------------------------------------------------------
    // Dispose / teardown
    // -----------------------------------------------------------------------

    public override void UpdateAdapter()
    {
        // base.UpdateAdapter() unsubscribes the previous adapter, builds a new one, and
        // re-subscribes its private CollectionItemsSourceChanged handler. Mirror that
        // sub/unsub here for our parallel handler so we always track the current source.
        UnsubscribeShiftTracking();
        base.UpdateAdapter();
        SubscribeShiftTracking();
    }

    public override void TearDownOldElement(CarouselView oldElement)
    {
        UnsubscribeShiftTracking();
        base.TearDownOldElement(oldElement);
    }

    /// <summary>
    /// Material's <see cref="CarouselLayoutManager"/> does not fire
    /// <c>OnScrolled</c> when items are inserted/removed before the currently centered
    /// item — it simply re-binds the position 0 view holder with the new item. The
    /// Handler1 path relies on that scroll callback to call <c>UpdatePosition</c> so
    /// <see cref="CarouselView.PositionChanged"/> fires when the visible item shifts in
    /// the source. To preserve parity, we subscribe a parallel handler on the
    /// <see cref="Items.ObservableItemsSource.CollectionItemsSourceChanged"/> event that
    /// synchronously writes the shifted index to <c>CarouselView.Position</c> when the
    /// current item's adapter index changed due to an Add/Remove/Move. The base
    /// handler's dispatched callback then settles <c>Position</c> according to
    /// <c>ItemsView.ItemsUpdatingScrollMode</c>, which mirrors the visible
    /// scroll-then-settle behavior of the Handler1 LinearLayoutManager path.
    /// </summary>
    void SubscribeShiftTracking()
    {
        UnsubscribeShiftTracking();
        if (ItemsViewAdapter?.ItemsSource is Items.ObservableItemsSource newSource)
        {
            _trackedItemsSource = newSource;
            newSource.CollectionItemsSourceChanged += OnCollectionItemsSourceChanged;
        }
    }

    void UnsubscribeShiftTracking()
    {
        if (_trackedItemsSource is { } source)
        {
            source.CollectionItemsSourceChanged -= OnCollectionItemsSourceChanged;
            _trackedItemsSource = null;
        }
    }

    void OnCollectionItemsSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Fast-exit for invalid state. Reset replaces everything;
        // CurrentItem semantics don't apply in that case.
        if (Carousel is not CarouselView carousel
            || ItemsViewAdapter?.ItemsSource is not Items.IItemsViewSource source
            || e.Action == NotifyCollectionChangedAction.Reset
            || carousel.CurrentItem is not { } currentItem)
        {
            return;
        }

        var newCurrentItemPosition = source.GetPosition(currentItem);
        if (newCurrentItemPosition < 0)
        {
            // CurrentItem is no longer in the source (e.g. it was removed). Let the
            // base handler's dispatched callback decide where Position should land.
            return;
        }

        var currentPosition = carousel.Position;
        if (newCurrentItemPosition != currentPosition)
        {
            // CRITICAL ordering for Material3 — prevent the insert-time flicker:
            //
            // The adapter has already been notified (NotifyItemInserted ran before this event).
            // Material's CarouselLayoutManager has no predictive-animation path that keeps the
            // previously-focal view anchored on insert, so its next layout pass would render
            // the just-inserted item in the focal slot for one frame (the visible "current item
            // disappears, new item flashes in" flicker on Handler2 but not on
            // Handler1, where LinearLayoutManager naturally keeps the anchor view in place).
            //
            // Calling the native RecyclerView.ScrollToPosition synchronously here sets a
            // pending scroll position that Material's layout pass honors, so the focal slot
            // re-anchors directly on the current item's new index instead of on the new item.
            // The base CollectionItemsSourceChanged handler's dispatched callback runs after
            // this and may move Position again per ItemsUpdatingScrollMode (e.g.
            // KeepItemsInView -> animate-scroll back to position 0, the new item) — that
            // transition is a smooth scroll, not a flicker.
            ScrollToPosition(newCurrentItemPosition);

            // Fire PositionChanged for the shifted current item.
            carousel.SetValueFromRenderer(CarouselView.PositionProperty, newCurrentItemPosition);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            UnsubscribeShiftTracking();
            _carouselSnapHelper?.AttachToRecyclerView(null);
            _carouselSnapHelper = null;
        }

        base.Dispose(disposing);
    }

    // -----------------------------------------------------------------------
    // IMauiCarouselRecyclerView2 — forward to base IMauiCarouselRecyclerView impl
    // -----------------------------------------------------------------------

    void IMauiCarouselRecyclerView2.UpdateFromCurrentItem()
        => ((Items.IMauiCarouselRecyclerView)this).UpdateFromCurrentItem();

    void IMauiCarouselRecyclerView2.UpdateFromPosition()
        => ((Items.IMauiCarouselRecyclerView)this).UpdateFromPosition();

    bool IMauiCarouselRecyclerView2.IsSwipeEnabled
    {
        get => IsSwipeEnabled;
        set => IsSwipeEnabled = value;
    }
}
