using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttendanceRecorder.FileSystemStorage;

public static class FileSystemStorageRegistration
{
    public static void UseFileSystemStorage(this IServiceCollection services, HostBuilderContext context)
    {
        services.Configure<FileSystemStorageConfig>(context.Configuration.GetRequiredSection(nameof(FileSystemStorageConfig)[..^6]));
        services.AddTransient<LifeSignWriterService>();
    }
}