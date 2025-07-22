using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;
using Xunit;

namespace Microsoft.Maui.DeviceTests;

[Category(TestCategory.DatePicker)]
public partial class DatePickerTests : ControlsHandlerTestBase
{
	void SetupBuilder()
	{
		EnsureHandlerCreated(builder =>
		{
			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<DatePicker, DatePickerHandler>();
			});
		});
	}

	[Fact(DisplayName = "DateSelected Event Fires When Platform View Date Changes")]
	public async Task DateSelectedEventFiresWhenPlatformViewDateChanges()
	{
		SetupBuilder();

		var originalDate = new DateTime(2023, 5, 15);
		var newDate = new DateTime(2023, 8, 20);

		var datePicker = new DatePicker
		{
			Date = originalDate
		};

		bool eventFired = false;
		DateTime eventOldDate = DateTime.MinValue;
		DateTime eventNewDate = DateTime.MinValue;

		datePicker.DateSelected += (sender, e) =>
		{
			eventFired = true;
			eventOldDate = e.OldDate ?? DateTime.MinValue;
			eventNewDate = e.NewDate ?? DateTime.MinValue;
		};

		await CreateHandlerAndAddToWindow<DatePickerHandler>(datePicker, async (handler) =>
		{
			await InvokeOnMainThreadAsync(() =>
			{
#if ANDROID
				// On Android, simulate DatePickerDialog callback
				if (handler.DatePickerDialog != null)
				{
					// Simulate the user selecting a date in the DatePickerDialog
					// This mimics the callback in CreateDatePickerDialog: VirtualView.Date = e.Date
					var previousDate = handler.VirtualView.Date;
					handler.VirtualView.Date = newDate;
				}
#elif IOS
                // On iOS, simulate platform date picker value change
                if (handler.DatePickerDialog != null)
                {
                    // Set the platform picker date first
                    handler.DatePickerDialog.SetDate(newDate.ToNSDate(), false);
                    // Then call SetVirtualViewDate which mimics OnValueChanged/OnDoneClicked
                    typeof(DatePickerHandler).GetMethod("SetVirtualViewDate",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                        .Invoke(handler, null);
                }
#elif WINDOWS
                // On Windows, simulate CalendarDatePicker date change
                if (handler.PlatformView != null)
                {
                    // This mimics the DateChanged event handler: VirtualView.Date = args.NewDate.Value.DateTime
                    handler.PlatformView.Date = newDate;
                    // The DateChanged event should automatically fire and update VirtualView.Date
                }
#else
                // Fallback for other platforms - directly set VirtualView.Date
                handler.VirtualView.Date = newDate;
#endif
			});

			// Give some time for the event to propagate
			await Task.Delay(100);

			Assert.True(eventFired, "DateSelected event should fire when platform view date changes");
			Assert.Equal(originalDate, eventOldDate);
			Assert.Equal(newDate, eventNewDate);
		});
	}
}