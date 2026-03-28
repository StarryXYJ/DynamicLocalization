using System;
using System.Globalization;

namespace DynamicLocalization.Core;

/// <summary>
/// Event arguments for culture change events.
/// </summary>
public class CultureChangedEventArgs(CultureInfo newCulture, CultureInfo? oldCulture = null) : EventArgs
{
    /// <summary>
    /// Gets the new culture.
    /// </summary>
    public CultureInfo NewCulture { get; } = newCulture;

    /// <summary>
    /// Gets the old culture.
    /// </summary>
    public CultureInfo OldCulture { get; } = oldCulture ?? CultureInfo.CurrentUICulture;
}
