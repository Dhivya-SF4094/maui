using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Xunit;
using WSolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace Microsoft.Maui.DeviceTests;

public partial class DatePickerTests
{
	[Fact]
	[Description("The DatePicker Text and Icon Color should work properly on PointerOver")]
	public async Task DatePickerTextAndIconColorShouldWorkProperlyOnPointerOver()
	{
		SetupBuilder();

		var datePicker = new Controls.DatePicker
		{
			TextColor = Colors.Red
		};
		var expectedValue = datePicker.TextColor;

		var handler = await CreateHandlerAsync<DatePickerHandler>(datePicker);
		var platformView = GetPlatformControl(handler);

		await InvokeOnMainThreadAsync(() =>
		{
			var foregroundPointerOverBrush = platformView.Resources["CalendarDatePickerTextForegroundPointerOver"] as WSolidColorBrush;
			var foregroundPointerOverColor = foregroundPointerOverBrush.Color.ToColor();
			Assert.Equal(expectedValue, foregroundPointerOverColor);

			var glyphForegroundPointerOverBrush = platformView.Resources["CalendarDatePickerCalendarGlyphForegroundPointerOver"] as WSolidColorBrush;
			var glyphForegroundPointerOverColor = glyphForegroundPointerOverBrush.Color.ToColor();
			Assert.Equal(expectedValue, glyphForegroundPointerOverColor);
		});
	}

	
	static CalendarDatePicker GetPlatformControl(DatePickerHandler handler) =>
		handler.PlatformView;

    [Fact(DisplayName = "DateSelected event fires when Platform View date selected")]
    public async Task DateSelectedEventFiresWhenUserSelectsDate()
    {
        SetupBuilder();

        var initialDate = new DateTime(2023, 6, 15);
        var newDate = new DateTime(2023, 9, 10);
        var eventFired = false;
        DateTime? receivedOldDate = null;
        DateTime? receivedNewDate = null;

        var datePicker = new Controls.DatePicker
        {
            Date = initialDate
        };

        datePicker.DateSelected += (sender, args) =>
        {
            eventFired = true;
            receivedOldDate = args.OldDate;
            receivedNewDate = args.NewDate;
        };

        var handler = await CreateHandlerAsync<DatePickerHandler>(datePicker);
        var platformView = GetPlatformControl(handler);

        // Simulate user interaction: open the picker and select a new date
        await InvokeOnMainThreadAsync(() =>
        {
            platformView.Date = new DateTimeOffset(newDate);
        });

        await Task.Delay(100);

        Assert.True(eventFired, "DateSelected event should have fired when user selected a date");
        Assert.Equal(initialDate, receivedOldDate);
        Assert.Equal(newDate, receivedNewDate);
    }
}