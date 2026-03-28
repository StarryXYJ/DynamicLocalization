using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DynamicLocalization.Core.Providers;

namespace DynamicLocalization.Core;

/// <summary>
/// Core interface for culture service providing localization functionality.
/// </summary>
/// <remarks>
/// <para>
/// This interface is the core of the localization system, managing the current culture,
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
/// // Get localized string
/// string greeting = cultureService["Greeting"];
/// 
/// // Switch culture
/// cultureService.SetCulture("zh-CN");
/// 
/// // Listen for culture changes
/// cultureService.CultureChanged += (s, e) => 
/// {
///     Console.WriteLine($"Culture changed from {e.OldCulture.Name} to {e.NewCulture.Name}");
/// };
/// </code>
/// </example>
public interface ICultureService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the current culture for UI localization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Setting this property triggers the <see cref="CultureChanged"/> event
    /// and updates <see cref="CultureInfo.CurrentUICulture"/>.
    /// </para>
    /// <para>
    /// Note: This only affects resource lookup (localized strings). 
    /// To also affect formatting (numbers, dates, currency), use <see cref="SetCulture"/>.
    /// </para>
    /// </remarks>
    CultureInfo CurrentCulture { get; set; }

    /// <summary>
    /// Gets the current culture name (e.g., "en", "zh-CN").
    /// </summary>
    /// <remarks>
    /// This is a convenience property equivalent to <c>CurrentCulture.Name</c>.
    /// </remarks>
    string CurrentCultureName { get; }

    /// <summary>
    /// Gets all available cultures.
    /// </summary>
    /// <remarks>
    /// This list is composed of cultures from all registered <see cref="ILocalizationProvider"/> instances.
    /// </remarks>
    IReadOnlyList<CultureInfo> AvailableCultures { get; }

    /// <summary>
    /// Gets the localized string for the specified key using the current culture.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <returns>The localized string, or #key# format if not found.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Gets the localized string for the specified key using the current culture.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <returns>The localized string.</returns>
    string GetString(string key);

    /// <summary>
    /// Gets the localized string for the specified key and culture.
    /// </summary>
    /// <param name="key">The localization key.</param>
    /// <param name="culture">The target culture, or null to use the current culture.</param>
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
    /// string message = cultureService.Format("Welcome", "John");
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
    /// Sets the current culture by name (e.g., "en", "zh-CN").
    /// </summary>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="includeFormatting">
    /// If <c>true</c>, also sets <see cref="CultureInfo.CurrentCulture"/> for number/date/currency formatting.
    /// Default is <c>false</c> to only affect resource lookup.
    /// </param>
    /// <remarks>
    /// <para>
    /// When <paramref name="includeFormatting"/> is <c>false</c> (default):
    /// <list type="bullet">
    /// <item><description>Only <see cref="CultureInfo.CurrentUICulture"/> is set</description></item>
    /// <item><description>Affects localized string lookup only</description></item>
    /// <item><description>Safe for APIs and services that need consistent formatting</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// When <paramref name="includeFormatting"/> is <c>true</c>:
    /// <list type="bullet">
    /// <item><description>Both <see cref="CultureInfo.CurrentUICulture"/> and <see cref="CultureInfo.CurrentCulture"/> are set</description></item>
    /// <item><description>Affects localized strings AND number/date/currency formatting</description></item>
    /// <item><description>Recommended for desktop applications with local users</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="cultureName"/> is invalid.</exception>
    void SetCulture(string cultureName, bool includeFormatting = false);

    /// <summary>
    /// Occurs when the current culture changes.
    /// </summary>
    event EventHandler<CultureChangedEventArgs>? CultureChanged;
}
