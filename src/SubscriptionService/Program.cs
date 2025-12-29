using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Serilog bootstrap
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddHttpLogging(_ => { });



builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health/live", () => "Alive");
app.MapGet("/health/ready", () => "Ready");
app.Run();
