using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.LifeSign;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
builder.Services.UseFileSystemStorage(builder.Configuration);
builder.Services.UseLifeSign(builder.Configuration);
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