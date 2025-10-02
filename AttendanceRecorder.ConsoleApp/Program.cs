using AttendanceRecorder.LifeSign;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttendanceRecorder.ConsoleApp;

public static class Program
{
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.UseLifeSign(context);
                services.Configure<LifeSignConfig>(context.Configuration.GetSection("LifeSignConfig"));
            })
            .Build();

        var lifeSignService = host.Services.GetRequiredService<LifeSignService>();
        lifeSignService.Start();

        Console.Read();
    }
}
