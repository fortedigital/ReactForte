using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactForte.Extensions;
using System.Collections.Generic;
using System;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using React.AspNet;
using Jering.Javascript.NodeJS;
using React.Services;
using Microsoft.Extensions.Options;
using ReactForte.Application.Webpack;
using Microsoft.AspNetCore.Http;

namespace React.Extensions;

public static class ReactExtensions
{
    public static void AddReact(
        this IServiceCollection services, IConfiguration configuration,
        string rootPath)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        ReactServiceCollectionExtensions.AddReact(services);
        services.AddScoped<IReactService, ReactDotNetService>();
        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
            .AddV8();

        services.Configure<WebpackOptions>(configuration.GetSection("Webpack"));
        services.AddSingleton(provider =>
            new WebpackManifest(provider.GetRequiredService<IOptions<WebpackOptions>>(), rootPath));
    }

    public static void AddReact(
        this IServiceCollection services, IConfiguration configuration,
        string rootPath, Action<NodeJSProcessOptions> configureNodeJs)
    {
        ReactForteExtensions.AddReact(services, configuration, rootPath, configureNodeJs);

        services.AddScoped<IReactService, ReactForteService>();
    }

    public static void UseReact(
        this IApplicationBuilder app,
        IEnumerable<string> scriptUrls, Version reactVersion, bool disableServerSideRendering = false)
    {
        ReactForteExtensions.UseReact(app, scriptUrls, reactVersion, disableServerSideRendering);
    }

    public static void UseReact(
        this IApplicationBuilder app,
        Action<IReactSiteConfiguration> configure)
    {
        ReactBuilderExtensions.UseReact(app, configure);
    }
}