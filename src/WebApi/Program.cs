using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Endpoint/protocol come from OTEL_EXPORTER_OTLP_* env vars set by the AppHost (pointing at the collector).
builder.Logging.AddOpenTelemetry(o =>
{
    o.IncludeFormattedMessage = true;
    o.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("webapi"))
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation())
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
    .UseOtlpExporter();

var app = builder.Build();

app.MapGet("/", () => "Hello from webapi!");
app.MapGet("/weather", (ILogger<Program> logger) =>
{
    logger.LogInformation("Weather requested");
    return new[] { "Sunny", "Cloudy", "Rainy" };
});

app.Run();
