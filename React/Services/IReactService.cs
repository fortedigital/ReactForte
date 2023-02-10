using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace React.Services;

internal interface IReactService
{
    Task<IHtmlContent> RenderToStringAsync<T>(string componentName, T props);
    IHtmlContent InitJavascript();
}