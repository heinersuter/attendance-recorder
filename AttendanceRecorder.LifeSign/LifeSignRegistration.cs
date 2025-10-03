using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttendanceRecorder.LifeSign;

public static class LifeSignRegistration
{
    public static void AddLifeSign(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<LifeSignService>();
    }
}
