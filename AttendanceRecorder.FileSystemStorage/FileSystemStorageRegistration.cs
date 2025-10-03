using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttendanceRecorder.FileSystemStorage;

public static class FileSystemStorageRegistration
{
    public static void UseFileSystemStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileSystemStorageConfig>(configuration.GetRequiredSection(nameof(FileSystemStorageConfig)[..^6]));
        services.AddTransient<LifeSignWriterService>();
        services.AddTransient<LifeSignReaderService>();
    }
}