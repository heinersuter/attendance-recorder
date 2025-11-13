using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.FileSystemStorage;

public class LifeSignWriterService(ILogger<LifeSignWriterService> logger, IOptions<FileSystemStorageConfig> config)
{
    public async Task WriteLifeSignAsync()
    {
        var now = DateTime.Now;

        var yearDirectory = Path.Combine(config.Value.Directory, $"{now.Year}");
        Directory.CreateDirectory(yearDirectory);

        var filePath = Path.Combine(yearDirectory, $"{now:MM-dd}.attrec");

        await File.AppendAllTextAsync(filePath, $"{now:HH:mm:ss}{Environment.NewLine}");

        logger.LogInformation("Wrote life sign to {FilePath}", filePath);
    }
}