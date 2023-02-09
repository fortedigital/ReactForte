using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace React.Services;

internal interface IReactService
{
    Task<IHtmlContent> RenderToStringAsync<T>(IHtmlHelper htmlHelper, string componentName, T props);
    IHtmlContent InitJavascript(IHtmlHelper htmlHelper);
}