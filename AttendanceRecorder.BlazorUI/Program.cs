using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AttendanceRecorder.BlazorUi.Components;
using AttendanceRecorder.Client;
using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.LifeSign;
using AttendanceRecorder.WebApi;
using AttendanceRecorder.WebApi.WorkingDay;
using Serilog;
using Serilog.Formatting.Json;

namespace AttendanceRecorder.BlazorUi;

[SuppressMessage("ReSharper", "ConvertToStaticClass", Justification = "Must not be static")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Not true")]
public sealed class Program
{
    private Program()
    {
    }

    public static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.File(
                new JsonFormatter(),
                "logs/log.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 2)
            .CreateBootstrapLogger();
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseStaticWebAssets();

        builder.Services.AddFileSystemStorage(builder.Configuration);
        builder.Services.AddLifeSign(builder.Configuration);
        builder.Services.AddWorkingDay(builder.Configuration);
        builder.Services.AddControllers().AddApplicationPart(typeof(GetYearsController).Assembly);
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSerilog((services, lc) => lc
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.File(
                new JsonFormatter(),
                "logs/log.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 2));

        builder.Services.AddScoped<ApiClient>(_ =>
            new ApiClient(builder.WebHost.GetSetting("urls"), new HttpClient()));

        var app = builder.Build();

        app.UseRouting();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        var lifeSignService = app.Services.GetRequiredService<LifeSignService>();
        await lifeSignService.StartAsync();

        await app.RunAsync();
    }
}