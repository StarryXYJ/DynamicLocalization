using System;
using System.Windows.Data;
using System.Windows.Markup;
using DynamicLocalization.Core;

namespace DynamicLocalization.WPF.MarkupExtensions;

/// <summary>
/// XAML markup extension for binding localized strings in XAML.
/// </summary>
/// <remarks>
/// <para>
/// This extension retrieves the language service through <see cref="LocalizationService"/>
/// and returns a binding object bound to <see cref="LocalizedString"/>,
/// enabling automatic UI updates when language changes.
/// </para>
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// &lt;TextBlock Text="{loc:Localize Greeting}"/&gt;
/// </code>
/// 
/// With string format:
/// <code>
/// &lt;TextBlock Text="{loc:Localize WelcomeMessage, StringFormat='Hello, {0}!'}"/&gt;
/// </code>
/// </example>
public class LocalizeExtension : MarkupExtension
{
    /// <summary>
    /// Gets the localization key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets or sets the string format template.
    /// </summary>
    public string? StringFormat { get; set; }

    /// <summary>
    /// Creates a new instance of LocalizeExtension.
    /// </summary>
    /// <param name="key">The localization key.</param>
    public LocalizeExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Provides the binding value.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A binding object bound to the localized string.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var cultureService = LocalizationService.CultureService;
        if (cultureService == null)
        {
            return $"#{Key}#";
        }

        var localizedString = new LocalizedString(cultureService, Key);

        var binding = new Binding(nameof(LocalizedString.Value))
        {
            Source = localizedString,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }
}
