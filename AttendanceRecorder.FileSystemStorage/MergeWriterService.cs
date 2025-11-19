using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.FileSystemStorage;

public class MergeWriterService(ILogger<MergeWriterService> logger, IOptions<FileSystemStorageConfig> config)
{
    public void WriteActiveMerge(DateOnly day, TimeOnly start, TimeOnly end)
    {
        WriteMerge(day, isActive: true, start, end);
    }

    public void WriteInactiveMerge(DateOnly day, TimeOnly start, TimeOnly end)
    {
        WriteMerge(day, isActive: false, start, end);
    }

    private void WriteMerge(DateOnly day, bool isActive, TimeOnly start, TimeOnly end)
    {
        var yearDirectory = Path.Combine(config.Value.Directory, $"{day.Year}");
        Directory.CreateDirectory(yearDirectory);

        var filePath = Path.Combine(yearDirectory, $"{day:MM-dd}.{(isActive ? "meract" : "merina")}");

        File.AppendAllText(filePath, $"{start:HH:mm:ss}-{end:HH:mm:ss}{Environment.NewLine}");

        logger.LogInformation("Wrote {MergeType} merge to {FilePath}", isActive ? "active" : "inactive", filePath);
    }
}