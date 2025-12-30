//using BillingService.Middleware;
using BuildingBlocks.Infrastructure.Middleware;
using Serilog;

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

var app = builder.Build();

// MUST be before Serilog request logging
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health/live", () => "Alive");
app.MapGet("/health/ready", () => "Ready");
app.Run();
