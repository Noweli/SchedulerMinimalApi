using AddressApi.Resources;
using Microsoft.EntityFrameworkCore;
using SchedulerDatabase;
using SchedulerDatabase.Models;
using SchedulerDatabase.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var address = app.MapGroup("/address");

address.MapGet("/", async (AppDbContext dbContext) => await dbContext.Addresses.ToListAsync());
address.MapGet("/{id:int}", async (AppDbContext dbContext, int id) =>
{
    var addressEntity = await dbContext.Addresses.FindAsync(id);

    return addressEntity is null ? Results.NotFound() : Results.Ok(address);
});

address.MapPost("/", async (AppDbContext dbContext, AddressDto addressDto) =>
{
    if (string.IsNullOrEmpty(addressDto.City))
    {
        return Results.BadRequest(ErrorMessages.CityIsRequired);
    }
    
    var schedule = await dbContext.Schedules.FindAsync(addressDto.ScheduleId);
    
    if (schedule is null)
    {
        return Results.BadRequest(ErrorMessages.ScheduleNotFound);
    }
    
    var addressEntity = new Address
    {
        Street = addressDto.Street,
        City = addressDto.City,
        HomeNo = addressDto.HomeNo,
        Schedule = schedule
    };
    
    await dbContext.Addresses.AddAsync(addressEntity);
    await dbContext.SaveChangesAsync();
    
    return Results.Created($"/address/{addressEntity.Id}", addressEntity);
});

address.MapDelete("/{id:int}", async (AppDbContext dbContext, int id) =>
{
    var addressEntity = await dbContext.Addresses.FindAsync(id);

    if (addressEntity is null)
    {
        return Results.NotFound();
    }

    dbContext.Addresses.Remove(addressEntity);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();