using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using React.AspNet;

namespace React.Services;

internal class ReactDotNetService : IReactService
{
    private readonly IHtmlHelper _htmlHelper;

    public ReactDotNetService(IHtmlHelper htmlHelper)
    {
        _htmlHelper = htmlHelper;
    }

    public async Task<IHtmlContent> RenderToStringAsync<T>(string componentName, T props)
    {
        return _htmlHelper.React(componentName, props);
    }

    public IHtmlContent InitJavascript()
    {
        return _htmlHelper.ReactInitJavaScript();
    }
}