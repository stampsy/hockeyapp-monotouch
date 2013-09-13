## Introduction

This project implements MonoTouch C# bindings for [HockeyApp SDK](https://github.com/bitstadium/HockeySDK-iOS). Please refer to the original [HockeyApp SDK documentation](https://github.com/bitstadium/HockeySDK-iOS#introduction) to learn what it does and why you might want to use it.

The specific version of HockeySDK we're linking against is 3.0 ([source](https://github.com/bitstadium/HockeySDK-iOS/tree/3.0.0), [binaries](http://download.hockeyapp.net/sdk/ios/HockeySDK-iOS-3.0.0.zip)).  
You don't need to download them but the source is useful to find method signatures for bindings.

## Integration

You can git clone this project or [download a compiled binary](https://github.com/stampsy/hockeyapp-monotouch/releases/download/v3.0/HockeyApp.dll) and reference it like a normal .NET library.

You should probably want to use the binary over the source code: Xamarin Studio autocompletion doesn't recognize classes from binding projects and this gets in the way. Your project will also build faster if you reference the binary.

There are **four important things** you need to do next, or they will hunt you down and bite you in the arse later.

------

For your convenience, **all of these steps have already been done in the sample app** so you can just check it out. Still, don't forget to specify your app key in `AppDelegate`.

Your `AppDelegate` should end up looking like [this](https://github.com/stampsy/hockeyapp-monotouch/blob/master/SampleApp/AppDelegate.cs) when you're done.

----

### Specify App Version

HockeyApp uses short bundle identifier in its user interface to differentiate between versions and it makes sense to set a different one each time you upload a new build on HockeyApp. To do so, open your `Info.plist` on Advanced tab and add a string value called `CFBundleShortVersionString`. It has no strict format.

### Find You HockeyApp App ID

[There's a website for that.](http://support.hockeyapp.net/kb/how-tos/how-to-find-the-app-identifier)

### Initialize HockeyApp in AppDelegate

You need to call a few methods to configure HockeyApp. You will probably want to do it in `FinishedLaunching` method [inside your `AppDelegate`](https://github.com/stampsy/hockeyapp-monotouch/blob/master/src/SampleApp/AppDelegate.cs). You can wrap HockeyApp activation in `#if !DEBUG` directive so it is stripped out of debug builds. Don't forget to **pass your app ID** to `Configure` method.

### Add Null Reference Exception Fix

**This piece is super duper important. Your app will crash if you don't do this.**

Any iOS crash reporting library overrides signal handlers to catch crashes. Unfortunately Mono needs to handle `SIGSEGV` and `SIGBUS` itself, or any null reference exception will crash your app, whether it was handled or not.

Fortunately, [adding some extra code to AppDelegate fixes the problem](http://stackoverflow.com/a/14499336/458193).

## Uploading Builds to HockeyApp

Skip this section if you're familiar with HockeyApp or TestFlight process.

In order to decode your crash reports, you need to actually **upload the exact builds you have deployed to HockeyApp along with the corresponding debug symbols**. If you get a crash report from a build that is not uploaded on HockeyApp, you will not see the crash stack trace.

MonoDevelop will happily generate `*.ipa` and `*.dSYM` that HockeyApp wants if you check “Build ad-hoc/enterprise package (IPA)” in iPhone IPA Options in your Project Settings. Again, `*.dSYM` is actually a directory, so you'll need to zip it before uploading to HockeyApp.

## Where's My Managed Stack Trace?

The stack traces you will receive will be close to managed code, but not quite. They may look somewhat like this:

```
Thread 0 Crashed:
1 libsystem_kernel.dylib _pthread_kill + 8
2 libsystem_c.dylib _abort + 95
3 FizzBuzz monotouch_throw_monotouch_exception (monotouch-glue.m:1369)
4 FizzBuzz mono_invoke_unhandled_exception_hook (mini-exceptions.c:2745)
5 FizzBuzz mono_thread_abort_dummy (mini.c:2809)
6 FizzBuzz mono_handle_exception_internal + 2149
7 FizzBuzz mono_handle_exception (mini-exceptions.c:1972)
8 FizzBuzz mono_arm_throw_exception (exceptions-arm.c:162)
9 FizzBuzz throw_exception + 68
10 FizzBuzz FB_BL_Managers_FBRegisterManager_OnRegisterSelected_int (FBRegisterManager.cs:97)
11 FizzBuzz FB_AL_TableSource_RowSelected_MonoTouch_UIKit_UITableView_MonoTouch_Foundation_NSIndexPath (TableSource.cs:137)
12 FizzBuzz wrapper_runtime_invoke_object_runtime_invoke_dynamic_intptr_intptr_intptr_intptr + 200
13 FizzBuzz mono_jit_runtime_invoke (mini.c:6457)
14 FizzBuzz mono_runtime_invoke (object.c:2827)
15 FizzBuzz native_to_managed_trampoline_FB_AL_TableSource_RowSelected (registrar.m:1809)
16 UIKit -[UITableView _selectRowAtIndexPath:animated:scrollPosition:notifyDelegate:] + 877
17 UIKit -[UITableView _userSelectRowAtPendingSelectionIndexPath:] + 157
18 Foundation __NSFireDelayedPerform + 451
19 CoreFoundation __CFRUNLOOP_IS_CALLING_OUT_TO_A_TIMER_CALLBACK_FUNCTION + 15
20 CoreFoundation CFRunLoopDoTimer + 273
21 CoreFoundation __CFRunLoopRun + 1233
22 CoreFoundation _CFRunLoopRunSpecific + 357
23 CoreFoundation _CFRunLoopRunInMode + 105
24 GraphicsServices _GSEventRunModal + 75
25 UIKit _UIApplicationMain + 1121
26 FizzBuzz wrapper_managed_to_native_MonoTouch_UIKit_UIApplication_UIApplicationMain_int_string_intptr_intptr (:1)
27 FizzBuzz MonoTouch_UIKit_UIApplication_Main_string___string_string (UIApplication.cs:38)
```

As you can see, there is some relevant information there hidden in the noise:

```
...
10 FizzBuzz FB_BL_Managers_FBRegisterManager_OnRegisterSelected_int (FBRegisterManager.cs:97)
11 FizzBuzz FB_AL_TableSource_RowSelected_MonoTouch_UIKit_UITableView_MonoTouch_Foundation_NSIndexPath (TableSource.cs:137)
...
16 UIKit -[UITableView _selectRowAtIndexPath:animated:scrollPosition:notifyDelegate:] + 877
17 UIKit -[UITableView _userSelectRowAtPendingSelectionIndexPath:] + 157
...
```

But this is still a bummer if you want to know the exception's message and type.

Luckily, you can log most unhandled managed exceptions in `Main` method and tell HockeyApp to read from your log file and attach any information you need to the crash report it generates. [More about this approach here.](https://github.com/stampsy/hockeyapp-monotouch/issues/5#issuecomment-24361780)

## Scope and Contributions

These bindings are very basic and contain just enough code to send crash reports on next launch and suggest app updates. Pull requests with more coverage are welcome.
