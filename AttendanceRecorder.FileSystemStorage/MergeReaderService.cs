using System.Globalization;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.FileSystemStorage;

public class MergeReaderService(IOptions<FileSystemStorageConfig> config)
{
    public IEnumerable<(TimeOnly Start, TimeOnly End)> GetActiveMerges(DateOnly date)
    {
        return GetMerges(date, isActive: true);
    }

    public IEnumerable<(TimeOnly Start, TimeOnly End)> GetInactiveMerges(DateOnly date)
    {
        return GetMerges(date, isActive: false);
    }

    private IEnumerable<(TimeOnly Start, TimeOnly End)> GetMerges(DateOnly date, bool isActive)
    {
        var filePath = Path.Combine(
            config.Value.Directory,
            date.Year.ToString(CultureInfo.InvariantCulture),
            $"{date:MM-dd}.{(isActive ? "meract" : "merina")}");

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