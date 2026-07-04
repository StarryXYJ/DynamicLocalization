# API Reference

## ICultureService

Core service interface for managing culture and localization.

| Member | Description |
|--------|-------------|
| `CurrentCulture` | Gets or sets the current culture |
| `CurrentCultureName` | Gets the current culture name (e.g., "en", "zh-CN") |
| `AvailableCultures` | Gets all available cultures across all providers |
| `this[string key]` | Gets localized string for the specified key |
| `GetString(string key)` | Gets a localized string |
| `GetString(string key, CultureInfo? culture)` | Gets localized string for specific culture |
| `Format(string key, params object[] args)` | Formats a localized string |
| `SetCulture(string cultureName)` | Sets current culture by name |
| `SetCulture(string cultureName, bool includeFormatting)` | Sets culture with optional formatting culture |
| `RegisterProvider(ILocalizationProvider provider)` | Registers a localization provider |
| `UnregisterProvider(string providerName)` | Unregisters a provider by name |
| `CultureChanged` | Event raised when culture changes |
| `ProvidersChanged` | Event raised when providers change |

## ILocalizationProvider

Interface for implementing custom data source providers.

```csharp
public interface ILocalizationProvider
{
    string Name { get; }
    IEnumerable<CultureInfo> GetAvailableCultures();
    string? GetString(string key, CultureInfo culture);
    bool TryGetString(string key, CultureInfo culture, out string? value);
    Task ReloadAsync(CancellationToken cancellationToken = default);
}
```

## ILocalizationProvider\<TOptions\>

Generic interface for providers with configuration options.

```csharp
public interface ILocalizationProvider<TOptions> : ILocalizationProvider
{
    void Initialize(TOptions options);
}
```

## LocalizedString

Observable wrapper for localized strings with `INotifyPropertyChanged` support. Automatically updates when culture changes.

```csharp
public class LocalizedString : INotifyPropertyChanged, IDisposable
{
    string Value { get; }       // Current localized value
    // Implicit conversion to string
}
```

## LocalizationService

Static entry point for XAML markup extensions and non-DI scenarios.

```csharp
public static class LocalizationService
{
    static ICultureService? CultureService { get; set; }
    static void Initialize(ICultureService cultureService);
    static string GetString(string key);
}
```

## ServiceCollectionExtensions

DI extension methods for registering providers.

```csharp
// JSON provider
IServiceCollection AddJsonLocalization(
    this IServiceCollection services,
    Action<JsonLocalizationProviderOptions>? configure = null)

// YAML provider
IServiceCollection AddYamlLocalization(
    this IServiceCollection services,
    Action<YamlLocalizationProviderOptions>? configure = null)

// RESX provider
IServiceCollection AddResxLocalization(
    this IServiceCollection services,
    Action<ResxLocalizationProviderOptions>? configure = null)

// Culture service (collects all registered providers)
IServiceCollection AddCultureService(this IServiceCollection services)

// Initialize static LocalizationService
IServiceProvider InitializeLocalization(this IServiceProvider serviceProvider)
```
