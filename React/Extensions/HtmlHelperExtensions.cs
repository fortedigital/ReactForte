using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using React.Services;

namespace React.Extensions;

public static class HtmlHelperExtensions
{
    public static async Task<IHtmlContent> ReactAsync<T>(
        this IHtmlHelper htmlHelper,
        string componentName,
        T props
    )
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return await reactService.RenderToStringAsync(htmlHelper, componentName, props);
    }

    public static IHtmlContent InitJavascript(
        this IHtmlHelper htmlHelper
    )
    {
        var reactService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IReactService>();

        return reactService.InitJavascript(htmlHelper);
    }
}