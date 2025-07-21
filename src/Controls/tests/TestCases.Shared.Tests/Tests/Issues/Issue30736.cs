using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;
public class Issue30736 : _IssuesUITest
{
	public Issue30736(TestDevice testDevice) : base(testDevice)
	{
	}

	public override string Issue => "DatePicker's DateSelected event not firing on Windows - .NET 10 Preview 6";

	[Test]
	[Category(UITestCategories.DatePicker)]
	public void DatePicker_DateSelected()
	{
		App.WaitForElement("Issue30736_DatePickerLabel");
		App.Tap("Issue30736_DatePicker");
#if ANDROID
		App.Tap("15");
		App.WaitForElement("OK");
		App.Tap("OK");
#elif WINDOWS
		App.Tap("15");
#endif
		App.WaitForElement("Date Selected event Fired");
	}
}
