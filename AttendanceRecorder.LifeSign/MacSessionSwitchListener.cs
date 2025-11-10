using System.Runtime.InteropServices;
using System.Text;

namespace AttendanceRecorder.LifeSign;

public sealed class MacSessionSwitchListener : IDisposable
{
    private const string CoreFoundationLib = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const uint KCfStringEncodingUtf8 = 0x08000100;

    public MacSessionSwitchListener(LifeSignService lifeSignService)
    {
        var center = CFNotificationCenterGetDistributedCenter();

        // Callback invoked for both notifications
        CFNotificationCallback cb = (_, _, namePtr, _, _) =>
        {
            var name = CfStringToDotNetString(namePtr);
            var when = DateTimeOffset.Now.ToString("u");

            if (name == "com.apple.screenIsLocked")
            {
                Console.WriteLine($"[{when}] Screen LOCKED");
                lifeSignService.Pause();
            }
            else if (name == "com.apple.screenIsUnlocked")
            {
                Console.WriteLine($"[{when}] Screen UNLOCKED");
                lifeSignService.Resume();
            }
            else
            {
                Console.WriteLine($"[{when}] Other notification: {name}");
            }
        };

        // Subscribe to com.apple.screenIsLocked
        Subscribe(center, "com.apple.screenIsLocked", cb);

        // Subscribe to com.apple.screenIsUnlocked
        Subscribe(center, "com.apple.screenIsUnlocked", cb);
    }

    public void Dispose()
    {
    }

    private static string CfStringToDotNetString(IntPtr cfString)
    {
        if (cfString == IntPtr.Zero)
        {
            return string.Empty;
        }

        var sb = new StringBuilder(256);
        if (CFStringGetCString(cfString, sb, sb.Capacity, KCfStringEncodingUtf8))
        {
            return sb.ToString();
        }

        // Fallback if the string was longer than 256
        var big = new StringBuilder(4096);
        if (CFStringGetCString(cfString, big, big.Capacity, KCfStringEncodingUtf8))
        {
            return big.ToString();
        }

        return string.Empty;
    }

    [DllImport(CoreFoundationLib)]
#pragma warning disable SYSLIB1054
#pragma warning disable SA1201
#pragma warning disable CA1838
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Local
    private static extern IntPtr CFNotificationCenterGetDistributedCenter();

    [DllImport(CoreFoundationLib)]
    private static extern void CFNotificationCenterAddObserver(
        IntPtr center,
        IntPtr observer,
        CFNotificationCallback callback,
        IntPtr name, // CFStringRef (nullable)
        IntPtr obj, // CFTypeRef (nullable)
        CFNotificationSuspensionBehavior suspensionBehavior);

    [DllImport(CoreFoundationLib, CharSet = CharSet.Unicode)]
    private static extern IntPtr CFStringCreateWithCString(
        IntPtr alloc, string str, uint encoding);

    [DllImport(CoreFoundationLib, CharSet = CharSet.Unicode)]
    private static extern bool CFStringGetCString(
        IntPtr theString, StringBuilder buffer, nint bufferSize, uint encoding);

    [DllImport(CoreFoundationLib)]
    private static extern void CFRelease(IntPtr cfTypeRef);

    private static void Subscribe(IntPtr center, string notificationName, CFNotificationCallback cb)
    {
        var nameRef = CFStringCreateWithCString(IntPtr.Zero, notificationName, KCfStringEncodingUtf8);
        try
        {
            CFNotificationCenterAddObserver(
                center,
                IntPtr.Zero,
                cb,
                nameRef,
                IntPtr.Zero,
                CFNotificationSuspensionBehavior.DeliverImmediately);
        }
        finally
        {
            CFRelease(nameRef);
        }
    }

    private enum CFNotificationSuspensionBehavior : uint
    {
        Drop = 1,
        Coalesce = 2,
        Hold = 3,
        DeliverImmediately = 4,
    }

    private delegate void CFNotificationCallback(
        IntPtr center, IntPtr observer, IntPtr name, IntPtr obj, IntPtr userInfo);

    // ReSharper restore UnusedMember.Local
    // ReSharper restore InconsistentNaming
#pragma warning restore CA1838
#pragma warning restore SA1201
#pragma warning restore SYSLIB1054
}