using System.Text.Json.Serialization;

namespace AttendanceRecorder.FileSystemStorage;

public class FileSystemStorageConfig
{
    public required string DirectoryRaw { get; init; }

    public string Directory =>
        DirectoryRaw.Replace("{Home}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
}