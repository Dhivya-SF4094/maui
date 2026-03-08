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
		// Wrap both taps in try-catch: a properly disabled MenuBarItem may not be
		// interactable via Appium and will throw; the crucial assertion is below.
		try { App.Tap("TestMenu"); } catch { }
		try { App.Tap("Perform Action"); } catch { }

		// Verify the action was NOT fired - label must remain "MenuBarDisabled"
		Assert.That(App.FindElement("ResultLabel").GetText(), Is.EqualTo("MenuBarDisabled"),
			"Clicking a disabled MenuBarItem should not fire actions");
	}
}

#endif
