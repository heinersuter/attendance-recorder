using Microsoft.Win32;

namespace AttendanceRecorder.LifeSign;

public sealed class WindowsSessionSwitchListener : ISessionSwitchListener, IDisposable
{
    private bool _isLocked;

    public WindowsSessionSwitchListener()
    {
        if (OperatingSystem.IsWindows())
        {
            SystemEvents.SessionSwitch += OnSessionSwitch;
        }
    }

    public bool IsLocked()
    {
        return _isLocked;
    }

    public void Dispose()
    {
        if (OperatingSystem.IsWindows())
        {
            SystemEvents.SessionSwitch -= OnSessionSwitch;
        }
    }

    private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        if (e.Reason == SessionSwitchReason.SessionLock)
        {
            _isLocked = true;
        }
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            _isLocked = false;
        }
    }
}