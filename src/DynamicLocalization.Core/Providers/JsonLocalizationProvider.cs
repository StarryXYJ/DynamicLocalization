using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicLocalization.Core.Providers;

/// <summary>
/// Configuration options for JSON localization provider.
/// </summary>
public class JsonLocalizationProviderOptions
{
    /// <summary>
    /// Base path for JSON files. Default is "Localization".
    /// </summary>
    public string BasePath { get; set; } = "Localization";

    /// <summary>
    /// File pattern to match. Default is "*.json".
    /// </summary>
    public string FilePattern { get; set; } = "*.json";

    /// <summary>
    /// Whether to use embedded resources instead of file system. Default is false.
    /// </summary>
    public bool UseEmbeddedResources { get; set; } = false;

    /// <summary>
    /// Assembly containing embedded resources. Default is the calling assembly.
    /// </summary>
    public Assembly? Assembly { get; set; }
}

/// <summary>
/// JSON localization provider that loads localized strings from JSON files or embedded resources.
/// </summary>
/// <remarks>
/// <para>
/// Supports two modes:
/// </para>
/// <list type="bullet">
///   <item><description>File system: Load from {BasePath}/{culture}.json files</description></item>
///   <item><description>Embedded resources: Load from {Assembly}.Localization.{culture}.json resources</description></item>
/// </list>
/// <para>
/// Supports two JSON formats:
/// </para>
/// <list type="bullet">
///   <item><description>Flat format: {"App.Title": "My App"}</description></item>
///   <item><description>Nested format: {"App": {"Title": "My App"}}</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // File system mode
/// services.AddJsonLocalization(options =>
/// {
///     options.BasePath = "Localization";
/// });
/// 
/// // Embedded resource mode
/// services.AddJsonLocalization(options =>
/// {
///     options.UseEmbeddedResources = true;
///     options.Assembly = typeof(App).Assembly;
/// });
/// </code>
/// </example>
public class JsonLocalizationProvider : ILocalizationProvider<JsonLocalizationProviderOptions>
{
    protected readonly ConcurrentDictionary<string, Dictionary<string, string>> _cache = new();
    protected JsonLocalizationProviderOptions? _options;

    public virtual string Name => "Json";

    public void Initialize(JsonLocalizationProviderOptions options)
    {
        _options = options;
        LoadAll();
    }

    /// <summary>
    /// Loads all localization resources.
    /// </summary>
    protected virtual void LoadAll()
    {
        if (_options == null) return;

        if (_options.UseEmbeddedResources)
        {
            LoadFromEmbeddedResources();
        }
        else
        {
            LoadFromFiles();
        }
    }

    /// <summary>
    /// Loads JSON localization files from embedded resources.
    /// Resource naming format: {Assembly}.Localization.{culture}.json
    /// Example: AvaloniaLab.Localization.en.json
    /// </summary>
    protected virtual void LoadFromEmbeddedResources()
    {
        var assembly = _options!.Assembly ?? Assembly.GetCallingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();

        foreach (var name in resourceNames)
        {
            if (!name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                continue;

            var cultureName = ExtractCultureName(name);
            if (string.IsNullOrEmpty(cultureName))
            {
                System.Diagnostics.Debug.WriteLine($"[JsonProvider] Failed to extract culture from: {name}");
                continue;
            }

            System.Diagnostics.Debug.WriteLine($"[JsonProvider] Loading: {name} -> Culture: {cultureName}");

            using var stream = assembly.GetManifestResourceStream(name);
            if (stream == null) continue;

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var dict = ParseJsonToFlatDictionary(json);
            if (dict != null)
            {
                _cache[cultureName] = dict;
                System.Diagnostics.Debug.WriteLine($"[JsonProvider] Loaded {dict.Count} strings for culture: {cultureName}");
            }
        }
    }

    /// <summary>
    /// Extracts culture name from embedded resource name.
    /// Example: AvaloniaLab.Localization.en.json -> en
    ///          AvaloniaLab.Localization.zh-CN.json -> zh-CN
    /// </summary>
    protected virtual string? ExtractCultureName(string resourceName)
    {
        var parts = resourceName.Split('.');

        for (int i = 0; i < parts.Length; i++)
        {
            if (string.Equals(parts[i], "Localization", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < parts.Length && parts[i + 1] != "json")
                {
                    return parts[i + 1];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Loads JSON localization files from the file system.
    /// File naming format: {culture}.json
    /// Example: en.json, zh-CN.json
    /// </summary>
    protected virtual void LoadFromFiles()
    {
        var basePath = _options!.BasePath;

        if (!Path.IsPathRooted(basePath))
        {
            basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath);
        }

        if (!Directory.Exists(basePath))
        {
            return;
        }

        var files = Directory.GetFiles(basePath, _options.FilePattern);

        foreach (var file in files)
        {
            var cultureName = Path.GetFileNameWithoutExtension(file);
            var json = File.ReadAllText(file);
            var dict = ParseJsonToFlatDictionary(json);
            if (dict != null)
            {
                _cache[cultureName] = dict;
            }
        }
    }

    /// <summary>
    /// Parses JSON content into a flat dictionary with dot-separated keys.
    /// Supports both flat and nested JSON formats.
    /// </summary>
    /// <param name="json">JSON string content</param>
    /// <returns>Dictionary with dot-separated keys, or null if parsing fails</returns>
    /// <example>
    /// Flat format: {"App.Title": "My App"} -> {"App.Title": "My App"}
    /// Nested format: {"App": {"Title": "My App"}} -> {"App.Title": "My App"}
    /// </example>
    protected virtual Dictionary<string, string>? ParseJsonToFlatDictionary(string json)
    {
        var result = new Dictionary<string, string>();
        
        try
        {
            var node = JsonNode.Parse(json);
            if (node is JsonObject rootObject)
            {
                FlattenJsonObject(rootObject, "", result);
            }
            else
            {
                var legacyDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                return legacyDict;
            }
        }
        catch (JsonException)
        {
            var legacyDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return legacyDict;
        }

        return result;
    }

    /// <summary>
    /// Recursively flattens a JSON object into a dictionary with dot-separated keys.
    /// </summary>
    /// <param name="obj">JSON object to flatten</param>
    /// <param name="prefix">Current key prefix</param>
    /// <param name="result">Result dictionary to populate</param>
    protected virtual void FlattenJsonObject(JsonObject obj, string prefix, Dictionary<string, string> result)
    {
        foreach (var property in obj)
        {
            var key = string.IsNullOrEmpty(prefix) ? property.Key : $"{prefix}.{property.Key}";
            
            if (property.Value is JsonValue jsonValue && jsonValue.TryGetValue(out string? stringValue))
            {
                result[key] = stringValue;
            }
            else if (property.Value is JsonObject nestedObject)
            {
                FlattenJsonObject(nestedObject, key, result);
            }
        }
    }

    /// <summary>
    /// Gets all available cultures.
    /// </summary>
    public IEnumerable<CultureInfo> GetAvailableCultures()
    {
        return _cache.Keys.Select(k => new CultureInfo(k));
    }

    /// <summary>
    /// Gets the localized string for the specified key and culture.
    /// Supports culture fallback: if exact match not found, tries parent culture.
    /// Example: zh-CN -> zh -> default
    /// </summary>
    public string? GetString(string key, CultureInfo culture)
    {
        if (TryGetFromCulture(key, culture, out var value))
        {
            return value;
        }

        if (!string.IsNullOrEmpty(culture.Parent?.Name) && TryGetFromCulture(key, culture.Parent, out value))
        {
            return value;
        }

        return null;
    }

    protected virtual bool TryGetFromCulture(string key, CultureInfo culture, out string? value)
    {
        value = null;
        if (_cache.TryGetValue(culture.Name, out var dict))
        {
            return dict.TryGetValue(key, out value);
        }
        return false;
    }

    public bool TryGetString(string key, CultureInfo culture, out string? value)
    {
        value = GetString(key, culture);
        return value != null;
    }

    /// <summary>
    /// Reloads all localization resources.
    /// </summary>
    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        _cache.Clear();
        await Task.Run(LoadAll, cancellationToken);
    }
}
