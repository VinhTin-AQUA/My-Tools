using System.Diagnostics;
using Thanh_Video_Labeler.Extensions;
using Thanh_Video_Labeler.Hubs;
using Thanh_Video_Labeler.Hubs.VideoAws;
using Thanh_Video_Labeler.Middlewares;
using Thanh_Video_Labeler.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSqliteDbContext(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddSignalR();

// enable cors
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", option => option
        .WithOrigins("http://localhost:4200", "https://thanh-video-labeler-app.vercel.app")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

app.StartMigrationPending();

using (var scope = app.Services.CreateScope())
{
    var configService = scope.ServiceProvider.GetService<ConfigService>();
    if (configService != null)
    {
        await configService.Init(scope.ServiceProvider);
    }
    
    var videoAwsService = scope.ServiceProvider.GetService<VideoAwsService>();
    if (videoAwsService != null)
    {
        await videoAwsService.Init(scope.ServiceProvider);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Lifetime.ApplicationStarted.Register(() =>
{
    var url = "http://localhost:5000";

    Process.Start(new ProcessStartInfo()
    {
        FileName = url,
        UseShellExecute = true
    }); 
});

app.UseCors("AllowOrigin");

//app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.UseStaticFiles();

app.MapHub<VideoDowloadHub>("/VideoDowloadHub");
app.MapHub<VideoAwsHub>("/AwsVideoDowloadHub");

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();