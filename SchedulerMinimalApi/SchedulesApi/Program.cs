using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using SchedulerDatabase;
using SchedulerDatabase.Models;
using SchedulerDatabase.Models.DTOs;
using SchedulesApi.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var schedules = app.MapGroup("/schedules");

schedules.MapGet("/", async (AppDbContext db) => await db.Schedules.ToListAsync());
schedules.MapGet("/{id:int}",
    async (AppDbContext db, int id) =>
    {
        return await db.Schedules
            .Include(schedule => schedule.Address)
            .FirstOrDefaultAsync(schedule => schedule.Id == id);
    });
schedules.MapPost("/", async (AppDbContext db, ScheduleDto scheduleDto) =>
{
    var schedule = new Schedule
    {
        UserId = scheduleDto.UserId,
        ScheduleName = scheduleDto.ScheduleName,
        Start = scheduleDto.Start.ToUniversalTime(),
        End = scheduleDto.End.ToUniversalTime()
    };

    if (string.IsNullOrEmpty(scheduleDto.ScheduleName))
    {
        return Results.BadRequest(ErrorMessages.ScheduleNameRequired);
    }

    if (schedule.Start > schedule.End)
    {
        return Results.BadRequest(ErrorMessages.StartDateAfterEndDate);
    }

    if (!await db.Users.AnyAsync(u => u.Id == schedule.UserId))
    {
        return Results.NotFound(ErrorMessages.UserNotFound);
    }

    if (await db.Schedules.AnyAsync(s => s.ScheduleName == schedule.ScheduleName))
    {
        return Results.Conflict(ErrorMessages.ScheduleAlreadyExists);
    }

    db.Schedules.Add(schedule);
    await db.SaveChangesAsync();

    return Results.Created($"/schedules/{schedule.Id}", schedule);
});

schedules.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
{
    var schedule = await db.Schedules.FindAsync(id);

    if (schedule is null)
    {
        return Results.NotFound();
    }

    db.Schedules.Remove(schedule);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();