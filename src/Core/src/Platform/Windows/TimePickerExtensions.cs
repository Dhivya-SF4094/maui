using System;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WFlowDirection = Microsoft.UI.Xaml.FlowDirection;

namespace Microsoft.Maui.Platform
{
	public static class TimePickerExtensions
	{
		public static void UpdateTime(this TimePicker nativeTimePicker, ITimePicker timePicker)
		{
			nativeTimePicker.Time = timePicker.Time;

			if (timePicker.Format?.Contains('H', StringComparison.Ordinal) == true)
			{
				nativeTimePicker.ClockIdentifier = "24HourClock";
			}
			else
			{
				nativeTimePicker.ClockIdentifier = "12HourClock";
			}
		}

		public static void UpdateCharacterSpacing(this TimePicker platformTimePicker, ITimePicker timePicker)
		{
			platformTimePicker.CharacterSpacing = timePicker.CharacterSpacing.ToEm();
		}

		public static void UpdateFont(this TimePicker platformTimePicker, ITimePicker timePicker, IFontManager fontManager) =>
			platformTimePicker.UpdateFont(timePicker.Font, fontManager);

		public static void UpdateTextColor(this TimePicker platformTimePicker, ITimePicker timePicker)
		{
			Color textColor = timePicker.TextColor;

			UI.Xaml.Media.Brush? platformBrush = textColor?.ToPlatform();

			if (platformBrush == null)
			{
				platformTimePicker.Resources.RemoveKeys(TextColorResourceKeys);
				platformTimePicker.ClearValue(TimePicker.ForegroundProperty);
			}
			else
			{
				platformTimePicker.Resources.SetValueForAllKey(TextColorResourceKeys, platformBrush);
				platformTimePicker.Foreground = platformBrush;
			}

			platformTimePicker.RefreshThemeResources();
		}

        public static void UpdateTextAlignment(this TimePicker platformTimePicker, ITimePicker timePicker)
        {			

			if(platformTimePicker.IsLoaded)
			{
				UpdateFlowDirectionInTimePicker(platformTimePicker, timePicker);
			}
			else
			{
				platformTimePicker.Loaded += (s, e) => UpdateFlowDirectionInTimePicker(platformTimePicker, timePicker);
			}
		}

        static void UpdateFlowDirectionInTimePicker(TimePicker platformTimePicker, ITimePicker timePicker)
        {
            var flowDirection = timePicker.FlowDirection;
            var textAlignment = (flowDirection == FlowDirection.MatchParent || flowDirection == FlowDirection.LeftToRight)
                ? Microsoft.UI.Xaml.TextAlignment.Left
                : Microsoft.UI.Xaml.TextAlignment.Right;

            var hourTextBlock = platformTimePicker.GetDescendantByName<TextBlock>("HourTextBlock");
            if (hourTextBlock is not null)
                hourTextBlock.TextAlignment = textAlignment;

            var minuteTextBlock = platformTimePicker.GetDescendantByName<TextBlock>("MinuteTextBlock");
            if (minuteTextBlock is not null)
                minuteTextBlock.TextAlignment = textAlignment;

            var periodTextBlock = platformTimePicker.GetDescendantByName<TextBlock>("PeriodTextBlock");
            if (periodTextBlock is not null)
                periodTextBlock.TextAlignment = textAlignment;
        }

		// ResourceKeys controlling the foreground color of the TimePicker.
		// https://docs.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.timepicker?view=windows-app-sdk-1.1
		static readonly string[] TextColorResourceKeys =
		{
			"TimePickerButtonForeground",
			"TimePickerButtonForegroundPointerOver",
			"TimePickerButtonForegroundPressed",
			"TimePickerButtonForegroundDisabled"
		};

		// TODO NET8 add to public API
		internal static void UpdateBackground(this TimePicker platformTimePicker, ITimePicker timePicker)
		{
			var brush = timePicker?.Background?.ToPlatform();

			if (brush is null)
			{
				platformTimePicker.Resources.RemoveKeys(BackgroundColorResourceKeys);
				platformTimePicker.ClearValue(TimePicker.BackgroundProperty);
			}
			else
			{
				platformTimePicker.Resources.SetValueForAllKey(BackgroundColorResourceKeys, brush);
				platformTimePicker.Background = brush;
			}

			platformTimePicker.RefreshThemeResources();
		}

		static readonly string[] BackgroundColorResourceKeys =
		{
			"TimePickerButtonBackground",
			"TimePickerButtonBackgroundPointerOver",
			"TimePickerButtonBackgroundPressed",
			"TimePickerButtonBackgroundDisabled",
			"TimePickerButtonBackgroundFocused",
		};
	}
}
