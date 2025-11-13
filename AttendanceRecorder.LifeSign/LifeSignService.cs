using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public sealed class LifeSignService(
    ILogger<LifeSignService> logger,
    IOptions<LifeSignConfig> config,
    LifeSignWriterService lifeSignWriterService)
    : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ManualResetEventSlim _runEvent = new(false);
    private MacSessionSwitchListener? _macSessionSwitchListener;
    private WindowsSessionSwitchListener? _windowsSessionSwitchListener;

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
        logger.LogInformation("Starting life sign service");
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
        logger.LogInformation("Life sign service paused");
    }

    public void Resume()
    {
        _runEvent.Set();
        logger.LogInformation("Life sign service resumed");
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while writing life sign: {Message}", ex.Message);
            }
        }
    }
}