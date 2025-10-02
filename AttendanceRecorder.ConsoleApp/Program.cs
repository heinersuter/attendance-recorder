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
                config.AddJsonFile("appsettings.json", false);
            })
            .ConfigureServices((context, services) =>
            {
                services.UseLifeSign(context);
            })
            .Build();

        var lifeSignService = host.Services.GetRequiredService<LifeSignService>();
        lifeSignService.Start();

        Console.Read();
    }
}