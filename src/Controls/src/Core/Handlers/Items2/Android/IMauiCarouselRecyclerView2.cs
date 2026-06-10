namespace Microsoft.Maui.Controls.Handlers.Items2
{
    /// <summary>
    /// Interface for the Material Carousel-backed RecyclerView used by <see cref="CarouselViewHandler2"/> on Android.
    /// Mirrors <see cref="Items.IMauiCarouselRecyclerView"/> so handler map methods can call the same operations.
    /// </summary>
    public interface IMauiCarouselRecyclerView2
    {
        void UpdateFromCurrentItem();

        void UpdateFromPosition();

        /// <summary>
        /// Rebuilds the layout manager, snap helper, scroll-axis padding, and adapter so
        /// changes to <see cref="CarouselView.PeekAreaInsets"/> take effect at runtime.
        /// </summary>
        void UpdatePeekAreaInsets();

        bool IsSwipeEnabled { get; set; }
    }
}
