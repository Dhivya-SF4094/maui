using System;
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue23675 : _IssuesUITest
{
    public Issue23675(TestDevice testDevice) : base(testDevice)
    {
    }
    public override string Issue => "[MAUI] I1_Layout - Horizontal grid for DataTemplate page does not display four-row of grid and the content of step3";

    [Test]
    [Category(UITestCategories.CollectionView)]
    public void VerifyHorizontalGridDisplaysFourRows()
    {
        App.WaitForElement("Issue23675Label");
        VerifyScreenshot();
    }
}
