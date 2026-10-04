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
        db.Events.Add(new Event
        {
            Id = Guid.NewGuid(),
            Title = "CampusPass Demo Workshop",
            Description = "Demo published event for EK1.",
            StartsAt = DateTime.UtcNow.AddDays(7),
            Capacity = 30,
            RegisteredCount = 0,
            AuthorId = Guid.NewGuid(),
            State = EventState.Published
        });

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
