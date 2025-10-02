using Microsoft.Extensions.Options;

namespace AttendanceRecorder.LifeSign;

public class LifeSignWriter(LifeSignConfig config)
{
    public void WriteLifeSign()
    {
        Directory.CreateDirectory(config.Directory);

        var now = DateTime.Now;

        var filePath = Path.Combine(config.Directory, $"{now:yyyy-MM-dd}.attrec");

        File.AppendAllText(filePath, $"{now:HH:mm:ss}{Environment.NewLine}");
    }
}