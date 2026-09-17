#if ANDROID
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue38620 : _IssuesUITest
{
	public Issue38620(TestDevice device) : base(device) { }

	public override string Issue => "Material 3 AppBar lift target crashes under GC pressure";

	[Test]
	[Category(UITestCategories.Material3)]
	public void AppBarLiftTargetShouldSurviveCarouselViewChurnUnderGcPressure()
	{
		App.WaitForElement("StressStatus");
		Assert.That(App.WaitForTextToBePresentInElement(
			"StressStatus",
			"Stress completed",
			timeout: TimeSpan.FromSeconds(60)), Is.True);
	}
}
#endif
