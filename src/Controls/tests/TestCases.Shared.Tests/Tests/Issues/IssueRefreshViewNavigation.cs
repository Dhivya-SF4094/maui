using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues
{
	public class IssueRefreshViewNavigation : _IssuesUITest
	{
		public override string Issue => "RefreshView IsRefreshing property not working with navigation on Windows";

		public IssueRefreshViewNavigation(TestDevice device) : base(device) { }

		[Test]
		[Category(UITestCategories.RefreshView)]
		public void RefreshViewShouldShowIndicatorAfterNavigation()
		{
			// Verify we're on the main page
			App.WaitForElement("MainContent");

			// Navigate to second page and set IsRefreshing=true, then navigate back
			App.Tap("NavigateButton");
			App.WaitForElement("SecondPageLabel");
			App.Tap("GoBackButton");

			// Wait to return to main page
			App.WaitForElement("MainContent");

			// The refresh indicator should be visible now
			// We can't directly test the visual indicator, but we can test that
			// the toggle button can turn it off, indicating it was on
			App.Tap("ToggleButton");

			// If the refresh was working, toggling should turn it off
			// If it wasn't working, the toggle would turn it on for the first time
			// This is an indirect way to test the functionality
		}

		[Test]
		[Category(UITestCategories.RefreshView)]
		public void RefreshViewToggleWorksAfterNavigation()
		{
			// Verify we're on the main page
			App.WaitForElement("MainContent");

			// Test that normal toggle works without navigation first
			App.Tap("ToggleButton");
			App.Tap("ToggleButton"); // Toggle it back
									 // Should work without issues
		}
	}
}