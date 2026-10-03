# OTEL-experiments
experiment repository to play around and learn more about OTEL


## Aspire + OpenTelemetry Collector (contrib)

Prerequisites: .NET 9 SDK and Docker.

```
dotnet run --project src/AppHost --launch-profile http
```

- The Aspire dashboard opens at http://localhost:15180.
- `AppHost` starts the `otel/opentelemetry-collector-contrib` container (config: `otel-collector/config.yaml`)
  and the `WebApi` project. `WebApi` sends OTLP to the collector (`localhost:4317`), and the collector
  forwards traces, metrics and logs to the dashboard's OTLP endpoint.
- Hit the `webapi` endpoint (see dashboard "Resources") on `/` or `/weather` to generate telemetry.
