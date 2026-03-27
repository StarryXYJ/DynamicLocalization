using System.Globalization;

namespace Avalonia.DynamicLocalization.Providers;

/// <summary>
/// Localization provider interface defining the contract for retrieving localized strings from specific data sources.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to create localization providers for different data sources, such as:
/// </para>
/// <list type="bullet">
///   <item><description>JSON files (<see cref="JsonLocalizationProvider"/>)</description></item>
///   <item><description>RESX resource files (<see cref="ResxLocalizationProvider"/>)</description></item>
///   <item><description>Databases, remote APIs, etc.</description></item>
/// </list>
/// </remarks>
/// <example>
/// Custom provider example:
/// <code>
/// public class DatabaseLocalizationProvider : ILocalizationProvider
/// {
///     public string Name => "Database";
///     
///     public string? GetString(string key, CultureInfo culture)
///     {
///         // Query from database
///         return _db.GetLocalizedString(key, culture.Name);
///     }
///     
///     // ... other method implementations
/// }
/// </code>
/// </example>
public interface ILocalizationProvider
{
    /// <summary>
    /// Gets the provider name for identification and unregistration.
    /// </summary>
    /// <example>
    /// <code>
    /// // Unregister a specific provider
    /// languageService.UnregisterProvider("Json");
    /// </code>
    /// </example>
    string Name { get; }

    /// <summary>
    /// Gets all cultures supported by this provider.
    /// </summary>
    /// <returns>List of supported cultures.</returns>
    IEnumerable<CultureInfo> GetAvailableCultures();

    /// <summary>
    /// Gets the localized string for the specified key and culture.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <param name="culture">The target culture.</param>
    /// <returns>The localized string, or null if not found.</returns>
    string? GetString(string key, CultureInfo culture);

    /// <summary>
    /// Tries to get the localized string.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <param name="culture">The target culture.</param>
    /// <param name="value">The output localized string.</param>
    /// <returns>True if successfully retrieved; otherwise false.</returns>
    bool TryGetString(string key, CultureInfo culture, out string? value);

    /// <summary>
    /// Reloads the localization data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ReloadAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Localization provider interface with configuration options.
/// </summary>
/// <typeparam name="TOptions">The configuration options type.</typeparam>
public interface ILocalizationProvider<TOptions> : ILocalizationProvider
{
    /// <summary>
    /// Initializes the provider with the specified configuration.
    /// </summary>
    /// <param name="options">The configuration options.</param>
    void Initialize(TOptions options);
}
