using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace Producer.iOS.UITests
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest ()
		{
			app = ConfigureApp.iOS.StartApp ();
		}

		[Test]
		public void ViewIsDisplayed ()
		{

			var results = app.WaitForElement (c => c.Marked ("Allow"));

			app.Tap (x => x.Marked ("Allow"));

			app.Screenshot ("First screen.");

			Assert.IsTrue (results.Any ());
		}
	}
}

