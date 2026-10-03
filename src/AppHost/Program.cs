var builder = DistributedApplication.CreateBuilder(args);

// The collector receives OTLP from the apps (host port 4317) and forwards it to the
// Aspire dashboard's OTLP endpoint (see otel-collector/config.yaml).
var dashboardOtlp = builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] ?? "http://localhost:18889";
var dashboardOtlpFromContainer = dashboardOtlp
    .Replace("localhost", "host.docker.internal")
    .Replace("127.0.0.1", "host.docker.internal");

var collector = builder.AddContainer("otel-collector", "otel/opentelemetry-collector-contrib", "latest")
    .WithBindMount("../../otel-collector/config.yaml", "/etc/otelcol-contrib/config.yaml", isReadOnly: true)
    .WithEnvironment("DASHBOARD_OTLP_ENDPOINT", dashboardOtlpFromContainer)
    .WithContainerRuntimeArgs("--add-host=host.docker.internal:host-gateway")
    .WithEndpoint(port: 4317, targetPort: 4317, name: "otlp-grpc", scheme: "http");

builder.AddProject<Projects.WebApi>("webapi")
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")
    .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
    .WaitFor(collector);

builder.Build().Run();
