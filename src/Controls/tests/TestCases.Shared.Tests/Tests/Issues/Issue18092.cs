using NUnit.Framework;
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
			var initialHeight = App.WaitForElement("RoundRectangle").GetRect().Height;
			App.EnterText("Entry", "E");
			App.EnterText("Entry", "n");
			App.EnterText("Entry", "t");
			App.EnterText("Entry", "r");
			App.EnterText("Entry", "y");
			var finalHeight = App.WaitForElement("RoundRectangle").GetRect().Height;
			Assert.Equals(initialHeight, finalHeight);
		}
	}
}
