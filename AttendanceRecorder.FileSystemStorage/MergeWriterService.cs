using Microsoft.Extensions.Options;

namespace AttendanceRecorder.FileSystemStorage;

public class MergeWriterService(IOptions<FileSystemStorageConfig> config)
{
    public void WriteMerge(DateOnly date, TimeOnly start, TimeOnly end)
    {
        var yearDirectory = Path.Combine(config.Value.Directory, $"{date.Year}");
        Directory.CreateDirectory(yearDirectory);

        var filePath = Path.Combine(yearDirectory, $"{date:MM-dd}.attrecmer");

        File.AppendAllText(filePath, $"{start:HH:mm:ss}-{end:HH:mm:ss}{Environment.NewLine}");
    }
}