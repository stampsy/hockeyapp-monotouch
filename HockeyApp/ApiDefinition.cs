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

        [Export("startManager")]
        void StartManager();
    }
}

