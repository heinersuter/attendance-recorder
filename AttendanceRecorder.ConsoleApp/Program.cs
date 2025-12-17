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
                formatter: new JsonFormatter(),
                path: "logs/log.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 2)
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(le =>
                    le.Properties.TryGetValue("SourceContext", out var sc) &&
                    sc is Serilog.Events.ScalarValue { Value: string s } &&
                    s.StartsWith("AttendanceRecorder.LifeSign", StringComparison.Ordinal))
                .WriteTo.File(
                    path: "logs/life-sign.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 2,
                    formatProvider: CultureInfo.InvariantCulture))
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
                retainedFileCountLimit: 2)
            .WriteTo.Logger(lc2 => lc2
                .Filter.ByIncludingOnly(le =>
                    le.Properties.TryGetValue("SourceContext", out var sc) &&
                    sc is Serilog.Events.ScalarValue sv &&
                    sv.Value is string s &&
                    s.StartsWith("AttendanceRecorder.LifeSign", StringComparison.Ordinal))
                .WriteTo.File(
                    "logs/lifesign.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 2,
                    formatProvider: CultureInfo.InvariantCulture)));

        var port = builder.Configuration.GetValue<int>("Server:Port");
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(port);
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseCors();
        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Swagger UI available at: {SwaggerUrl}", $"http://localhost:{port}/swagger/index.html");

        var lifeSignService = app.Services.GetRequiredService<LifeSignService>();
        await lifeSignService.StartAsync();

        await app.RunAsync();
    }
}