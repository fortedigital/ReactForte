using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using ReactForte.Application;

namespace React.Services;

internal class ReactForteService : IReactService
{
    private readonly IHtmlService _htmlService;

    public ReactForteService(IHtmlService htmlService)
    {
        _htmlService = htmlService;
    }

    public async Task<IHtmlContent> RenderToStringAsync<T>( string componentName, T props)
    {
        return await _htmlService.ReactAsync(componentName, props);
    }

    public IHtmlContent InitJavascript()
    {
        return _htmlService.InitJavascript();
    }
}