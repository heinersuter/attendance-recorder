using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.LifeSign;
using AttendanceRecorder.WebApi.Model;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
builder.Services.AddFileSystemStorage(builder.Configuration);
builder.Services.AddLifeSign(builder.Configuration);
builder.Services.AddWorkingDay(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Run application
var app = builder.Build();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

var lifeSignService = app.Services.GetRequiredService<LifeSignService>();
await lifeSignService.StartAsync();

await app.RunAsync();