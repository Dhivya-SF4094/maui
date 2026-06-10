#nullable disable
using System;
using System.Diagnostics.CodeAnalysis;
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

        var layoutManager = new CarouselLayoutManager(CreateCarouselStrategy(), orientation);

        // When PeekAreaInsets > 0 we want the focal item centered so BOTH the previous
        // and the next items peek symmetrically (default alignment is START, which only
        // peeks the trailing neighbor). AlignmentCenter is a public, non-obsolete API on
        // CarouselLayoutManager so this is safe.
        if (HasPeekAreaInsets())
        {
            layoutManager.CarouselAlignment = CarouselLayoutManager.AlignmentCenter;
        }

        return layoutManager;
    }

    /// <summary>
    /// Creates the <see cref="CarouselStrategy"/> to use.
    ///
    /// When <see cref="CarouselView.PeekAreaInsets"/> is zero we use
    /// <see cref="FullScreenCarouselStrategy"/> (one-up viewing). When peek insets are
    /// requested we fall back to <c>UncontainedCarouselStrategy</c>, which keeps items
    /// at their measured size and lets neighbors peek — matching the legacy Handler1
    /// behavior. UncontainedCarouselStrategy is marked <c>[Obsolete(error:true)]</c> in
    /// the Xamarin.Google.Android.Material binding (Google flags it "internal API"),
    /// so it must be instantiated via reflection.
    /// </summary>
    protected virtual CarouselStrategy CreateCarouselStrategy()
    {
        if (HasPeekAreaInsets())
        {
            var uncontained = CreateUncontainedCarouselStrategy();
            if (uncontained is not null)
            {
                return uncontained;
            }
        }

        return new FullScreenCarouselStrategy();
    }

    /// <summary>
    /// Reflectively constructs a <c>UncontainedCarouselStrategy</c>. The type is bound
    /// in <c>Xamarin.Google.Android.Material</c> but marked <c>[Obsolete(error:true)]</c>
    /// — neither the type nor its constructor can be referenced directly, and the error
    /// is CS0619 which cannot be suppressed with <c>#pragma warning disable</c>.
    /// </summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor,
        "Google.Android.Material.Carousel.UncontainedCarouselStrategy",
        "Xamarin.Google.Android.Material")]
    static CarouselStrategy CreateUncontainedCarouselStrategy()
    {
        try
        {
            var type = Type.GetType(
                "Google.Android.Material.Carousel.UncontainedCarouselStrategy, Xamarin.Google.Android.Material",
                throwOnError: false);

            if (type is null)
            {
                return null;
            }

            return Activator.CreateInstance(type) as CarouselStrategy;
        }
        catch
        {
            return null;
        }
    }

    bool HasPeekAreaInsets()
    {
        var insets = Carousel?.PeekAreaInsets ?? default;
        return IsHorizontal
            ? (insets.Left > 0 || insets.Right > 0)
            : (insets.Top > 0 || insets.Bottom > 0);
    }

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

        UpdatePeekPadding();
    }

    /// <summary>
    /// Reserves <see cref="CarouselView.PeekAreaInsets"/>-sized padding on the scroll axis
    /// of the RecyclerView and disables <c>clipToPadding</c>. Material's
    /// <c>getLeftOrTopPaddingForKeylineShift</c> shifts the start/end keylines by the
    /// padding amount only when <c>clipToPadding == false</c>, which produces the outer
    /// empty space on the first/last items so they can be centered like the middle
    /// items (otherwise Material pins them flush to the container edge). The middle
    /// keyline state is unaffected. Matches the Windows handler's behavior
    /// (<c>ListViewBase.Padding = PeekAreaInsets</c>).
    /// </summary>
    void UpdatePeekPadding()
    {
        // No padding while the EmptyView is showing or when peek is not requested.
        if (GetLayoutManager() is not CarouselLayoutManager || !HasPeekAreaInsets())
        {
            SetPadding(0, 0, 0, 0);
            SetClipToPadding(true);
            return;
        }

        var ctx = Context;
        if (ctx is null)
        {
            return;
        }

        var insets = Carousel?.PeekAreaInsets ?? default;

        if (IsHorizontal)
        {
            int leftPx = (int)ctx.ToPixels(insets.Left);
            int rightPx = (int)ctx.ToPixels(insets.Right);
            SetPadding(leftPx, 0, rightPx, 0);
        }
        else
        {
            int topPx = (int)ctx.ToPixels(insets.Top);
            int bottomPx = (int)ctx.ToPixels(insets.Bottom);
            SetPadding(0, topPx, 0, bottomPx);
        }

        // ViewGroup.ClipToPadding is not bound as a settable property — must call the
        // explicit setter.
        SetClipToPadding(false);
    }

    /// <summary>
    /// Refresh layout manager + snap helper + adapter when PeekAreaInsets changes at
    /// runtime. Skipped when the EmptyView is active so we don't churn the empty state.
    /// </summary>
    void IMauiCarouselRecyclerView2.UpdatePeekAreaInsets()
    {
        if (GetAdapter() is Items.EmptyViewAdapter)
        {
            return;
        }

        // Rebuild the layout manager so the new strategy + alignment are picked up.
        SetLayoutManager(SelectLayoutManager(ItemsLayout));
        UpdateSnapBehavior();
        UpdatePeekPadding();
        (this as Items.IMauiRecyclerView<CarouselView>)?.UpdateAdapter();
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

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
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
