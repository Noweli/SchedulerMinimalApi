using Microsoft.EntityFrameworkCore;
using SchedulerDatabase;
using SchedulerDatabase.Models;
using UserApi.Resources;

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

var users = app.MapGroup("/users");

users.MapGet("/", async (AppDbContext db) => await db.Users.ToListAsync());
users.MapGet("/{id:int}", async (AppDbContext db, int id) => await db.Users.FindAsync(id));
users.MapPost("/", async (AppDbContext db, string userName) =>
{
    var user = new User { Username = userName };
    
    if(string.IsNullOrEmpty(user.Username))
    {
        return Results.BadRequest(ErrorMessages.UsernameRequired);
    }
    
    if(await db.Users.AnyAsync(u => u.Username == user.Username))
    {
        return Results.Conflict(ErrorMessages.UserAlreadyExists);
    }

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user);
});

users.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
{
    var user = await db.Users.FindAsync(id);

    if(user is null)
    {
        return Results.NotFound();
    }

    db.Users.Remove(user);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();