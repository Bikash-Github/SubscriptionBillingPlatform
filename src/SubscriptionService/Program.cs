using BuildingBlocks.Infrastructure.Handlers;
using BuildingBlocks.Infrastructure.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using SubscriptionService.Application.Behaviors;
using SubscriptionService.Application.Commands.CreateSubscription;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Persistence;
using SubscriptionService.Infrastructure.Repositories;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "AuthService",
            ValidAudience = "subscription-platform",
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("THIS_IS_A_SUPER_LONG_256_BIT_SECRET_KEY_1234567890"))
        };
    });



// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "sqlserver",
        timeout: TimeSpan.FromSeconds(5));


builder.Services.AddSingleton<SqlConnectionFactory>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(SubscriptionService.Application.Commands.CreateSubscription.CreateSubscriptionCommand)
            .Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(
    typeof(CreateSubscriptionCommand).Assembly);

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));



// ----------------- Build -----------------
var app = builder.Build();

// ----------------- Middleware pipeline -----------------

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