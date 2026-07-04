using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DynamicLocalization.Core.Providers;

/// <summary>
/// Configuration options for YAML localization provider.
/// </summary>
public class YamlLocalizationProviderOptions
{
    /// <summary>
    /// Base path for YAML files. Default is "Localization".
    /// </summary>
    public string BasePath { get; set; } = "Localization";

    /// <summary>
    /// File pattern to match YAML files. Default is "*.yml".
    /// Supports both "*.yml" and "*.yaml" patterns.
    /// </summary>
    public string FilePattern { get; set; } = "*.yml";
}

/// <summary>
/// YAML localization provider that loads localized strings from YAML files or raw YAML strings.
/// </summary>
/// <remarks>
/// <para>
/// Supports loading from:
/// </para>
/// <list type="bullet">
///   <item><description>File system: Load from {BasePath}/{culture}.yml files</description></item>
///   <item><description>YAML strings: Load programmatically via <see cref="LoadYamlString"/> method</description></item>
/// </list>
/// <para>
/// Supports both flat and nested YAML formats:
/// </para>
/// <list type="bullet">
///   <item><description>Flat format: <c>App.Title: "My App"</c></description></item>
///   <item><description>Nested format: <c>App: Title: "My App"</c></description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // File system mode
/// services.AddYamlLocalization(options =>
/// {
///     options.BasePath = "Localization";
/// });
/// 
/// // Programmatic mode with YAML strings
/// var provider = new YamlLocalizationProvider();
/// provider.LoadYamlString("App:\n  Title: My App\n  Greeting: Hello", "en");
/// provider.LoadYamlString("App:\n  Title: 我的应用\n  Greeting: 你好", "zh-CN");
/// </code>
/// </example>
public class YamlLocalizationProvider : ILocalizationProvider<YamlLocalizationProviderOptions>
{
    protected readonly ConcurrentDictionary<string, Dictionary<string, string>> _cache = new();
    protected YamlLocalizationProviderOptions? _options;

    public virtual string Name => "Yaml";

    public void Initialize(YamlLocalizationProviderOptions options)
    {
        _options = options;
        LoadAll();
    }

    /// <summary>
    /// Loads all localization resources from file system.
    /// </summary>
    protected virtual void LoadAll()
    {
        if (_options == null) return;
        LoadFromFiles();
    }

    /// <summary>
    /// Loads YAML localization files from the file system.
    /// File naming format: {culture}.yml (or .yaml)
    /// Example: en.yml, zh-CN.yml, en.yaml, zh-CN.yaml
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

            // Handle double extension like "en.yaml" or "en.yml"
            // Path.GetFileNameWithoutExtension only strips the last extension
            // So "en.yaml" -> "en" is correct already
            // But "something.en.yaml" would be "something.en" - that's acceptable

            var yaml = File.ReadAllText(file);
            var dict = ParseYamlToFlatDictionary(yaml);
            if (dict != null)
            {
                _cache[cultureName] = dict;
            }
        }
    }

    /// <summary>
    /// Parses YAML content into a flat dictionary with dot-separated keys.
    /// Supports both flat and nested YAML formats.
    /// </summary>
    /// <param name="yaml">YAML string content</param>
    /// <returns>Dictionary with dot-separated keys, or empty dictionary if parsing fails</returns>
    /// <example>
    /// Flat format:
    /// <code>
    /// App.Title: My App
    /// App.Description: Hello
    /// </code>
    /// Nested format:
    /// <code>
    /// App:
    ///   Title: My App
    ///   Description: Hello
    /// </code>
    /// Both produce: {"App.Title": "My App", "App.Description": "Hello"}
    /// </example>
    protected virtual Dictionary<string, string>? ParseYamlToFlatDictionary(string yaml)
    {
        var result = new Dictionary<string, string>();

        try
        {
            var deserializer = new Deserializer();
            var yamlObject = deserializer.Deserialize<Dictionary<string, object?>>(yaml);

            if (yamlObject != null)
            {
                FlattenYamlObject(yamlObject, "", result);
            }
        }
        catch (YamlException)
        {
            // Return empty dictionary if parsing fails
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Recursively flattens a YAML object into a dictionary with dot-separated keys.
    /// Handles nested dictionaries, lists, and scalar values.
    /// </summary>
    protected virtual void FlattenYamlObject(Dictionary<string, object?> obj, string prefix, Dictionary<string, string> result)
    {
        foreach (var kvp in obj)
        {
            var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";

            if (kvp.Value is string stringValue)
            {
                result[key] = stringValue;
            }
            else if (kvp.Value is Dictionary<string, object?> nestedDict)
            {
                FlattenYamlObject(nestedDict, key, result);
            }
            else if (kvp.Value is Dictionary<object, object?> objectDict)
            {
                // Handle dictionaries with non-string keys (YamlDotNet may deserialize as Dictionary<object, object?>)
                var converted = new Dictionary<string, object?>();
                foreach (var item in objectDict)
                {
                    converted[item.Key?.ToString() ?? ""] = item.Value;
                }
                FlattenYamlObject(converted, key, result);
            }
            else if (kvp.Value is IList list)
            {
                // For lists, join values with comma
                var listValues = new List<string>();
                foreach (var item in list)
                {
                    listValues.Add(item?.ToString() ?? "");
                }
                result[key] = string.Join(", ", listValues);
            }
            else if (kvp.Value != null)
            {
                // Other scalar types (int, bool, etc.)
                result[key] = kvp.Value.ToString() ?? "";
            }
        }
    }

    /// <summary>
    /// Loads localization data from a raw YAML string for the specified culture.
    /// </summary>
    /// <param name="yaml">The YAML string containing localization key-value pairs.</param>
    /// <param name="cultureName">The culture name (e.g., "en", "zh-CN").</param>
    /// <example>
    /// <code>
    /// var provider = new YamlLocalizationProvider();
    /// provider.LoadYamlString(@"
    /// App:
    ///   Title: My App
    ///   Greeting: Hello
    /// ", "en");
    /// </code>
    /// </example>
    public void LoadYamlString(string yaml, string cultureName)
    {
        var dict = ParseYamlToFlatDictionary(yaml);
        if (dict != null)
        {
            _cache[cultureName] = dict;
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
