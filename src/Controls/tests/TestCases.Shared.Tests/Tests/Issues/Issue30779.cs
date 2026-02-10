using System;
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue30779 : _IssuesUITest
{
    public Issue30779(TestDevice testDevice) : base(testDevice)
    {
    }
    public override string Issue => "SearchBar - CursorPosition and SelectionLength are not updated when the user types";

    [Test]
    [Category(UITestCategories.SearchBar)]
    public void EnsureCursorPositionAndSelectionLengthAreUpdated()
    {
        // Wait for SearchBar to be ready
        App.WaitForElement("VerifyButton");

        // Initial state verification
        App.Tap("VerifyButton");
        var initialResult = App.FindElement("ResultLabel").GetText();
        Assert.That(initialResult, Does.Contain("Cursor="));

        // Test setting cursor position programmatically
        App.Tap("SetCursorButton");
        var cursorResult = App.FindElement("ResultLabel").GetText();
        Assert.That(cursorResult, Does.Contain("Set CursorPosition to 5, actual: 5"));

        // Verify the label reflects the cursor position
        var cursorLabel = App.FindElement("CursorPositionLabel").GetText();
        Assert.That(cursorLabel, Does.Contain("CursorPosition: 5"));

        // Test setting selection programmatically
        App.Tap("SetSelectionButton");
        var selectionResult = App.FindElement("ResultLabel").GetText();
        Assert.That(selectionResult, Does.Contain("Cursor=2, Length=3"));

        // Verify labels reflect the selection
        var cursorLabelAfterSelection = App.FindElement("CursorPositionLabel").GetText();
        var selectionLabel = App.FindElement("SelectionLengthLabel").GetText();
        Assert.That(cursorLabelAfterSelection, Does.Contain("CursorPosition: 2"));
        Assert.That(selectionLabel, Does.Contain("SelectionLength: 3"));

        // Test clearing selection
        App.Tap("ClearSelectionButton");
        var clearResult = App.FindElement("ResultLabel").GetText();
        Assert.That(clearResult, Does.Contain("Length=0"));

        var selectionLabelAfterClear = App.FindElement("SelectionLengthLabel").GetText();
        Assert.That(selectionLabelAfterClear, Does.Contain("SelectionLength: 0"));
    }
}
