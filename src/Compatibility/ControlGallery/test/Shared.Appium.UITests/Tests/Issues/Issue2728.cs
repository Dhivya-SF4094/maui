﻿using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace UITests 
{
	public class Issue2728 : IssuesUITest
	{
		const string LabelHome = "Hello Label";

		public Issue2728(TestDevice testDevice) : base(testDevice)
		{
		}

		public override string Issue => "[macOS] Label FontAttributes Italic is not working";

		[Test]
		[Category(UITestCategories.Label)]
		public void Issue2728TestsItalicLabel()
		{
			this.IgnoreIfPlatforms([TestDevice.Android, TestDevice.iOS, TestDevice.Windows]);

			RunningApp.WaitForNoElement(LabelHome);
			RunningApp.Screenshot("Label rendered with italic font");
		}
	}
}