using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Avalonia.DynamicLocalization.Providers;

namespace Avalonia.DynamicLocalization.Core;

/// <summary>
/// Core interface for language service providing localization functionality.
/// </summary>
/// <remarks>
/// <para>
/// This interface is the core of the localization system, managing the current language,
/// retrieving localized strings, and coordinating multiple localization providers.
/// </para>
/// <para>
/// Supports multiple <see cref="ILocalizationProvider"/> instances working together,
/// searching for strings in registration order.
/// </para>
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// // Get localized string——
/// string greeting = languageService["Greeting"];
/// 
/// // Switch language
/// languageService.CurrentLanguage = new CultureInfo("zh-CN");
/// 
/// // Listen for language changes
/// languageService.LanguageChanged += (s, e) => 
/// {
///     Console.WriteLine($"Language changed from {e.OldLanguage.Name} to {e.NewLanguage.Name}");
/// };
/// </code>
/// </example>
public interface ILanguageService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the current language.
    /// </summary>
    /// <remarks>
    /// Setting this property triggers the <see cref="LanguageChanged"/> event
    /// and updates the thread's UI culture.
    /// </remarks>
    CultureInfo CurrentLanguage { get; set; }

    /// <summary>
    /// Gets all available languages.
    /// </summary>
    /// <remarks>
    /// This list is composed of cultures from all registered <see cref="ILocalizationProvider"/> instances.
    /// </remarks>
    IReadOnlyList<CultureInfo> AvailableLanguages { get; }

    /// <summary>
    /// Gets the localized string for the specified key using the current language.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <returns>The localized string, or #key# format if not found.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Gets the localized string for the specified key using the current language.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <returns>The localized string.</returns>
    string GetString(string key);

    /// <summary>
    /// Gets the localized string for the specified key and culture.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <param name="culture">The target culture, or null to use the current language.</param>
    /// <returns>The localized string.</returns>
    string GetString(string key, CultureInfo? culture);

    /// <summary>
    /// Gets a formatted localized string.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    /// <example>
    /// <code>
    /// // JSON: "Welcome": "Welcome, {0}!"
    /// string message = languageService.Format("Welcome", "John");
    /// // Result: "Welcome, John!"
    /// </code>
    /// </example>
    string Format(string key, params object[] args);

    /// <summary>
    /// Registers a localization provider.
    /// </summary>
    /// <param name="provider">The provider to register.</param>
    void RegisterProvider(ILocalizationProvider provider);

    /// <summary>
    /// Unregisters a localization provider by name.
    /// </summary>
    /// <param name="providerName">The provider name.</param>
    void UnregisterProvider(string providerName);

    /// <summary>
    /// Occurs when the current language changes.
    /// </summary>
    event EventHandler<LanguageChangedEventArgs>? LanguageChanged;
}
