using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using React.AspNet;

namespace React.Services;

internal class ReactDotNetService : IReactService
{
    public async Task<IHtmlContent> RenderToStringAsync<T>(IHtmlHelper htmlHelper, string componentName, T props)
    {
        return htmlHelper.React(componentName, props);
    }

    public IHtmlContent InitJavascript(IHtmlHelper htmlHelper)
    {
        return htmlHelper.ReactInitJavaScript();
    }
}