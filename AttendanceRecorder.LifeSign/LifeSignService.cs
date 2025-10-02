using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public sealed class LifeSignService : IAsyncDisposable
{
    private readonly LifeSignConfig _config;
    private readonly CancellationTokenSource _cts = new();
    private readonly LifeSignWriter _lifeSignWriter;
    private readonly ManualResetEventSlim _runEvent = new(false);

    public LifeSignService(IOptions<LifeSignConfig> lifeSignConfigOptions)
    {
        _config = lifeSignConfigOptions.Value;
        _lifeSignWriter = new LifeSignWriter(_config);
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync().ConfigureAwait(false);
        _cts.Dispose();
        _runEvent.Dispose();
    }

    public async Task StartAsync()
    {
        _runEvent.Set();
        _ = Task.Run(LoopAsync);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public void Stop()
    {
        _runEvent.Reset();
    }

    private async Task LoopAsync()
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
                await Task.Delay(_config.UpdatePeriod, _cts.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}