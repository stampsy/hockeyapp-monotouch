## Introduction

This project implements MonoTouch C# bindings for [HockeyApp SDK](https://github.com/bitstadium/HockeySDK-iOS). Please refer to the original [HockeyApp SDK documentation](https://github.com/bitstadium/HockeySDK-iOS#introduction) to learn what it does and why you might want to use it.

## Integration

You can git clone this project or [download a compiled binary](https://github.com/downloads/stampsy/hockeyapp-monotouch/hockeyapp-monotouch.zip) and reference it like a normal .NET library. 

If you clone the binding, copy `HockeyApp` folder to your solution folder alongside with your project. Then add a reference to the `HockeyApp/HockeyApp.csproj` in Projects tab.

If you downloaded just the binaries, place the `*.dll` wherever you will in your project and then locate it using .NET Assembly tab in Add Reference dialog, as you would normally do.

There is one benefit to using the binaries over the source code. MonoDevelop doesn't play nice with binding projects yet and highlights all the code using them red. Autocompletion is also not available when referencing binding project.

There is no such problem if you reference the compiled `*.dll` itself.
Whichever way you decide to link the binding, there are three things you need to do next.

### Add Resource Bundle

First, copy `HockeySDKResources.bundle` from the binding directory to your **application project** (not the binding project!). Right-click your project in Solution Explorer and check Show All Files in Display Options. Find `HockeySDKResources.bundle` in your project tree, right-click it and click Include in Project. You can uncheck Show All Files now.

### Specify App Version

HockeyApp uses short bundle identifier in its user interface to differentiate between versions and it makes sense to set a different one each time you upload a new build on HockeyApp. To do so, open your `Info.plist` on Advanced tab and add a string value called `CFBundleShortVersionString`. It has no strict format.

### Tell HockeyApp to Get Going

Finally, you need to tell HockeyApp to activate and [give it your HockeyApp Public ID](http://support.hockeyapp.net/kb/how-tos/how-to-find-the-app-identifier). You will probably want to do it in `FinishedLaunching` method inside your `AppDelegate`. You can wrap HockeyApp activation in `#if !DEBUG` directive so it is stripped out of debug builds.

Your code will end up looking similar to this:

    const string HockeyAppId = "0123456789abcdeffedcba09876543210";

    static void EnableCrashReporting()
    {
        var manager = BITHockeyManager.SharedHockeyManager;
        manager.Configure(HockeyAppId, null);
        manager.StartManager();
    }

    public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
    {
    #if !DEBUG
        EnableCrashReporting();
    #endif
        return true;
    }       

For your convenience, **all of these steps have already been done in the sample app** so you can just check it out. Still, don't forget to specify your app key in `AppDelegate`.

## Uploading Builds to HockeyApp

Skip this section if you're familiar with HockeyApp or TestFlight process.

In order to decode your crash reports, you need to actually **upload the exact builds you have deployed to HockeyApp along with the corresponding debug symbols**. If you get a crash report from a build that is not uploaded on HockeyApp, you will not see the crash stack trace.

MonoDevelop will happily generate `*.ipa` and `*.dSYM` that HockeyApp wants if you check “Build ad-hoc/enterprise package (IPA)” in iPhone IPA Options in your Project Settings. Again, `*.dSYM` is actually a directory, so you'll need to zip it before uploading to HockeyApp. This quickly becomes a pain in the ass if you want to upload many builds a day so we wrote a Python script to bump bundle version on each Release build and upload it to HockeyApp with the debug symbols right away.

Unfortunately this script is somewhat buggy and tailored to specific our project so we don't release it yet but if you think it can be useful to you, ping me at dan@stampsy.com and I'll get in touch with you.

## Scope and Contributions

These bindings are very basic and contain just enough code to send crash reports on next launch and suggest app updates. Pull requests with more coverage are welcome.