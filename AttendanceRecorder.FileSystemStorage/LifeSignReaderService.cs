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
}