var builder = DistributedApplication.CreateBuilder(args);

var dashboardOtlp = builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] ?? "http://localhost:18889";

if (!Uri.TryCreate(dashboardOtlp, UriKind.Absolute, out var dashboardOtlpUri) ||
    (dashboardOtlpUri.Scheme != Uri.UriSchemeHttp && dashboardOtlpUri.Scheme != Uri.UriSchemeHttps))
{
    throw new InvalidOperationException(
        "ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL must be an absolute HTTP or HTTPS URL.");
}

var dashboardOtlpFromContainer = new UriBuilder(dashboardOtlpUri)
{
    Host = "host.docker.internal"
}.Uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);

var dashboardUsesTls = dashboardOtlpUri.Scheme == Uri.UriSchemeHttps;

var collector = builder.AddContainer("otel-collector", "otel/opentelemetry-collector-contrib", "0.164.0")
    .WithBindMount("../../otel-collector/config.yaml", "/etc/otelcol-contrib/config.yaml", isReadOnly: true)
    .WithEnvironment("DASHBOARD_OTLP_ENDPOINT", dashboardOtlpFromContainer)
    .WithEnvironment("DASHBOARD_OTLP_TLS_INSECURE", (!dashboardUsesTls).ToString().ToLowerInvariant())
    .WithEnvironment("DASHBOARD_OTLP_TLS_INSECURE_SKIP_VERIFY", dashboardUsesTls.ToString().ToLowerInvariant())
    .WithEndpoint(port: 4317, targetPort: 4317, name: "otlp-grpc", scheme: "http");

builder.AddDotnetProject("webapi", "../WebApi/WebApi.csproj")
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")
    .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
    .WaitFor(collector);

builder.Build().Run();
