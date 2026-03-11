using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue34402 : _IssuesUITest
{
	public Issue34402(TestDevice device) : base(device) { }

	public override string Issue => "FlowDirection property not working on BoxView Control";

	[Test]
	[Category(UITestCategories.BoxView)]
	public void BoxViewAndGraphicsViewFlowDirectionShouldMirrorOnRTL()
	{
		App.WaitForElement("MyBoxView");
		App.WaitForElement("MyGraphicsView");
		VerifyScreenshot("LTR_Initial");

		App.Tap("RtlCheckBox");
		VerifyScreenshot("RTL_AfterCheckBox");
	}
}
