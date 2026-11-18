# OTEL-experiments
experiment repository to play around and learn more about OTEL


## Aspire + OpenTelemetry Collector (contrib)

Prerequisites: .NET 9 SDK and Docker.

Generate an OTLP API key for the current terminal before starting the AppHost:

```powershell
$env:Dashboard__Otlp__PrimaryApiKey = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
dotnet run --project src/AppHost --launch-profile http
```

- The Aspire dashboard opens at http://localhost:15180.
- `AppHost` starts the `otel/opentelemetry-collector-contrib` container (config: `otel-collector/config.yaml`)
  and the `WebApi` project. `WebApi` sends OTLP to the collector (`localhost:4317`), and the collector
  forwards traces, metrics and logs to the dashboard's OTLP endpoint. The dashboard's OTLP listener binds
  to all host interfaces so the collector can reach it through `host.docker.internal`; the HTTP launch
  profile uses an unencrypted local connection, while the HTTPS profile uses TLS and accepts the local
  development certificate. Both profiles allow anonymous dashboard access for local development only.
  Do not use this configuration outside local development; enable dashboard authentication and mount a
  trusted CA certificate for shared or production environments.
- Hit the `webapi` endpoint (see dashboard "Resources") on `/` or `/weather` to generate telemetry.

### Tail sampling

The collector tail-samples only traces: it retains all error traces, traces taking at least one second,
and 10% of all other traces. It waits up to 10 seconds to make each decision and can buffer 50,000
in-flight traces. Metrics and logs are not sampled.

Tail sampling keeps trace attributes in collector memory until a decision is made. Do not add sensitive
data to telemetry attributes; this local-development setup forwards retained traces only to the Aspire
dashboard. (ASVS 14.2.4)
