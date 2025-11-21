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
            .Select(int.Parse!);
    }

    public IEnumerable<int> GetWeeksByYear(int year)
    {
        return GetDaysByYear(year)
            .Select(GetWeekOfYear)
            .Distinct();
    }

    public IEnumerable<DateOnly> GetDaysByWeek(int year, int week)
    {
        return GetDaysByYear(year)
            .Where(day => GetWeekOfYear(day) == week);
    }

    public IEnumerable<TimeOnly> GetLifeSigns(DateOnly day)
    {
        var filePath = Path.Combine(
            config.Value.Directory,
            day.Year.ToString(CultureInfo.InvariantCulture),
            $"{day.Month:D2}-{day.Day:D2}.attrec");

        if (!File.Exists(filePath))
        {
            return [];
        }

        return File
            .ReadAllLines(filePath)
            .Select(line => TimeOnly.ParseExact(line, "HH:mm:ss", CultureInfo.InvariantCulture));
    }

    private IEnumerable<DateOnly> GetDaysByYear(int year)
    {
        var yearDirectory = Path.Combine(config.Value.Directory, year.ToString(CultureInfo.InvariantCulture));
        if (!Directory.Exists(yearDirectory))
        {
            return [];
        }

        return Directory
            .GetFiles(yearDirectory, "*.attrec")
            .Select(filePath => ToDateOnly(year, filePath));
    }

    private static int GetWeekOfYear(DateOnly day)
    {
        var calendar = CultureInfo.GetCultureInfo("de-CH").Calendar;
        return calendar.GetWeekOfYear(
            day.ToDateTime(TimeOnly.MinValue),
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
    }

    private static DateOnly ToDateOnly(int year, string filePath)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

        var parts = fileNameWithoutExtension.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            throw new FormatException($"Invalid file name format: {filePath}");
        }

        if (!int.TryParse(parts[0], out var month) ||
            !int.TryParse(parts[1], out var day))
        {
            throw new FormatException($"Invalid date components in file name: {filePath}");
        }

        return new DateOnly(year, month, day);
    }
}