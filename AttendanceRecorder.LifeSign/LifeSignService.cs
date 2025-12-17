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
    private ISessionSwitchListener? _sessionSwitchListener;
    private WindowsSessionSwitchListener? _windowsSessionSwitchListener;

    public async ValueTask DisposeAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _runEvent.Dispose();
        _windowsSessionSwitchListener?.Dispose();
    }

    public async Task StartAsync()
    {
        logger.LogInformation("Starting life sign service");
        _runEvent.Set();
        _ = Task.Run(LoopAsync);
        await Task.CompletedTask;
        if (OperatingSystem.IsWindows())
        {
            logger.LogInformation("Attaching WindowsSessionSwitchListener");
            _windowsSessionSwitchListener = new WindowsSessionSwitchListener();
            _sessionSwitchListener = _windowsSessionSwitchListener;
        }
        else if (OperatingSystem.IsMacOS())
        {
            logger.LogInformation("Attaching MacSessionSwitchListener");
            _sessionSwitchListener = new MacSessionSwitchListener();
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

            try
            {
                if (_sessionSwitchListener == null || !_sessionSwitchListener.IsLocked())
                {
                    await lifeSignWriterService.WriteLifeSignAsync();
                }
                else
                {
                    logger.LogInformation("Session is locked, no life sign is written");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while writing life sign: {Message}", ex.Message);
            }

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