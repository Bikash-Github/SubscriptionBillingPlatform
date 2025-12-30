using Serilog;
//using SubscriptionService.Handlers;
//using SubscriptionService.Middleware;
using BuildingBlocks.Infrastructure.Middleware;
using BuildingBlocks.Infrastructure.Handlers;

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

// ----------------- Build -----------------
var app = builder.Build();

// ----------------- Middleware pipeline -----------------

app.UseMiddleware<CorrelationIdMiddleware>();


app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health/live", () => "Alive");
app.MapGet("/health/ready", () => "Ready");
app.Run();
