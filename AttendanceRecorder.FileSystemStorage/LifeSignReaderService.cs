using System.Globalization;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.FileSystemStorage;

public class LifeSignReaderService(IOptions<FileSystemStorageConfig> config)
{
    public IEnumerable<int> GetYears()
    {
        return Directory
            .GetDirectories(config.Value.Directory)
            .Select(Path.GetFileName)
            .Select(int.Parse!)
            .OrderByDescending(y => y);
    }

    public IEnumerable<DateOnly> GetDates(int year)
    {
        var yearDirectory = Path.Combine(config.Value.Directory, year.ToString(CultureInfo.InvariantCulture));
        if (!Directory.Exists(yearDirectory))
        {
            return [];
        }

        var files = Directory.GetFiles(yearDirectory, "*.attrec");
        var dates = new List<DateOnly>();
        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (DateOnly.TryParseExact(
                    $"{year}-{name}",
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                dates.Add(date);
            }
        }

        return dates;
    }
}