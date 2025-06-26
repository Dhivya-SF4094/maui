using System;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml.Controls;

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

		public static void UpdateFlowDirection(this TimePicker platformTimePicker, ITimePicker timePicker)
		{
			// Set FlowDirection on the TimePicker itself following the same pattern as UpdateCharacterSpacing
			var flowDirection = timePicker.FlowDirection;
			switch (flowDirection)
			{
				case FlowDirection.MatchParent:
					platformTimePicker.ClearValue(UI.Xaml.FrameworkElement.FlowDirectionProperty);
					break;
				case FlowDirection.LeftToRight:
					platformTimePicker.FlowDirection = UI.Xaml.FlowDirection.LeftToRight;
					break;
				case FlowDirection.RightToLeft:
					platformTimePicker.FlowDirection = UI.Xaml.FlowDirection.RightToLeft;
					break;
			}

			if (platformTimePicker.IsLoaded)
			{
				UpdateFlowDirectionInTimePicker(platformTimePicker, timePicker);
			}
			else
			{
				platformTimePicker.OnLoaded(() =>
				{
					UpdateFlowDirectionInTimePicker(platformTimePicker, timePicker);
				});
			}
		}

		static void UpdateFlowDirectionInTimePicker(this TimePicker platformTimePicker, ITimePicker timePicker)
		{
			var hourTextBlock = platformTimePicker.GetDescendantByName<TextBlock>("HourTextBlock");
			var minuteTextBlock = platformTimePicker.GetDescendantByName<TextBlock>("MinuteTextBlock");
			var periodTextBlock = platformTimePicker.GetDescendantByName<TextBlock>("PeriodTextBlock");

			var isRtl = timePicker.FlowDirection == FlowDirection.RightToLeft;
			var textAlignment = isRtl ? UI.Xaml.TextAlignment.Right : UI.Xaml.TextAlignment.Left;

			if (hourTextBlock is not null)
			{
				if (timePicker.FlowDirection == FlowDirection.MatchParent)
					hourTextBlock.ClearValue(TextBlock.TextAlignmentProperty);
				else
					hourTextBlock.TextAlignment = textAlignment;
			}
			if (minuteTextBlock is not null)
			{
				if (timePicker.FlowDirection == FlowDirection.MatchParent)
					minuteTextBlock.ClearValue(TextBlock.TextAlignmentProperty);
				else
					minuteTextBlock.TextAlignment = textAlignment;
			}
			if (periodTextBlock is not null)
			{
				if (timePicker.FlowDirection == FlowDirection.MatchParent)
					periodTextBlock.ClearValue(TextBlock.TextAlignmentProperty);
				else
					periodTextBlock.TextAlignment = textAlignment;
			}
		}
	}
}
