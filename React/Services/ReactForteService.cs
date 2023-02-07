using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReactForte.Application;

namespace React.Services;

internal class ReactForteService : IReactService
{
    private readonly IHtmlService _htmlService;

    public ReactForteService(IHtmlService htmlService)
    {
        _htmlService = htmlService;
    }

    public async Task<IHtmlContent> RenderToStringAsync<T>(IHtmlHelper htmlHelper, string componentName, T props)
    {
        return await _htmlService.ReactAsync(componentName, props);
    }

    public IHtmlContent InitJavascript(IHtmlHelper htmlHelper)
    {
        return _htmlService.InitJavascript();
    }
}