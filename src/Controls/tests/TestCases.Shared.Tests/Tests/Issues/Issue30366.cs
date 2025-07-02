using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue30366 : _IssuesUITest
{
    public Issue30366(TestDevice testDevice) : base(testDevice)
    {
    }

    public override string Issue => "SearchBar CharacterSpacing property is not working as expected";

    [Test]
    [Category(UITestCategories.SearchBar)]
    public void CharacterSpacingShouldApplyForSeachBarPlaceHolderText()
    {
        App.WaitForElement("Issue30366_SearchBar");
        VerifyScreenshot();
    }
}