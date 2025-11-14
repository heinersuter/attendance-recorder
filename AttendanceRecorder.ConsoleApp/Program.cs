using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.LifeSign;
using AttendanceRecorder.WebApi;
using AttendanceRecorder.WebApi.WorkingDay;
using Serilog;
using Serilog.Formatting.Json;

namespace AttendanceRecorder.ConsoleApp;

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

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.SetIsOriginAllowed(origin => new Uri(origin).Host is "localhost" or "127.0.0.1")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddFileSystemStorage(builder.Configuration);
        builder.Services.AddLifeSign(builder.Configuration);
        builder.Services.AddWorkingDay(builder.Configuration);
        builder.Services.AddControllers().AddApplicationPart(typeof(GetYearsController).Assembly);
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

        var app = builder.Build();

        app.UseCors();
        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var port = builder.Configuration.GetValue<int>("Server:Port");
        logger.LogInformation("Swagger UI available at: {SwaggerUrl}", $"http://localhost:{port}/swagger/index.html");

        var lifeSignService = app.Services.GetRequiredService<LifeSignService>();
        await lifeSignService.StartAsync();

        await app.RunAsync();
    }
}