using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AttendanceRecorder.WebApi.Model;

public static class WorkingDayRegistration
{
    public static void AddWorkingDay(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<WorkingDayService>();
    }
}