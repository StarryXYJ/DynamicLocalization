using System;
using System.ComponentModel;
using System.Globalization;

namespace DynamicLocalization.Core;

/// <summary>
/// Localized string wrapper that supports automatic updates when language changes.
/// </summary>
/// <remarks>
/// <para>
/// This class implements <see cref="INotifyPropertyChanged"/> interface,
/// automatically updating the <see cref="Value"/> property and raising notifications
/// when the language changes.
/// </para>
/// <para>
/// Primarily used in XAML binding scenarios, working with platform-specific markup extensions.
/// </para>
/// </remarks>
public class LocalizedString : INotifyPropertyChanged, IDisposable
{
    private readonly ICultureService _cultureService;
    private readonly string _key;
    private readonly object?[]? _args;
    private string? _value;

    /// <summary>
    /// Creates a new localized string instance.
    /// </summary>
    /// <param name="cultureService">The language service.</param>
    /// <param name="key">The localization key.</param>
    /// <param name="args">Optional format arguments.</param>
    public LocalizedString(ICultureService cultureService, string key, params object?[] args)
    {
        _cultureService = cultureService;
        _key = key;
        _args = args;
        _value = GetValue();
        _cultureService.CultureChanged += OnCultureChanged;
    }

    /// <summary>
    /// Gets the localized string value for the current language.
    /// </summary>
    public string? Value => _value;

    /// <summary>
    /// Returns the string representation.
    /// </summary>
    public override string? ToString() => _value;

    /// <summary>
    /// Implicit conversion to string.
    /// </summary>
    public static implicit operator string?(LocalizedString? ls) => ls?.ToString();

    /// <summary>
    /// Gets the localized string value.
    /// </summary>
    private string? GetValue()
    {
        if (_args == null || _args.Length == 0)
        {
            return _cultureService[_key];
        }
        return _cultureService.Format(_key, _args!);
    }

    /// <summary>
    /// Handles language change events.
    /// </summary>
    private void OnCultureChanged(object? sender, CultureChangedEventArgs e)
    {
        _value = GetValue();
        OnPropertyChanged(nameof(Value));
    }

    /// <summary>
    /// Property changed event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the property changed notification.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Disposes resources and unsubscribes from language change events.
    /// </summary>
    public void Dispose()
    {
        _cultureService.CultureChanged -= OnCultureChanged;
    }
}
