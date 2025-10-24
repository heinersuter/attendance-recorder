using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.LifeSign;
using AttendanceRecorder.WebApi;
using AttendanceRecorder.WebApi.WorkingDay;

namespace AttendanceRecorder.ConsoleApp;

public sealed class Program
{
    private Program()
    {
    }

    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddFileSystemStorage(builder.Configuration);
        builder.Services.AddLifeSign(builder.Configuration);
        builder.Services.AddWorkingDay(builder.Configuration);

        // Register controllers from WebApi assembly explicitly.
        builder.Services.AddControllers().AddApplicationPart(typeof(GetYearsController).Assembly);

        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI();

        var lifeSignService = app.Services.GetRequiredService<LifeSignService>();
        await lifeSignService.StartAsync();

        await app.RunAsync();
    }
}