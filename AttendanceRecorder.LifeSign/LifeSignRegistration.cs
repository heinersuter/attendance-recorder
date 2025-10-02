using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttendanceRecorder.LifeSign;

public static class LifeSignRegistration
{
    public static void UseLifeSign(this IServiceCollection services, HostBuilderContext context)
    {
        services.Configure<LifeSignConfig>(context.Configuration.GetSection("LifeSignConfig"));
        services.AddTransient<LifeSignService>();
    }
}
