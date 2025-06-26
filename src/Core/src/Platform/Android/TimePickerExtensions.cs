namespace Microsoft.Maui.Platform
{
	public static class TimePickerExtensions
	{
		public static void UpdateFormat(this MauiTimePicker mauiTimePicker, ITimePicker timePicker)
		{
			mauiTimePicker.SetTime(timePicker);
		}

		public static void UpdateTime(this MauiTimePicker mauiTimePicker, ITimePicker timePicker)
		{
			mauiTimePicker.SetTime(timePicker);
		}

		internal static void SetTime(this MauiTimePicker mauiTimePicker, ITimePicker timePicker)
		{
			var time = timePicker.Time;
			var format = timePicker.Format;

			mauiTimePicker.Text = time.ToFormattedString(format);
		}
		public static void UpdateTextAlignment(this MauiTimePicker textField, ITimePicker timePicker)
		{
			textField.TextAlignment = timePicker.FlowDirection == FlowDirection.RightToLeft
					? Android.Views.TextAlignment.TextEnd
					: Android.Views.TextAlignment.TextStart;
		}
	}
}