using System.Globalization;

namespace Avalonia.DynamicLocalization.Core;

/// <summary>
/// Event arguments for language change events.
/// </summary>
public class LanguageChangedEventArgs(CultureInfo newLanguage, CultureInfo? oldLanguage = null) : EventArgs
{
    /// <summary>
    /// Gets the new language.
    /// </summary>
    public CultureInfo NewLanguage { get; } = newLanguage;

    /// <summary>
    /// Gets the old language.
    /// </summary>
    public CultureInfo OldLanguage { get; } = oldLanguage ?? CultureInfo.CurrentUICulture;
}
