using System.Globalization;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.FileSystemStorage;

public class MergeReaderService(IOptions<FileSystemStorageConfig> config)
{
    public IEnumerable<(TimeOnly Start, TimeOnly End)> GetActiveMerges(DateOnly day)
    {
        return GetMerges(day, isActive: true);
    }

    public IEnumerable<(TimeOnly Start, TimeOnly End)> GetInactiveMerges(DateOnly day)
    {
        return GetMerges(day, isActive: false);
    }

    private IEnumerable<(TimeOnly Start, TimeOnly End)> GetMerges(DateOnly day, bool isActive)
    {
        var filePath = Path.Combine(
            config.Value.Directory,
            day.Year.ToString(CultureInfo.InvariantCulture),
            $"{day:MM-dd}.{(isActive ? "meract" : "merina")}");

        if (!File.Exists(filePath))
        {
            return [];
        }

        return File
            .ReadAllLines(filePath)
            .Select(line =>
            {
                var parts = line.Split('-');
                var start = TimeOnly.ParseExact(parts[0], "HH:mm:ss", CultureInfo.InvariantCulture);
                var end = TimeOnly.ParseExact(parts[1], "HH:mm:ss", CultureInfo.InvariantCulture);
                return (start, end);
            });
    }
}