using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue34738 : _IssuesUITest
{
	public Issue34738(TestDevice device) : base(device) { }

	public override string Issue => "TabBarDisabledColor not applied to disabled tabs on Windows";

	[Test]
	[Category(UITestCategories.Shell)]
	public void TabBarDisabledColorAppliedToDisabledTab()
	{
		App.WaitForElement("Tab2");
		VerifyScreenshot("DisabledTabWithGreenColor");

		App.Tap("EnableButton");
		App.WaitForElement("Tab2");
		VerifyScreenshot("EnabledTabWithNormalColor");
	}
}
