using BuildingBlocks.Infrastructure.Handlers;
using BuildingBlocks.Infrastructure.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Serilog ----------------
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] CorrelationId={CorrelationId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ---------------- Services ----------------
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// 🔑 REQUIRED for DelegatingHandler
builder.Services.AddHttpContextAccessor();

// 🔑 Shared handler
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

builder.Services.AddHttpClient("DownstreamClient")
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();


// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "sqlserver",
        timeout: TimeSpan.FromSeconds(5));

// ---------------- Build ----------------
var app = builder.Build();

// ---------------- Middleware ----------------
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false, // liveness = app is running
    ResponseWriter = WriteHealthResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true, // readiness = all checks
    ResponseWriter = WriteHealthResponse
});

app.Run();

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var response = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            error = entry.Value.Exception?.Message
        })
    };

    return context.Response.WriteAsync(
        JsonSerializer.Serialize(response));
}



