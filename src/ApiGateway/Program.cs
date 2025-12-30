using BuildingBlocks.Infrastructure.Middleware;
using BuildingBlocks.Infrastructure.Handlers;
using Serilog;

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

// ---------------- Build ----------------
var app = builder.Build();

// ---------------- Middleware ----------------
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health/live", () => "Alive");
app.MapGet("/health/ready", () => "Ready");

app.Run();
