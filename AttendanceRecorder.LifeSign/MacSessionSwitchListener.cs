using System.Runtime.InteropServices;

namespace AttendanceRecorder.LifeSign;

public sealed class MacSessionSwitchListener : IDisposable
{
    private const string CoreFoundationLib = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const string CoreGraphicsLib = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
    private readonly LifeSignService _lifeSignService;

    private bool? _wasLocked;

    public MacSessionSwitchListener(LifeSignService lifeSignService)
    {
        _lifeSignService = lifeSignService;
        _ = new Timer(CheckScreenLockState, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
    }

    public void Dispose()
    {
    }

    private void CheckScreenLockState(object? state)
    {
        var sessionInfo = CGSessionCopyCurrentDictionary();

        if (sessionInfo == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to get session info");
        }

        try
        {
            var screenLockedKey = CFStringCreateWithCString(IntPtr.Zero, "CGSSessionScreenIsLocked", 0x08000100);
            var screenLockedValue = CFDictionaryGetValue(sessionInfo, screenLockedKey);

            var screenIsLocked = screenLockedValue != IntPtr.Zero && CFBooleanGetValue(screenLockedValue);

            CFRelease(screenLockedKey);

            if (!_wasLocked.HasValue || screenIsLocked != _wasLocked.Value)
            {
                if (screenIsLocked)
                {
                    _lifeSignService.Pause();
                }
                else
                {
                    _lifeSignService.Resume();
                }
            }

            _wasLocked = screenIsLocked;
        }
        finally
        {
            CFRelease(sessionInfo);
        }
    }

#pragma warning disable CA2101
#pragma warning disable SYSLIB1054

    [DllImport(CoreGraphicsLib)]
    private static extern IntPtr CGSessionCopyCurrentDictionary();

    [DllImport(CoreFoundationLib)]
    private static extern IntPtr CFStringCreateWithCString(IntPtr alloc, string str, uint encoding);

    [DllImport(CoreFoundationLib)]
    private static extern void CFRelease(IntPtr cfTypeRef);

    [DllImport(CoreFoundationLib)]
    private static extern IntPtr CFDictionaryGetValue(IntPtr theDict, IntPtr key);

    [DllImport(CoreFoundationLib)]
    private static extern bool CFBooleanGetValue(IntPtr boolean);

#pragma warning restore CA2101
#pragma warning restore SYSLIB1054
}