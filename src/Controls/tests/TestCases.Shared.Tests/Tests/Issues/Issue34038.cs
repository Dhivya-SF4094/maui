#if MACCATALYST || WINDOWS
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

[Category(UITestCategories.IsEnabled)]
public class Issue34038 : _IssuesUITest
{
	public override string Issue => "[macOS] IsEnabled property false not working on MenuBarItem";

	public Issue34038(TestDevice device) : base(device) { }

	[Test]
	public void MenuBarItemIsEnabledFalseDisablesItem()
	{
		// Wait for the page to load
		App.WaitForElement("DisableMenuButton");

		// Disable the MenuBarItem
		App.Tap("DisableMenuButton");
		Assert.That(App.WaitForElement("ResultLabel").GetText(), Is.EqualTo("MenuBarDisabled"),
			"Disable button should have updated the result label");

		// Try to open the disabled MenuBarItem — it should be greyed out/unclickable.
		try
		{ App.Tap("TestMenu"); }
		catch { }
		try
		{ App.Tap("Perform Action"); }
		catch { }

		// Verify the action was NOT fired - label must remain "MenuBarDisabled"
		Assert.That(App.FindElement("ResultLabel").GetText(), Is.EqualTo("MenuBarDisabled"),
			"Clicking a disabled MenuBarItem should not fire actions");
	}

	[Test]
	public void MenuFlyoutItemIsEnabledFalseDisablesChildItem()
	{
		// Enable menubar and disable only the second MenuFlyoutItem
		App.WaitForElement("DisableItemButton");
		App.Tap("DisableItemButton");
		Assert.That(App.WaitForElement("ResultLabel").GetText(), Is.EqualTo("MenuItemDisabled"),
			"Setup button should enable the MenuBarItem and disable the second child item");

		// Open the menu and tap the ENABLED first item — action should fire
		App.Tap("TestMenu");
		App.Tap("Perform Action");
		Assert.That(App.WaitForElement("ResultLabel").GetText(), Is.EqualTo("FirstActionFired"),
			"Tapping an enabled child MenuFlyoutItem when MenuBarItem is enabled should fire its action");

		// Open the menu again and try to tap the DISABLED second item — action should NOT fire
		App.Tap("TestMenu");
		try
		{ App.Tap("Perform Second Action"); }
		catch { }
		Assert.That(App.FindElement("ResultLabel").GetText(), Is.EqualTo("FirstActionFired"),
			"Tapping a disabled child MenuFlyoutItem should not fire its action");
	}
}

#endif
