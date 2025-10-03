using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AttendanceRecorder.FileSystemStorage;

public static class FileSystemStorageRegistration
{
    public static void AddFileSystemStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileSystemStorageConfig>(configuration.GetRequiredSection(nameof(FileSystemStorageConfig)[..^6]));
        services.Configure<LifeSignConfig>(configuration.GetRequiredSection(nameof(LifeSignConfig)[..^6]));
        services.AddTransient<LifeSignWriterService>();
        services.AddTransient<LifeSignReaderService>();
    }
}