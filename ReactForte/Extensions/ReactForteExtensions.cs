using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Jering.Javascript.NodeJS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReactForte.Application;
using ReactForte.Application.Webpack;
using ReactForte.Infrastructure;

namespace ReactForte.Extensions;

public static class ReactForteExtensions
{
    public static void AddReact(
        this IServiceCollection services, IConfiguration configuration, string rootPath, Action<NodeJSProcessOptions>? configureNodeJs = null)
    {
        services.AddNodeJS();

        var fullFilePath = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().GetName().CodeBase)?[6..] ?? string.Empty;

        services.Configure<NodeJSProcessOptions>(options =>
        {
            configureNodeJs?.Invoke(options);
            options.ProjectPath = fullFilePath;
        });
        services.AddSingleton<Config>();
        services.AddScoped<IReactService, ReactService>();
        services.AddScoped<IHtmlService, HtmlService>();

        services.Configure<WebpackOptions>(configuration.GetSection("Webpack"));
        services.AddSingleton(provider =>
            new WebpackManifest(provider.GetRequiredService<IOptions<WebpackOptions>>(), rootPath));
    }

    public static void UseReact(
        this IApplicationBuilder app,
        IEnumerable<string> scriptUrls, Version reactVersion, bool disableServerSideRendering = false)
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