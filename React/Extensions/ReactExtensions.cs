﻿using Microsoft.AspNetCore.Builder;
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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace React.Extensions;

public static class ReactExtensions
{
    public static void AddReact(
        this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        ReactServiceCollectionExtensions.AddReact(services);

        services.AddScoped<ReactDotNetService>();
        services.AddScoped<IReactService>(provider =>
            new ReactServiceMethodExecutionTimeDecorator(provider.GetRequiredService<ReactDotNetService>(), provider.GetRequiredService<ILogger<ReactServiceMethodExecutionTimeDecorator>>()));
        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
            .AddV8();
    }

    public static void AddReact(
        this IServiceCollection services, Action<NodeJSProcessOptions> configureNodeJs)
    {
        ReactForteExtensions.AddReact(services, configureNodeJs);

        services.AddScoped<ReactForteService>();
        services.AddScoped<IReactService>(provider =>
            new ReactServiceMethodExecutionTimeDecorator(provider.GetRequiredService<ReactForteService>(), provider.GetRequiredService<ILogger<ReactServiceMethodExecutionTimeDecorator>>()));
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