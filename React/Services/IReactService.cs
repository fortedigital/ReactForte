using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace React.Services;

internal class ReactServiceRenderToStringAsyncExecutionTimeDecorator : IReactService
{
    private readonly IReactService _service;
    private readonly ILogger<ReactServiceRenderToStringAsyncExecutionTimeDecorator> _logger;
    
    public ReactServiceRenderToStringAsyncExecutionTimeDecorator(IReactService service, ILogger<ReactServiceRenderToStringAsyncExecutionTimeDecorator> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IHtmlContent> RenderToStringAsync<T>(IHtmlHelper htmlHelper, string componentName, T props)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();    
        var renderedString = await _service.RenderToStringAsync(htmlHelper, componentName, props);
        var elapsedMs = watch.ElapsedMilliseconds;

        _logger.Log(LogLevel.Debug, $"RenderToStringAsync for component {componentName} lasted: {elapsedMs}");

        return renderedString;
    }

    public IHtmlContent InitJavascript(IHtmlHelper htmlHelper)
    {
        return _service.InitJavascript(htmlHelper);
    }
}