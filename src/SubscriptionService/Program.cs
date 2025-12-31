using BuildingBlocks.Infrastructure.Handlers;
using BuildingBlocks.Infrastructure.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Serilog bootstrap
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] CorrelationId={CorrelationId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddHttpLogging(_ => { });



builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpContextAccessor();

// 🔑 Register handler FIRST
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

// 🔑 Register HttpClient BEFORE Build()
builder.Services.AddHttpClient("BillingClient")
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "sqlserver",
        timeout: TimeSpan.FromSeconds(5));

// ----------------- Build -----------------
var app = builder.Build();

// ----------------- Middleware pipeline -----------------

app.UseMiddleware<CorrelationIdMiddleware>();


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