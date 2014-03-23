using System;
using System.Runtime.InteropServices;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using HockeyApp;
using MonoTouch.Dialog;

namespace SampleApp
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window {
			get;
			set;
		}
		UINavigationController _nav;
		DialogViewController _rootVc;
		RootElement _rootElement;
		StyledStringElement _button;

		// Put your HockeyApp ID here
		const string HockeyAppId = "";

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			EnableCrashReporting ();

			Window = new UIWindow(UIScreen.MainScreen.Bounds);
			_button = new StyledStringElement("Test");
			_button.Tapped += delegate {
				throw new Exception("test");
			};

			_rootElement = new RootElement("HockeyTest"){new Section(){_button} };
			_rootVc = new DialogViewController(_rootElement);
			_nav = new UINavigationController(_rootVc);
			Window.RootViewController = _rootVc;
			Window.MakeKeyAndVisible();
			return true;
		}

		[DllImport ("libc")]
		private static extern int sigaction (Signal sig, IntPtr act, IntPtr oact);
		
		enum Signal {
			SIGBUS = 10,
			SIGSEGV = 11
		}

		/// <summary>
		/// This method works around a problem with all iOS crash reporters.
		/// 
		/// Because they override signal handlers for SIGSEGV and SIGBUS,
		/// they break null reference exception handling in Mono.
		/// 
		/// We have to re-install Mono signal handlers.
		/// 
		/// Read more about this fix here:
		/// http://stackoverflow.com/a/14499336/458193
		/// </summary>
		static void EnableCrashReporting ()
		{
			IntPtr sigbus = Marshal.AllocHGlobal (512);
			IntPtr sigsegv = Marshal.AllocHGlobal (512);
			
			// Store Mono SIGSEGV and SIGBUS handlers
			sigaction (Signal.SIGBUS, IntPtr.Zero, sigbus);
			sigaction (Signal.SIGSEGV, IntPtr.Zero, sigsegv);
			
			// Enable crash reporting libraries
			EnableCrashReportingUnsafe ();
			
			// Restore Mono SIGSEGV and SIGBUS handlers            
			sigaction (Signal.SIGBUS, sigbus, IntPtr.Zero);
			sigaction (Signal.SIGSEGV, sigsegv, IntPtr.Zero);

			Marshal.FreeHGlobal (sigbus);
			Marshal.FreeHGlobal (sigsegv);
		}
		
		static void EnableCrashReportingUnsafe ()
		{
			// Run your crash reporting library initialization code here--
			// this example uses HockeyApp but it should work well
			// with TestFlight or other libraries.
			
			// Verify in documentation that your library of choice
			// installs its sigaction hooks before leaving this method.

			if (string.IsNullOrWhiteSpace (HockeyAppId)) {
				throw new Exception ("You forgot to fill in your HockeyApp app ID.");
			}

			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure (HockeyAppId, null);

			// Uncomment following line to enable auto-send of crash reports on relaunch of app
			// manager.CrashManager.CrashManagerStatus = BITCrashManagerStatus.AutoSend;

			manager.StartManager ();
		}		
	}
}

