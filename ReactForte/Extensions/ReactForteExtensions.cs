using System;
using System.Collections.Generic;
using System.Linq;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ReactForte.Application;
using ReactForte.Infrastructure;

namespace ReactForte.Extensions;

public static class ReactForteExtensions
{
    public static void AddReact(this IServiceCollection services, Action<NodeJSProcessOptions>? configureNodeJs = null)
    {
        services.AddNodeJS();

        services.Configure<NodeJSProcessOptions>(options => { configureNodeJs?.Invoke(options); });
        services.Configure<OutOfProcessNodeJSServiceOptions>(options =>
        {
            options.Concurrency = Concurrency.MultiProcess;
        });
        services.AddSingleton<Config>();
        services.AddScoped<IReactService, ReactService>();
        services.AddScoped<IHtmlService, HtmlService>();
    }

    public static void UseReact(this IApplicationBuilder app, IEnumerable<string> scriptUrls, Version reactVersion, bool disableServerSideRendering = false)
    {
        var config = app.ApplicationServices.GetService<Config>();

        if (config is null)
        {
            throw new InvalidOperationException("Remember to add AddReact to your code");
        }

        config.IsServerSideDisabled = disableServerSideRendering;
        config.ScriptUrls = scriptUrls.ToList();
        config.ReactVersion = reactVersion;
    }
}