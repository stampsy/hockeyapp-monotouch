using System;
using System.Drawing;

using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace HockeyApp
{
    [BaseType (typeof (NSObject))]
    public interface BITHockeyManager {
        [Static, Export ("sharedHockeyManager")]
        BITHockeyManager SharedHockeyManager { get; }

        [Export("configureWithIdentifier:delegate:")]
        void Configure(string identifier, [NullAllowed] NSObject del);

        [Export("debugLogEnabled")]
        bool DebugLogEnabled { get; set; }

        [Export("startManager")]
        void StartManager();

		[Export ("crashManager", ArgumentSemantic.Retain)]
		BITCrashManager CrashManager { get; }
    }

	[BaseType (typeof (NSObject))]
	public partial interface BITHockeyBaseManager {
		[Export ("serverURL", ArgumentSemantic.Copy)]
		string ServerURL { get; set; }
	}

	[BaseType (typeof (BITHockeyBaseManager))]
	public partial interface BITCrashManager {
		[Export ("crashManagerStatus")]
		BITCrashManagerStatus CrashManagerStatus { get; set; }
	}

    [BaseType (typeof (NSObject)), Model]
    public interface BITCrashManagerDelegate {
        [Export ("applicationLogForCrashManager:")]
        string GetApplicationLog (NSObject crashManager);
    }

	[BaseType (typeof (NSObject)), Model]
	public interface BITUpdateManagerDelegate {
		[Export ("customDeviceIdentifierForUpdateManager:")]
		string GetCustomDeviceIdentifier (NSObject updateManager);
	}
}

