using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ReactForte.Application.Webpack;

public class WebpackManifest
{
    private readonly WebpackOptions _webpackOptions;
    private readonly Lazy<IReadOnlyDictionary<string, string>> _localInstance;

    public string ManifestPath => $"{_webpackOptions.OutputPath}/manifest.json";

    public IReadOnlyDictionary<string, string> Instance => _localInstance.Value;

    public WebpackManifest(IOptions<WebpackOptions> webpackOptions, string contentRootPath)
    {
        _webpackOptions = webpackOptions.Value;
        IManifestLoader manifestLoader = _webpackOptions.IsDevServer
            ? new DevServerLoader(ManifestPath, _webpackOptions.OutputPath)
            : new LocalLoader(contentRootPath, _webpackOptions.OutputPath, ManifestPath);
        _localInstance = new Lazy<IReadOnlyDictionary<string, string>>(() => manifestLoader.LoadAsync().Result);
    }

    private static IEnumerable<string> FilterUrls(IEnumerable<string> urls, IEnumerable<string> pattern,
        IEnumerable<string>? exclude = null)
    {
        return urls.Where(v => string.IsNullOrEmpty(v) == false)
            .Where(v => pattern.Any(v.Contains))
            .Where(v => exclude == null || exclude.Any(v.Contains) == false);
    }

    public IEnumerable<string> GetAllStyleUrls(IEnumerable<string> pattern, IEnumerable<string>? exclude = null)
    {
        return FilterUrls(Instance.Values.Where(v => v.EndsWith(".css")), pattern, exclude);
    }

    public IEnumerable<string> GetAllScriptUrls(IEnumerable<string> pattern, IEnumerable<string>? exclude = null)
    {
        return FilterUrls(Instance.Values.Where(v => v.EndsWith(".js")), pattern, exclude);
    }

    private interface IManifestLoader
    {
        Task<IReadOnlyDictionary<string, string>> LoadAsync();
    }

    private class DevServerLoader : IManifestLoader
    {
        private readonly string _manifestPath;
        private readonly string _outputPath;
        private readonly HttpClient _httpClient = new();

        public DevServerLoader(string manifestPath, string outputPath)
        {
            _manifestPath = manifestPath;
            _outputPath = outputPath;
        }

        public async Task<IReadOnlyDictionary<string, string>> LoadAsync()
        {
            var getManifestPathResponse = await _httpClient.GetAsync(_manifestPath);

            var manifestContent = await getManifestPathResponse.Content.ReadAsStringAsync();

            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(manifestContent);
            return dic.ToDictionary(p => p.Key, p =>
                $"{_outputPath}/{p.Value}");
        }
    }

    private class LocalLoader : IManifestLoader
    {
        private readonly string _contentRootPath;
        private readonly string _outputPath;
        private readonly string _manifestPath;

        public LocalLoader(string contentRootPath, string outputPath, string manifestPath)
        {
            _contentRootPath = contentRootPath;
            _outputPath = outputPath;
            _manifestPath = manifestPath;
        }

        public async Task<IReadOnlyDictionary<string, string>> LoadAsync()
        {
            var manifestAbsolutePath = Path.Combine(
                _contentRootPath,
                _manifestPath);

            if (string.IsNullOrEmpty(manifestAbsolutePath))
            {
                throw new InvalidOperationException("Unable to get webpack manifest file path");
            }

            if (File.Exists(manifestAbsolutePath) == false)
            {
                throw new InvalidOperationException("Webpack manifest file does not exist");
            }

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                MaxDepth = 128,
            };

            return (JsonConvert.DeserializeObject<Dictionary<string, string>>(await File.ReadAllTextAsync(manifestAbsolutePath),
                serializerSettings) ?? throw new InvalidOperationException()).ToDictionary(p => p.Key, p =>
                $"~/.{_outputPath}/{p.Value}"); //TODO: should we copy dist/client/* to wwwroot
        }
    }
}