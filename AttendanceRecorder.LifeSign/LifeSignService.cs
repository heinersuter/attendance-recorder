using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public sealed class LifeSignService(IOptions<LifeSignConfig> config, LifeSignWriterService lifeSignWriterService)
    : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ManualResetEventSlim _runEvent = new(false);
    private WindowsSessionSwitchListener? _windowsSessionSwitchListener;
    private MacSessionSwitchListener? _macSessionSwitchListener;

    public async ValueTask DisposeAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _runEvent.Dispose();
        _windowsSessionSwitchListener?.Dispose();
        _macSessionSwitchListener?.Dispose();
    }

    public async Task StartAsync()
    {
        _runEvent.Set();
        _ = Task.Run(LoopAsync);
        await Task.CompletedTask;
        if (OperatingSystem.IsWindows())
        {
            _windowsSessionSwitchListener = new WindowsSessionSwitchListener(this);
        }
        else if (OperatingSystem.IsMacOS())
        {
            _macSessionSwitchListener = new MacSessionSwitchListener(this);
        }
    }

    public void Pause()
    {
        _runEvent.Reset();
    }

    public void Resume()
    {
        _runEvent.Set();
    }

    private async Task LoopAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            _runEvent.Wait(_cancellationTokenSource.Token);
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                break;
            }

            await lifeSignWriterService.WriteLifeSignAsync();

            try
            {
                await Task.Delay(config.Value.UpdatePeriod, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}