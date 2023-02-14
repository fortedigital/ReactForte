using System.Diagnostics;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace React.Services;

internal class ExecutionTimeReactServiceRenderToStringAsyncDecorator : IReactService
{
    private readonly IReactService _service;
    private readonly ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator> _logger;

    public ExecutionTimeReactServiceRenderToStringAsyncDecorator(IReactService service,
        ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IHtmlContent> RenderToStringAsync<T>(string componentName, T props)
    {
        var watch = Stopwatch.StartNew();
        var renderedString = await _service.RenderToStringAsync(componentName, props);
        var elapsedMs = watch.ElapsedMilliseconds;

        _logger.Log(LogLevel.Debug, "{implementation} - {methodName} for component {componentName} lasted: {elapsedMs}ms",
            _service.GetType().Name, nameof(RenderToStringAsync), componentName, elapsedMs);

        return renderedString;
    }

    public IHtmlContent InitJavascript()
    {
        return _service.InitJavascript();
    }
}