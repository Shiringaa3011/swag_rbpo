using CampusPass.Api.Data;
using CampusPass.Api.Models;
using Microsoft.EntityFrameworkCore;

// Тот же бинарный файл выполняет HTTP-пробу без curl в runtime-образе.
if (args.Length == 1 && args[0] == "--healthcheck")
{
    try
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        var url = Environment.GetEnvironmentVariable("CAMPUSPASS_HEALTH_URL")
            ?? "http://127.0.0.1:8080/health";
        using var response = await client.GetAsync(url);
        Environment.ExitCode = response.IsSuccessStatusCode ? 0 : 1;
    }
    catch (HttpRequestException)
    {
        Environment.ExitCode = 1;
    }
    catch (TaskCanceledException)
    {
        Environment.ExitCode = 1;
    }

    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CampusPassDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("CampusPass")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusPassDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.Events.AnyAsync())
    {
        var utcNow = DateTime.UtcNow;
        var demo = Event.Create(
            "CampusPass Demo Workshop",
            "Demo published event for EK1.",
            utcNow.AddDays(7),
            30,
            Guid.NewGuid(),
            utcNow);
        demo.State = EventState.Published;
        db.Events.Add(demo);

        await db.SaveChangesAsync();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/info", (IConfiguration configuration) =>
    Results.Ok(new
    {
        service = "CampusPass",
        label = configuration["APP_LABEL"] ?? "campuspass-local"
    }));

app.Run();

public partial class Program
{
}
