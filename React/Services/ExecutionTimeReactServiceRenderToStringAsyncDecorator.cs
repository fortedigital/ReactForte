using System.Diagnostics;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using React.Abstraction;

namespace React.Services;

internal class ExecutionTimeReactServiceRenderToStringAsyncDecorator : IReactService
{
    private readonly IReactService _service;
    private readonly ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator> _logger;
    private readonly ILatencyMetricSender _latencyMetricSender;

    public ExecutionTimeReactServiceRenderToStringAsyncDecorator(IReactService service,
        ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator> logger,
        ILatencyMetricSender latencyMetricSender)
    {
        _service = service;
        _logger = logger;
        _latencyMetricSender = latencyMetricSender;
    }

    public async Task<IHtmlContent> RenderToStringAsync<T>(string componentName, T props)
    {
        var watch = Stopwatch.StartNew();
        var renderedString = await _service.RenderToStringAsync(componentName, props);
        var elapsedMs = watch.ElapsedMilliseconds;

        _logger.Log(LogLevel.Debug, "{implementation} - {methodName} for component {componentName} lasted: {elapsedMs}ms",
            _service.GetType().Name, nameof(RenderToStringAsync), componentName, elapsedMs);

        _latencyMetricSender.Send(nameof(RenderToStringAsync), elapsedMs);

        return renderedString;
    }

    public IHtmlContent InitJavascript()
    {
        return _service.InitJavascript();
    }
}