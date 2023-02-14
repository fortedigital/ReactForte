using System.Diagnostics;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;

namespace React.Services;

internal class ExecutionTimeReactServiceRenderToStringAsyncDecorator : IReactService
{
    private readonly IReactService _service;
    private readonly ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator> _logger;
    private readonly TelemetryClient _telemetry;

    public ExecutionTimeReactServiceRenderToStringAsyncDecorator(IReactService service,
        ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator> logger,
        TelemetryClient telemetry)
    {
        _service = service;
        _logger = logger;
        _telemetry = telemetry;
    }

    public async Task<IHtmlContent> RenderToStringAsync<T>(string componentName, T props)
    {
        var watch = Stopwatch.StartNew();
        var renderedString = await _service.RenderToStringAsync(componentName, props);
        var elapsedMs = watch.ElapsedMilliseconds;

        _logger.Log(LogLevel.Debug, "{methodName} for component {componentName} lasted: {elapsedMs}ms",
            nameof(RenderToStringAsync), componentName, elapsedMs);

        TrackLatencyMetric(nameof(RenderToStringAsync), elapsedMs);

        return renderedString;
    }

    private void TrackLatencyMetric(string name, long elapsedMs)
    {
        var latency = new MetricTelemetry
        {
            Name = $"{name}Latency",
            Sum = elapsedMs
        };
        _telemetry.TrackMetric(latency);
    }

    public IHtmlContent InitJavascript()
    {
        return _service.InitJavascript();
    }
}