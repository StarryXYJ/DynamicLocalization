using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicLocalization.Core.Providers;

/// <summary>
/// Configuration options for RESX localization provider.
/// </summary>
public class ResxLocalizationProviderOptions
{
    /// <summary>
    /// RESX resource type (Visual Studio auto-generated resource class).
    /// </summary>
    /// <example>
    /// <code>
    /// options.ResourceType = typeof(Resources.Strings);
    /// </code>
    /// </example>
    public Type ResourceType { get; set; } = null!;

    /// <summary>
    /// Whether to auto-detect available cultures. Default is true.
    /// </summary>
    public bool AutoDetectCultures { get; set; } = true;

    /// <summary>
    /// Known cultures list for manually specifying supported languages.
    /// </summary>
    public string[]? KnownCultures { get; set; }
}

/// <summary>
/// RESX localization provider that loads localized strings from .resx resource files.
/// </summary>
/// <remarks>
/// <para>
/// RESX file naming convention:
/// </para>
/// <list type="bullet">
///   <item><description>Default resource: Strings.resx</description></item>
///   <item><description>Chinese resource: Strings.zh-CN.resx</description></item>
///   <item><description>English resource: Strings.en.resx</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// services.AddResxLocalization(options =>
/// {
///     options.ResourceType = typeof(Resources.Strings);
/// });
/// </code>
/// </example>
public class ResxLocalizationProvider : ILocalizationProvider<ResxLocalizationProviderOptions>
{
    protected ResourceManager? _resourceManager;
    protected ResxLocalizationProviderOptions? _options;
    protected List<CultureInfo>? _availableCultures;

    public virtual string Name => "Resx";

    public void Initialize(ResxLocalizationProviderOptions options)
    {
        _options = options;
        _resourceManager = new ResourceManager(options.ResourceType);
        DetectAvailableCultures();
    }

    /// <summary>
    /// Detects available cultures from the resource assembly.
    /// </summary>
    protected virtual void DetectAvailableCultures()
    {
        if (_options?.ResourceType == null || _resourceManager == null)
        {
            _availableCultures = new List<CultureInfo>();
            return;
        }

        var assembly = _options.ResourceType.Assembly;
        var cultures = new List<CultureInfo>();

        if (_options.AutoDetectCultures)
        {
            var baseName = _options.ResourceType.FullName;
            if (baseName != null)
            {
                var resourceNames = assembly.GetManifestResourceNames();
                var resourcePrefix = baseName.Replace('.', '_') + ".";

                foreach (var name in resourceNames)
                {
                    if (name.StartsWith(resourcePrefix) && name.EndsWith(".resources"))
                    {
                        var culturePart = name.Substring(resourcePrefix.Length, name.Length - resourcePrefix.Length - ".resources".Length);
                        if (!string.IsNullOrEmpty(culturePart) && culturePart != "resources")
                        {
                            try
                            {
                                var culture = new CultureInfo(culturePart);
                                cultures.Add(culture);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            var satelliteDirs = new[] { "en", "zh-CN", "zh-TW", "ja", "ko", "de", "fr", "es", "it", "ru", "pt", "ar", "nl", "pl", "tr", "vi", "th", "id", "cs", "hu", "sv", "da", "fi", "nb", "el", "he", "hi", "uk", "ro", "sk", "bg", "hr", "sl", "et", "lv", "lt" };
            foreach (var cultureName in satelliteDirs)
            {
                try
                {
                    var culture = new CultureInfo(cultureName);
                    if (_resourceManager.GetResourceSet(culture, true, false) != null)
                    {
                        if (!cultures.Any(c => c.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase)))
                        {
                            cultures.Add(culture);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        if (_options.KnownCultures != null)
        {
            foreach (var cultureName in _options.KnownCultures)
            {
                try
                {
                    var culture = new CultureInfo(cultureName);
                    if (!cultures.Any(c => c.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase)))
                    {
                        cultures.Add(culture);
                    }
                }
                catch
                {
                }
            }
        }

        _availableCultures = cultures.Distinct().ToList();
    }

    /// <summary>
    /// Gets all available cultures.
    /// </summary>
    public IEnumerable<CultureInfo> GetAvailableCultures()
    {
        return _availableCultures ?? Enumerable.Empty<CultureInfo>();
    }

    /// <summary>
    /// Gets the localized string for the specified key and culture.
    /// </summary>
    public string? GetString(string key, CultureInfo culture)
    {
        return _resourceManager?.GetString(key, culture);
    }

    public bool TryGetString(string key, CultureInfo culture, out string? value)
    {
        value = GetString(key, culture);
        return value != null;
    }

    /// <summary>
    /// Reloads available cultures.
    /// </summary>
    public Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        DetectAvailableCultures();
        return Task.CompletedTask;
    }
}
