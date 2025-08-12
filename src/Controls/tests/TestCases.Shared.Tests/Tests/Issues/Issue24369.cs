using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues
{
    public class Issue24369 : _IssuesUITest
    {
        public Issue24369(TestDevice testDevice) : base(testDevice)
        {
        }

        public override string Issue => "Picker's SelectedItem in CollectionView DataTemplate not working";

        [Test]
        [Category(UITestCategories.Picker)]
        [Category(UITestCategories.CollectionView)]
        public void PickerSelectedItemWorksInCollectionViewDataTemplate()
        {
            App.WaitForElement("Issue24369Label");
            VerifyScreenshot();
        }
    }
}
