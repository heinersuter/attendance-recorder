using System.Diagnostics.CodeAnalysis;
using AttendanceRecorder.BlazorUi.Components;
using AttendanceRecorder.Client;
using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.LifeSign;
using AttendanceRecorder.WebApi;
using AttendanceRecorder.WebApi.WorkingDay;

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
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseStaticWebAssets();

        builder.Services.AddFileSystemStorage(builder.Configuration);
        builder.Services.AddLifeSign(builder.Configuration);
        builder.Services.AddWorkingDay(builder.Configuration);
        builder.Services.AddControllers().AddApplicationPart(typeof(GetYearsController).Assembly);
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped(_ => new HttpClient());
        builder.Services.AddScoped<ApiClient>(services =>
            new ApiClient(builder.WebHost.GetSetting("urls"), services.GetRequiredService<HttpClient>()));

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