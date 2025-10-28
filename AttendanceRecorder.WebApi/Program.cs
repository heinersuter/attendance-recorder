using System.Diagnostics.CodeAnalysis;

namespace AttendanceRecorder.WebApi;

[SuppressMessage("ReSharper", "ConvertToStaticClass", Justification = "Must not be static")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Not true")]
public sealed class Program
{
    private Program()
    {
    }

    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddControllers().AddApplicationPart(typeof(GetYearsController).Assembly);
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        app.Run();
    }
}