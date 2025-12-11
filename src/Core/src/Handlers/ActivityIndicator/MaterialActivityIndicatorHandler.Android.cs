using System;

namespace Microsoft.Maui.Handlers;

internal partial class MaterialActivityIndicatorHandler : ViewHandler<IActivityIndicator, MaterialActivityIndicator>
{
    public static PropertyMapper<IActivityIndicator, MaterialActivityIndicatorHandler> Mapper =
                new(ElementMapper)
                {
                    [nameof(IActivityIndicator.IsRunning)] = MapIsRunning,
                    [nameof(IActivityIndicator.Color)] = MapColor,
                    [nameof(IActivityIndicator.Visibility)] = MapIsRunning,
                };

    //IActivityIndicator IActivityIndicatorHandler.VirtualView => VirtualView;

    private static void MapColor(MaterialActivityIndicatorHandler handler, IActivityIndicator indicator)
    {
        handler.PlatformView.UpdateColor(indicator);
    }

    private static void MapIsRunning(MaterialActivityIndicatorHandler handler, IActivityIndicator indicator)
    {
        handler.PlatformView.UpdateIsRunning(indicator);
    }

    public static CommandMapper<IActivityIndicator, MaterialActivityIndicatorHandler> CommandMapper =
            new(ViewCommandMapper);

    public MaterialActivityIndicatorHandler() : base(Mapper, CommandMapper)
    {
    }

    protected override MaterialActivityIndicator CreatePlatformView()
    {
        var indicator = new MaterialActivityIndicator(Context)
        {
            Indeterminate = true
        };

        // Set size to 48dp (Material 3 standard medium size)
        var size = (int)global::Android.Util.TypedValue.ApplyDimension(
            global::Android.Util.ComplexUnitType.Dip,
            30,
            Context.Resources?.DisplayMetrics);

        indicator.IndicatorSize = 12;

        // Set track thickness to 4dp (Material 3 standard)
        var thickness = (int)global::Android.Util.TypedValue.ApplyDimension(
            global::Android.Util.ComplexUnitType.Dip,
            1.6f,
            Context.Resources?.DisplayMetrics);

        indicator.TrackThickness = thickness;

        return indicator;
    }

    protected override void ConnectHandler(MaterialActivityIndicator platformView)
    {
        //base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(MaterialActivityIndicator platformView)
    {
        //base.DisconnectHandler(platformView);
    }

}
