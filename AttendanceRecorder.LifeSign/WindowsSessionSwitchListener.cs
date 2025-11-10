using Microsoft.Win32;

namespace AttendanceRecorder.LifeSign;

public sealed class WindowsSessionSwitchListener : IDisposable
{
    private readonly LifeSignService _lifeSignService;

    public WindowsSessionSwitchListener(LifeSignService lifeSignService)
    {
        _lifeSignService = lifeSignService;

        if (OperatingSystem.IsWindows())
        {
            SystemEvents.SessionSwitch += OnSessionSwitch;
        }
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
            _lifeSignService.Pause();
        }
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            _lifeSignService.Resume();
        }
    }
}