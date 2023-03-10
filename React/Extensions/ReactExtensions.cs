using Microsoft.AspNetCore.Builder;
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
using React.Abstraction;

namespace React.Extensions;

public class ReactOptions
{
    public bool EnableLoggingExecutionTimeReactServiceRenderToStringAsync { get; set; } = false;
}

public static class ReactExtensions
{
    public static void AddReactJsDotNet(this IServiceCollection services, 
        Action<ReactOptions> configureReact, 
        ILatencyMetricSender latencyMetricSender)
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
                    provider.GetRequiredService<ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator>>(),
                    latencyMetricSender));
        }
        else
        {
            services.AddScoped<IReactService, ReactDotNetService>();
        }

        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
            .AddV8();
    }

    public static void AddReactForte(this IServiceCollection services, 
        Action<ReactOptions> configureReact,
        Action<NodeJSProcessOptions> configureNodeJs, 
        ILatencyMetricSender latencyMetricSender)
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
                    provider.GetRequiredService<ILogger<ExecutionTimeReactServiceRenderToStringAsyncDecorator>>(),
                    latencyMetricSender));
        }
        else
        {
            services.AddScoped<IReactService, ReactForteService>();
        }
    }

    public static void UseReactJsDotNet(this IApplicationBuilder app, Action<IReactSiteConfiguration> configure)
    {
        ReactBuilderExtensions.UseReact(app, configure);
    }

    public static void UseReactForte(this IApplicationBuilder app, IEnumerable<string> scriptUrls, Version reactVersion,
        bool disableServerSideRendering = false)
    {
        ReactForteExtensions.UseReact(app, scriptUrls, reactVersion, disableServerSideRendering);
    }
}