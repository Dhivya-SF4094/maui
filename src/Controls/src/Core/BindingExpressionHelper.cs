using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.Maui.Controls
{
	internal static class BindingExpressionHelper
	{
		static readonly Type[] DecimalTypes = { typeof(float), typeof(decimal), typeof(double) };

		internal static bool TryConvert(ref object value, BindableProperty targetProperty, Type convertTo, bool toTarget)
		{
			if (value == null)
				return !convertTo.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(convertTo) != null;
			try
			{
				if ((toTarget && targetProperty.TryConvert(ref value)) || (!toTarget && convertTo.IsInstanceOfType(value)))
					return true;
			}
			catch (InvalidOperationException)
			{ //that's what TypeConverters ususally throw
				return false;
			}

			object original = value;
			try
			{
				convertTo = Nullable.GetUnderlyingType(convertTo) ?? convertTo;

				var stringValue = value as string ?? string.Empty;
				// see: https://bugzilla.xamarin.com/show_bug.cgi?id=32871
				// do not canonicalize "*.[.]"; "1." should not update bound BindableProperty
				if (stringValue.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal) && DecimalTypes.Contains(convertTo))
				{
					value = original;
					return false;
				}

				// Handle partial or non-canonical decimal input
				if (DecimalTypes.Contains(convertTo))
				{
					var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

					// Prevent conversion of values with leading zeros (e.g., "01", "0.01") unless they're valid decimals like "0.x" or "-0.x".
					if (stringValue.Length > 1 &&
						stringValue.StartsWith("0") &&
						stringValue.IndexOf(decimalSeparator, StringComparison.Ordinal) == -1)
					{
						value = original;
						return false;
					}

					// Prevent conversion while typing incomplete negative decimals like "-01"
					// but allow valid decimals like "-01.45" to convert to -1.45.
					if (stringValue.StartsWith("-0") &&
						stringValue.Length > 2 &&
						char.IsDigit(stringValue[2]) &&
						stringValue.IndexOf(decimalSeparator, StringComparison.Ordinal) == -1)
					{
						value = original;
						return false;
					}
				}

				value = Convert.ChangeType(value, convertTo, CultureInfo.CurrentCulture);

				return true;
			}
			catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is InvalidOperationException || ex is OverflowException)
			{
				value = original;
				return false;
			}
		}
	}
}
