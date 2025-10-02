using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public sealed class LifeSignService : IDisposable
{
    private readonly LifeSignWriter _lifeSignWriter;
    private readonly LifeSignConfig _config;
    private readonly ManualResetEventSlim _runEvent = new(false);
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _backgroundTask;

    public LifeSignService(IOptions<LifeSignConfig> lifeSignConfigOptions)
    {
        _config = lifeSignConfigOptions.Value;
        _lifeSignWriter = new LifeSignWriter(_config);
        _backgroundTask = Task.Run(BackgroundLoop);
    }

    public void Start()
    {
        _runEvent.Set();
    }

    public void Stop()
    {
        _runEvent.Reset();
    }

    private async Task BackgroundLoop()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            _runEvent.Wait(_cts.Token);
            if (_cts.Token.IsCancellationRequested)
            {
                break;
            }
            
            _lifeSignWriter.WriteLifeSign();
            
            try
            {
                await Task.Delay(_config.UpdatePeriod, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _backgroundTask.Wait();
        _cts.Dispose();
        _runEvent.Dispose();
    }
}