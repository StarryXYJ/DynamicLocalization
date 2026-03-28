namespace DynamicLocalization.Core;

/// <summary>
/// Static localization service entry point providing global access.
/// </summary>
/// <remarks>
/// <para>
/// This class is primarily used by XAML Markup Extensions
/// to access the localization service when dependency injection is not available.
/// </para>
/// <para>
/// Must be initialized at application startup using the <see cref="Initialize"/> method.
/// </para>
/// </remarks>
/// <example>
/// Initialization:
/// <code>
/// // In App.axaml.cs or App.xaml.cs
/// var cultureService = serviceProvider.GetRequiredService&lt;ICultureService&gt;();
/// LocalizationService.Initialize(cultureService);
/// </code>
/// 
/// Usage:
/// <code>
/// // Get string in code
/// string text = LocalizationService.GetString("Greeting");
/// 
/// // Use in XAML
/// // &lt;TextBlock Text="{loc:Localize Greeting}"/&gt;
/// </code>
/// </example>
public static class LocalizationService
{
    private static ICultureService? _cultureService;

    /// <summary>
    /// Gets or sets the culture service instance.
    /// </summary>
    /// <remarks>
    /// Typically no need to set manually; use <see cref="Initialize"/> method instead.
    /// </remarks>
    public static ICultureService? CultureService
    {
        get => _cultureService;
        set => _cultureService = value;
    }

    /// <summary>
    /// Initializes the static localization service.
    /// </summary>
    /// <param name="cultureService">The culture service instance.</param>
    /// <remarks>
    /// <para>
    /// This method should be called at application startup, typically immediately after the DI container is built.
    /// </para>
    /// <para>
    /// Recommended to use the platform-specific extension method <c>serviceProvider.InitializeLocalization()</c> for automatic initialization.
    /// </para>
    /// </remarks>
    public static void Initialize(ICultureService cultureService)
    {
        _cultureService = cultureService;
    }

    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <returns>The localized string, or #key# format if service not initialized or key not found.</returns>
    public static string GetString(string key)
    {
        return _cultureService?[key] ?? $"#{key}#";
    }
}
