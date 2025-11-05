using Microsoft.Extensions.Options;

namespace AttendanceRecorder.FileSystemStorage;

public class MergeWriterService(IOptions<FileSystemStorageConfig> config)
{
    public void WriteActiveMerge(DateOnly date, TimeOnly start, TimeOnly end)
    {
        WriteMerge(date, isActive: true, start, end);
    }

    public void WriteInactiveMerge(DateOnly date, TimeOnly start, TimeOnly end)
    {
        WriteMerge(date, isActive: false, start, end);
    }

    private void WriteMerge(DateOnly date, bool isActive, TimeOnly start, TimeOnly end)
    {
        var yearDirectory = Path.Combine(config.Value.Directory, $"{date.Year}");
        Directory.CreateDirectory(yearDirectory);

        var filePath = Path.Combine(yearDirectory, $"{date:MM-dd}.{(isActive ? "meract" : "merina")}");

        File.AppendAllText(filePath, $"{start:HH:mm:ss}-{end:HH:mm:ss}{Environment.NewLine}");
    }
}