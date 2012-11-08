using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using HockeyApp;

namespace SampleApp
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window {
			get;
			set;
		}

		// Put your HockeyApp ID here
		const string HockeyAppId = "";

		static void EnableCrashReporting()
		{
			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure(HockeyAppId, null);
			manager.StartManager();
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			EnableCrashReporting();
			return true;
		}		
	}
}

