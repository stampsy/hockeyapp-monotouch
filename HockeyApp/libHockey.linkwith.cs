using System;
using MonoTouch.ObjCRuntime;

[assembly: LinkWith ("libHockey.a", LinkTarget.ArmV7 | LinkTarget.Simulator, ForceLoad = true)]
