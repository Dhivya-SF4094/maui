using System;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Handlers;

internal class MaterialActivityIndicatorHandler : ViewHandler<IActivityIndicator, MaterialActivityIndicator>
{
    public static PropertyMapper<IActivityIndicator, MaterialActivityIndicatorHandler> Mapper =
         new(ViewMapper)
         {
             [nameof(IActivityIndicator.Color)] = MapColor,
             [nameof(IActivityIndicator.IsRunning)] = MapIsRunning,
             [nameof(IActivityIndicator.Visibility)] = MapIsRunning,
         };

    private static void MapIsRunning(MaterialActivityIndicatorHandler handler, IActivityIndicator indicator)
    {
        handler.PlatformView?.UpdateIsRunning(indicator);
    }

    private static void MapColor(MaterialActivityIndicatorHandler handler, IActivityIndicator indicator)
    {
        handler.PlatformView?.UpdateColor(indicator);
    }

    public MaterialActivityIndicatorHandler() : base(Mapper, CommandMapper)
    {
    }

    public static CommandMapper<IActivityIndicator, MaterialActivityIndicatorHandler> CommandMapper =
        new(ViewCommandMapper);

    protected override MaterialActivityIndicator CreatePlatformView()
    {
        return new MaterialActivityIndicator(Context)
        {
            Indeterminate = true
        };
    }

    public override void PlatformArrange(Rect frame)
    {
        if (Context == null || PlatformView == null)
        {
            return;
        }

        // Get the child's desired size (what it measured at)
        var desiredWidth = VirtualView?.DesiredSize.Width ?? frame.Width;
        var desiredHeight = VirtualView?.DesiredSize.Height ?? frame.Height;

        // Constrain to desired size (don't let parent stretch us)
        var constrainedWidth = Math.Min(frame.Width, desiredWidth);
        var constrainedHeight = Math.Min(frame.Height, desiredHeight);

        // Create new frame with constrained size, centered if necessary
        var arrangeFrame = new Rect(
            frame.X + (frame.Width - constrainedWidth) / 2,
            frame.Y + (frame.Height - constrainedHeight) / 2,
            constrainedWidth,
            constrainedHeight);

        base.PlatformArrange(arrangeFrame);
    }
}