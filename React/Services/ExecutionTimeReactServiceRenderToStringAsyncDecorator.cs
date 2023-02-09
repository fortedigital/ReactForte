using System.Diagnostics;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Reflection;

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

    public async Task<IHtmlContent> RenderToStringAsync<T>(IHtmlHelper htmlHelper, string componentName, T props)
    {
        var watch = Stopwatch.StartNew();
        var renderedString = await _service.RenderToStringAsync(htmlHelper, componentName, props);
        var elapsedMs = watch.ElapsedMilliseconds;

        _logger.Log(LogLevel.Debug, "{methodName} for component {componentName} lasted: {elapsedMs}ms",
            nameof(RenderToStringAsync), componentName, elapsedMs);

        return renderedString;
    }

    public IHtmlContent InitJavascript(IHtmlHelper htmlHelper)
    {
        return _service.InitJavascript(htmlHelper);
    }
}