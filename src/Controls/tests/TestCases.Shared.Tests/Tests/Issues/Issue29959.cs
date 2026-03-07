using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue29959 : _IssuesUITest
{
	public Issue29959(TestDevice testDevice) : base(testDevice)
	{
	}

	public override string Issue => "[Windows] ImageButton Aspect Property Not Working for Various ImageSource Types";

	// The test page has radio buttons to select Aspect (AspectFit, AspectFill, Fill, Center)
	// and Source Type (File, Uri, Stream, FontImage), with a single large ImageButton.
	// Bug: on Windows, changing the Aspect has no visual effect for any ImageSource type.

	[Test]
	[Category(UITestCategories.ImageButton)]
	public void FileImageSourceAspectFillShouldDifferFromAspectFit()
	{
		App.WaitForElement("MyImageButton");

		// File source is selected by default; switch to AspectFill
		App.Tap("ImageAspectFill");

		VerifyScreenshot();
	}

	[Test]
	[Category(UITestCategories.ImageButton)]
	public void FileImageSourceFillShouldDifferFromAspectFit()
	{
		App.WaitForElement("MyImageButton");

		App.Tap("ImageFill");

		VerifyScreenshot();
	}

	[Test]
	[Category(UITestCategories.ImageButton)]
	public void FileImageSourceCenterShouldDifferFromAspectFit()
	{
		App.WaitForElement("MyImageButton");

		App.Tap("ImageCenter");

		VerifyScreenshot();
	}

	[Test]
	[Category(UITestCategories.ImageButton)]
	public void UriImageSourceAspectFillShouldDifferFromAspectFit()
	{
		App.WaitForElement("MyImageButton");

		// Switch to Uri source, then AspectFill
		App.Tap("SourceTypeUri");
		App.WaitForElement("MyImageButton");
		App.Tap("ImageAspectFill");

		VerifyScreenshot(retryTimeout: TimeSpan.FromSeconds(3));
	}

	[Test]
	[Category(UITestCategories.ImageButton)]
	public void StreamImageSourceAspectFillShouldDifferFromAspectFit()
	{
		App.WaitForElement("MyImageButton");

		// Switch to Stream source, then AspectFill
		App.Tap("SourceTypeStream");
		App.WaitForElement("MyImageButton");
		App.Tap("ImageAspectFill");

		VerifyScreenshot();
	}

	[Test]
	[Category(UITestCategories.ImageButton)]
	public void FontImageSourceAspectFillShouldDifferFromAspectFit()
	{
		App.WaitForElement("MyImageButton");

		// Switch to FontImage source, then AspectFill
		App.Tap("SourceTypeFontImage");
		App.WaitForElement("MyImageButton");
		App.Tap("ImageAspectFill");

		VerifyScreenshot();
	}
}
