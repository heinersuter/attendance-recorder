using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public class LifeSignWriter(IOptions<LifeSignConfig> lifeSignConfigOptions)
{
    private readonly LifeSignConfig _config = lifeSignConfigOptions.Value;

    public void WriteLifeSign()
    {
        Directory.CreateDirectory(_config.Directory);

        var now = DateTime.Now;

        var filePath = Path.Combine(_config.Directory, $"{now:yyyy-MM-dd}.attrec");

        File.AppendAllText(filePath, $"{now:HH:mm:ss}{Environment.NewLine}");
    }
}