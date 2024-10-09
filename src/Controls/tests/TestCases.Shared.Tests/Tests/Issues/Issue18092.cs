#if !MACCATALYST
using NUnit.Framework;
using NUnit.Framework.Legacy;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues
{
    internal class Issue18092 : _IssuesUITest
    {
		public override string Issue => "Entry bug—height grows on every input event";
		public Issue18092(TestDevice testDevice) : base(testDevice)
		{
		}

		[Test]
		[Category(UITestCategories.Entry)]
		public void RoundRectangleHeight()
		{
			var entry = App.WaitForElement("Entry");
			App.EnterText("Entry", "E");
			var initialHeight = entry.GetRect().Height;
			App.EnterText("Entry", "ntry Control");
			var finalHeight = App.WaitForElement("Entry").GetRect().Height;
			Assert.That(initialHeight, Is.EqualTo(finalHeight));
		}
	}
}
#endif
