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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace React.Extensions;

public class ReactOptions
{
    public bool EnableLoggingExecutionTimeReactServiceRenderToStringAsync { get; set; } = false;
}

public static class ReactExtensions
{
    public static void AddReact(
        this IServiceCollection services, Action<ReactOptions> configureReact)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        ReactServiceCollectionExtensions.AddReact(services);

        var reactOptions = new ReactOptions();

        configureReact(reactOptions);

        if (reactOptions.EnableLoggingExecutionTimeReactServiceRenderToStringAsync)
        {
            services.AddScoped<ReactDotNetService>();
            services.AddScoped<IReactService>(provider =>
                new ExecutionTimeReactServiceRenderToStringAsyncDecorator(
                    provider.GetRequiredService<ReactDotNetService>(),
                    provider.GetRequiredService<ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator>>()));
        }
        else
        {
            services.AddScoped<IReactService, ReactDotNetService>();
        }

        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
            .AddV8();
    }

    public static void AddReact(
        this IServiceCollection services, Action<ReactOptions> configureReact,
        Action<NodeJSProcessOptions> configureNodeJs)
    {
        ReactForteExtensions.AddReact(services, configureNodeJs);

        var reactOptions = new ReactOptions();

        configureReact(reactOptions);

        if (reactOptions.EnableLoggingExecutionTimeReactServiceRenderToStringAsync)
        {
            services.AddScoped<ReactForteService>();
            services.AddScoped<IReactService>(provider =>
                new ExecutionTimeReactServiceRenderToStringAsyncDecorator(
                    provider.GetRequiredService<ReactForteService>(),
                    provider.GetRequiredService<ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator>>()));
        }
        else
        {
            services.AddScoped<IReactService, ReactForteService>();
        }
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