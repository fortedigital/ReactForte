using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using React.Services;

namespace Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlHelperExtensions
{
    public static async Task<IHtmlContent> ReactAsync<T>(this IHtmlHelper htmlHelper, string componentName, T props)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return await reactService.RenderToStringAsync(componentName, props);
    }

    public static IHtmlContent InitJavascript(this IHtmlHelper htmlHelper)
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return reactService.InitJavascript();
    }
}