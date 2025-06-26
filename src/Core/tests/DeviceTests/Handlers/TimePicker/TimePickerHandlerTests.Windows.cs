using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class TimePickerHandlerTests
	{
		TimePicker GetNativeTimePicker(TimePickerHandler timePickerHandler) =>
			timePickerHandler.PlatformView;

		async Task ValidateTime(ITimePicker timePickerStub, Action action = null)
		{
			var actual = await GetValueAsync(timePickerStub, handler =>
			{
				var native = GetNativeTimePicker(handler);
				action?.Invoke();

				return native.Time.ToString();
			});

			var expected = timePickerStub.ToFormattedString();

			bool condition = actual.StartsWith(expected);

			Assert.True(condition);
		}

		FlowDirection GetFlowDirection(TimePickerHandler timePickerHandler)
		{
			var platformTimePicker = GetNativeTimePicker(timePickerHandler);
			return ConvertToFlowDirection(platformTimePicker.FlowDirection);
		}

		FlowDirection ConvertToFlowDirection(Microsoft.UI.Xaml.FlowDirection flowDirection)
		{
			return flowDirection switch
			{
				Microsoft.UI.Xaml.FlowDirection.RightToLeft => FlowDirection.RightToLeft,
				Microsoft.UI.Xaml.FlowDirection.LeftToRight => FlowDirection.LeftToRight,
				_ => FlowDirection.MatchParent
			};
		}
	}
}