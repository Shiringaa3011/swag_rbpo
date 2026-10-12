using CampusPass.Api.Data;
using CampusPass.Api.Models;
using Microsoft.EntityFrameworkCore;

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

app.Run();

public partial class Program
{
}
