using Avalonia.DynamicLocalization.Core;

namespace Avalonia.DynamicLocalization;

/// <summary>
/// Static localization service entry point providing global access.
/// </summary>
/// <remarks>
/// <para>
/// This class is primarily used by XAML Markup Extensions (like <see cref="MarkupExtensions.LocalizeExtension"/>)
/// to access the localization service when dependency injection is not available.
/// </para>
/// <para>
/// Must be initialized at application startup using the <see cref="Initialize"/> method.
/// </para>
/// </remarks>
/// <example>
/// Initialization:
/// <code>
/// // In App.axaml.cs
/// var languageService = serviceProvider.GetRequiredService&lt;ILanguageService&gt;();
/// LocalizationService.Initialize(languageService);
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
    private static ILanguageService? _languageService;

    /// <summary>
    /// Gets or sets the language service instance.
    /// </summary>
    /// <remarks>
    /// Typically no need to set manually; use <see cref="Initialize"/> method instead.
    /// </remarks>
    public static ILanguageService? LanguageService
    {
        get => _languageService;
        set => _languageService = value;
    }

    /// <summary>
    /// Initializes the static localization service.
    /// </summary>
    /// <param name="languageService">The language service instance.</param>
    /// <remarks>
    /// <para>
    /// This method should be called at application startup, typically immediately after the DI container is built.
    /// </para>
    /// <para>
    /// Recommended to use the extension method <c>serviceProvider.InitializeLocalization()</c> for automatic initialization.
    /// </para>
    /// </remarks>
    public static void Initialize(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <returns>The localized string, or #key# format if service not initialized or key not found.</returns>
    public static string GetString(string key)
    {
        return _languageService?[key] ?? $"#{key}#";
    }
}
