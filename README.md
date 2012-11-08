## Introduction

This project implements MonoTouch C# bindings for [HockeyApp SDK](https://github.com/bitstadium/HockeySDK-iOS). Please refer to the original [HockeyApp SDK documentation](https://github.com/bitstadium/HockeySDK-iOS#introduction) to learn what it does and why you might want to use it.

## Integration

You can git clone this project or download a compiled binary and reference it like a normal .NET library.

Things to do next:

### Add Resource Bundle

Then copy `HockeyApp/HockeySDKResources.bundle` to your **application project** (not the binding project!). Right-click your project in Solution Explorer and check Show All Files in Display Options. Then find `HockeySDKResources.bundle` in your project tree, right-click it and click Include in Project. After that you can uncheck Show All Files.

### Specify app version

HockeyApp uses short bundle identifier in its user interface to differentiate between versions, and it makes sense to set a different one each time you upload a new build on HockeyApp. You can set it by opening your `Info.plist` on Advanced tab and adding a string value with key `CFBundleShortVersionString`.

### Tell HockeyApp to initialize

Finally, you need to tell HockeyApp to activate and provide an API key. You will probably want to do it in your `AppDelegate`'s  `FinishedLaunching` method. It also makes sense to wrap HockeyApp activation in `#if not DEBUG` directive so it is stripped out of debug builds.

Here's how it looks:

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

For your convenience, **all of these steps have already been done in the sample app** so you can just check it out. Don't forget to paste in your API key in `AppDelegate` though.

## Uploading builds to HockeyApp

Skip this section if you're familiar with HockeyApp or TestFlight process.

Just something we struggled with: in order to decode your crash reports, you need to actually **upload the exact builds you have deployed to HockeyApp along with the corresponding debug symbols**. If you get a crash report from a build that is not uploaded on HockeyApp, you will not see the stack trace.

MonoDevelop will happily generate *.ipa and *.dSYM that HockeyApp wants if you check “Build ad-hoc/enterprise package (IPA)” in iPhone IPA Options in your Project Settings. Again, `*.dSYM` is actually a directory, so you'll need to zip it before uploading to HockeyApp. This quickly becomes a PITA if you want to upload many builds a day so we hacked up a Python script that will bump our bundle version after each Release build and upload it to HockeyApp with the debug symbols. This script is somewhat buggy yet so we don't release it yet but if you think it can be useful to you, ping me at dan@stampsy.com and we'll get in touch.

## Scope and Contributions

These bindings are very basic and contain just enough code to send crash reports on next launch and suggest app updates. Pull requests with more functionality are welcome.
