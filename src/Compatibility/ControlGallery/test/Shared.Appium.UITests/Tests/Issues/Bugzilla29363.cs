﻿using NUnit.Framework;
using UITest.Appium;

namespace UITests
{
	public class Bugzilla29363 : IssuesUITest
	{
		public Bugzilla29363(TestDevice testDevice) : base(testDevice)
		{
		}

		public override string Issue => "PushModal followed immediate by PopModal crashes";

		[Test]
		[Category(UITestCategories.Navigation)]
		public void PushButton()
		{
			RunningApp.Tap("ModalPushPopTest");
			Thread.Sleep(2000);

			// if it didn't crash, yay
			RunningApp.WaitForElement("ModalPushPopTest");
		}
	}
}