﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;
using ReactForte.Infrastructure;

namespace ReactForte.Application;

internal interface IReactService
{
    Task<string> RenderToStringAsync(string componentName, object props);
    string GetInitJavascript();
}

internal class ReactService : IReactService
{
    private List<Component> Components { get; } = new();

    private readonly INodeJSService _nodeJsService;
    private readonly Config _config;
    private const string RenderToStringCacheIdentifier = nameof(RenderToStringAsync);

    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ReactService(INodeJSService nodeJsService, Config config)
    {
        _nodeJsService = nodeJsService;
        _config = config;
    }

    public async Task<string> RenderToStringAsync(string componentName, object props)
    {
        var component = new Component(componentName, props);
        Components.Add(component);

        var args = new[] { componentName, props, _config.ScriptUrls };

        var (success, cachedResult) =
            await _nodeJsService.TryInvokeFromCacheAsync<string>(RenderToStringCacheIdentifier, args: args);

        if (success)
        {
            return WrapRenderedStringComponent(cachedResult, component);
        }

        var currentAssembly = typeof(ReactService).Assembly;
        var renderToStringScriptManifestName = currentAssembly.GetManifestResourceNames().Single();

        Stream ModuleFactory()
        {
            return currentAssembly.GetManifestResourceStream(renderToStringScriptManifestName) ??
                   throw new InvalidOperationException(
                       $"Can not get manifest resource stream with name - {renderToStringScriptManifestName}");
        }

        var result = await _nodeJsService.InvokeFromStreamAsync<string>(ModuleFactory,
            RenderToStringCacheIdentifier,
            args: args);

        return WrapRenderedStringComponent(result, component);
    }

    private static string WrapRenderedStringComponent(string? renderedStringComponent, Component component)
    {
        if (renderedStringComponent is null)
        {
            throw new ArgumentNullException(nameof(renderedStringComponent));
        }

        return $"<div id=\"{component.ContainerId}\">{renderedStringComponent}</div>";
    }

    public string GetInitJavascript()
    {
        Func<Component, string> initFunction = _config.IsServerSideDisabled ? Render : Hydrate;
        var componentInitiation = Components.Select(initFunction);

        return $"<script>{string.Join("", componentInitiation)}</script>";
    }

    private string GetElementById(string containerId)
    {
        return $"document.getElementById(\"{containerId}\")";
    }

    private string CreateElement(Component component)
    {
        return $"React.createElement({component.Name}, {JsonSerializer.Serialize(component.Props, _serializeOptions)})";
    }


    private string Render(Component component)
    {
        return _config.ReactVersion.Major < 18
            ? $"ReactDOM.render({CreateElement(component)}, {GetElementById(component.ContainerId)});"
            : $"ReactDOMClient.createRoot({GetElementById(component.ContainerId)}).render({CreateElement(component)});";
    }

    private string Hydrate(Component component)
    {
        return _config.ReactVersion.Major < 18
            ? $"ReactDOM.hydrate({CreateElement(component)}, {GetElementById(component.ContainerId)});"
            : $"ReactDOMClient.hydrateRoot({GetElementById(component.ContainerId)}, {CreateElement(component)});";
    }
}