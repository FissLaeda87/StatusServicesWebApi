using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StatusServicesApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EventsDbContext>();

var app = builder.Build();

app.MapGet("/", () => "Hello, Minimal API!");

app.MapPost("/failures", async (HttpContext context, EventsDbContext dbContext) =>
{
    var request = await context.Request.ReadFromJsonAsync<Event>();

    var @event = new Event
    {
        Service = request.Service,
        Status = "Failure",
        Message = request.Message,
        Timestamp = DateTime.Now
    };

    dbContext.Events.Add(@event);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
});

app.MapPost("/recoveries", async (HttpContext context, EventsDbContext dbContext) =>
{
    var request = await context.Request.ReadFromJsonAsync<Event>();

    var @event = new Event
    {
        Service = request.Service,
        Status = "Recovery",
        Message = request.Message,
        Timestamp = DateTime.Now
    };

    dbContext.Events.Add(@event);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/events", (HttpContext context, EventsDbContext dbContext) =>
{
    var startDate = DateTime.Parse(context.Request.Query["start_date"]);
    var endDate = DateTime.Parse(context.Request.Query["end_date"]).AddDays(1);

    var events = dbContext.Events
    .Where(e => e.Timestamp >= startDate && e.Timestamp < endDate)
    .ToList();

    return Results.Ok(events);
});


// ƒобавл€ем новый endpoint POST дл€ добавлени€ событий
app.MapPost("/api/events", async (EventsDbContext dbContext, Event newEvent) =>
{
    try
    {
        
        newEvent.IsArchived = newEvent.Timestamp < DateTime.Now.AddMonths(-1);

        dbContext.Events.Add(newEvent);
        await dbContext.SaveChangesAsync();

        return Results.Ok("Event added successfully");
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred: {ex.Message}", null, StatusCodes.Status500InternalServerError);
    }
});

app.MapGet("/api/events/archived", async (EventsDbContext dbContext) =>
{
    var archivedEvents = await dbContext.Events.Where(e => e.IsArchived).ToListAsync();
    return Results.Ok(archivedEvents);
});

app.Run();