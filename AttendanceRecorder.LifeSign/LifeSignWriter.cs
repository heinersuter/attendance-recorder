namespace AttendanceRecorder.LifeSign;

public class LifeSignWriter(LifeSignConfig config)
{
    public void WriteLifeSign()
    {
        var now = DateTime.Now;

        var yearDirectory = Path.Combine(config.Directory, $"{now.Year}");
        Directory.CreateDirectory(yearDirectory);

        var filePath = Path.Combine(yearDirectory, $"{now:MM-dd}.attrec");

        File.AppendAllText(filePath, $"{now:HH:mm:ss}{Environment.NewLine}");
    }
}