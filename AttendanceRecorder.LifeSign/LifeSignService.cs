using System.ComponentModel;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public class LifeSignService
{
    private readonly LifeSignWriter _lifeSignWriter;
    private readonly LifeSignConfig _config;
    private readonly BackgroundWorker _backgroundWorker = new();
    private readonly ManualResetEvent _runEvent = new(false);

    public LifeSignService(IOptions<LifeSignConfig> lifeSignConfigOptions, LifeSignWriter lifeSignWriter)
    {
        _lifeSignWriter = lifeSignWriter;
        _config = lifeSignConfigOptions.Value;
        _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
        _backgroundWorker.RunWorkerAsync();
    }

    public void Start()
    {
        _runEvent.Set();
    }

    public void Stop()
    {
        _runEvent.Reset();
    }

    private void BackgroundWorkerOnDoWork(object? sender, DoWorkEventArgs doWorkEventArgs)
    {
        while (_backgroundWorker.IsBusy)
        {
            if (_runEvent.WaitOne())
            {
                _lifeSignWriter.WriteLifeSign();
            }

            Thread.Sleep(_config.UpdatePeriod);
        }
    }
}
