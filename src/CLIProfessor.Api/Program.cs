using CLIProfessor.Api.Middleware;
using CLIProfessor.Application.Features.CommandSuggestions.Queries;
using CLIProfessor.Domain.Entities;
using CLIProfessor.Domain.Interfaces;
using CLIProfessor.Infrastructure.Persistence;
using CLIProfessor.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Domain & Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetCommandSuggestionQuery).Assembly));

// Infrastructure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseVector()));

builder.Services.AddScoped<IVectorStore, PostgresVectorStore>();
builder.Services.AddScoped<IGeminiService, GeminiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware
app.UseMiddleware<ToonFormatterMiddleware>();

// Endpoints
app.MapPost("/suggest", async (IMediator mediator, [FromBody] SuggestRequest request) =>
{
    var query = new GetCommandSuggestionQuery(request.NaturalLanguageInput, new TerminalContext(request.Context.OS, request.Context.Shell, request.Context.CurrentDirectory));
    var result = await mediator.Send(query);
    return Results.Ok(result);
})
.WithName("GetCommandSuggestion")
.WithOpenApi();

app.Run();

// Simple DTO for the request body
public record SuggestRequest(string NaturalLanguageInput, SuggestContext Context);
public record SuggestContext(string OS, string Shell, string CurrentDirectory);
