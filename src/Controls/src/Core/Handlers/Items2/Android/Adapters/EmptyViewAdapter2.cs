#nullable disable
using System;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Carousel;
using Google.Android.Material.Shape;

namespace Microsoft.Maui.Controls.Handlers.Items2;

/// <summary>
/// A Material3-aware <see cref="Items.EmptyViewAdapter"/> for <see cref="MauiCarouselRecyclerView2"/>.
///
/// Material's <see cref="CarouselLayoutManager"/> requires every direct child of the
/// <see cref="RecyclerView"/> to be a <see cref="MaskableFrameLayout"/>. The shared
/// <see cref="Items.EmptyViewAdapter"/> produces plain Forms-backed view holders
/// (<c>SimpleViewHolder</c> / <c>TemplatedItemViewHolder</c>) whose item view is just
/// a regular Android View, which throws <c>IllegalStateException</c> on the first
/// measure pass.
///
/// This adapter delegates view-holder creation, binding, and recycling to the base
/// implementation, but wraps the inner item view inside a <see cref="MaskableFrameLayout"/>
/// — mirroring what <see cref="CarouselViewAdapter2"/> does for regular items — so the
/// empty-view path satisfies the layout manager's contract without swapping the
/// layout manager.
/// </summary>
internal sealed class EmptyViewAdapter2 : Items.EmptyViewAdapter
{
    readonly Func<bool> _isHorizontal;

    public EmptyViewAdapter2(ItemsView itemsView, Func<bool> isHorizontal)
        : base(itemsView)
    {
        _isHorizontal = isHorizontal;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        // Let the base build the regular empty/header/footer holder. Its ItemView is
        // not yet attached to any parent at this point — RecyclerView attaches only
        // after onCreateViewHolder returns.
        var inner = base.OnCreateViewHolder(parent, viewType);

        var context = parent.Context;
        bool horizontal = _isHorizontal?.Invoke() ?? true;

        var maskable = new MaskableFrameLayout(context)
        {
            LayoutParameters = new RecyclerView.LayoutParams(
                horizontal ? ViewGroup.LayoutParams.WrapContent : ViewGroup.LayoutParams.MatchParent,
                horizontal ? ViewGroup.LayoutParams.MatchParent : ViewGroup.LayoutParams.WrapContent),
        };

        // Apply the Material 3 "Corner Extra Large" shape appearance so the empty-view
        // mask is consistent with regular carousel items.
        using (var value = new global::Android.Util.TypedValue())
        {
            if (context.Theme.ResolveAttribute(Resource.Attribute.shapeAppearanceCornerExtraLarge, value, true)
                && value.ResourceId != 0)
            {
                maskable.ShapeAppearanceModel = ShapeAppearanceModel
                    .InvokeBuilder(context, value.ResourceId, 0)
                    .Build();
            }
        }

        // Re-parent the inner item view into the MaskableFrameLayout. Ensure it fills
        // the maskable so layout/measure flows through unchanged.
        var innerView = inner.ItemView;
        (innerView.Parent as ViewGroup)?.RemoveView(innerView);
        innerView.LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);
        maskable.AddView(innerView);

        return new MaskableEmptyViewHolder(maskable, inner);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        // Forward to the inner holder so the base type-checked binding logic
        // (TemplatedItemViewHolder vs SimpleViewHolder) still runs. The base only
        // reads the `position` argument, so RecyclerView not tracking the inner
        // holder is safe here.
        if (holder is MaskableEmptyViewHolder wrapper)
        {
            base.OnBindViewHolder(wrapper.Inner, position);
            return;
        }

        base.OnBindViewHolder(holder, position);
    }

    public override void OnViewRecycled(Java.Lang.Object holder)
    {
        if (holder is MaskableEmptyViewHolder wrapper)
        {
            base.OnViewRecycled(wrapper.Inner);
            return;
        }

        base.OnViewRecycled(holder);
    }

    /// <summary>
    /// A ViewHolder whose root <see cref="RecyclerView.ViewHolder.ItemView"/> is a
    /// <see cref="MaskableFrameLayout"/> (required by <see cref="CarouselLayoutManager"/>)
    /// and that delegates binding/recycling to an <see cref="Inner"/> holder produced by
    /// the shared <see cref="Items.EmptyViewAdapter"/>.
    /// </summary>
    sealed class MaskableEmptyViewHolder : RecyclerView.ViewHolder
    {
        public RecyclerView.ViewHolder Inner { get; }

        public MaskableEmptyViewHolder(MaskableFrameLayout root, RecyclerView.ViewHolder inner)
            : base(root)
        {
            Inner = inner;
        }
    }
}
