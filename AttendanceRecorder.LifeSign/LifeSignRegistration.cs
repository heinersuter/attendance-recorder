using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttendanceRecorder.LifeSign;

public static class LifeSignRegistration
{
    public static void UseLifeSign(this IServiceCollection services, HostBuilderContext context)
    {
        services.Configure<LifeSignConfig>(context.Configuration.GetRequiredSection(nameof(LifeSignConfig)[..^6]));
        services.AddTransient<LifeSignService>();
    }
}
