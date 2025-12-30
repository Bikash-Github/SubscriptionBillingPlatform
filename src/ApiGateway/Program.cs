using ApiGateway.Handlers;
using ApiGateway.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ----------------- Serilog -----------------
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] CorrelationId={CorrelationId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ----------------- Services -----------------
builder.Services.AddHttpLogging(_ => { });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// 🔑 Register handler FIRST
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

// 🔑 Register HttpClient BEFORE Build()
builder.Services.AddHttpClient("DownstreamClient")
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

// ----------------- Build -----------------
var app = builder.Build();

// ----------------- Middleware pipeline -----------------

// CorrelationId must run early
app.UseMiddleware<CorrelationIdMiddleware>();

// Serilog request logging AFTER correlation middleware
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health/live", () => "Alive");
app.MapGet("/health/ready", () => "Ready");

app.Run();
