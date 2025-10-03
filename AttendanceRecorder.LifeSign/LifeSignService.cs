using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public sealed class LifeSignService(IOptions<LifeSignConfig> config, LifeSignWriterService lifeSignWriterService)
    : IAsyncDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private readonly ManualResetEventSlim _runEvent = new(false);

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        _cts.Dispose();
        _runEvent.Dispose();
    }

    public async Task StartAsync()
    {
        _runEvent.Set();
        _ = Task.Run(LoopAsync);
        await Task.CompletedTask;
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

            await lifeSignWriterService.WriteLifeSignAsync();

            try
            {
                await Task.Delay(config.Value.UpdatePeriod, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}