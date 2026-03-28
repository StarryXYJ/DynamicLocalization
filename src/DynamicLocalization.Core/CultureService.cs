using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DynamicLocalization.Core.Providers;

namespace DynamicLocalization.Core;

public class CultureService : ICultureService, INotifyPropertyChanged
{
    private readonly List<ILocalizationProvider> _providers = new();
    private CultureInfo _currentCulture;
    private List<CultureInfo>? _availableCultures;
    private CultureInfo _currentCultureField;

    public event PropertyChangedEventHandler? PropertyChanged;
    
    public event EventHandler<CultureChangedEventArgs>? CultureChanged;

    public CultureInfo CurrentCulture
    {
        get => _currentCultureField;
        set
        {
            if (_currentCultureField != value)
            {
                var oldCulture = _currentCulture;
                _currentCulture = value;
                CultureInfo.CurrentUICulture = value;
                CultureInfo.DefaultThreadCurrentUICulture = value;
                _currentCultureField = value;
                OnPropertyChanged(nameof(CurrentCulture));
                OnPropertyChanged(nameof(CurrentCultureName));
                OnPropertyChanged("Item[]");
                OnCultureChanged(value, oldCulture);
            }
        }
    }

    public string CurrentCultureName => _currentCultureField.Name;

    public IReadOnlyList<CultureInfo> AvailableCultures
    {
        get
        {
            if (_availableCultures == null)
            {
                _availableCultures = _providers
                    .SelectMany(p => p.GetAvailableCultures())
                    .Distinct()
                    .ToList();
            }
            return _availableCultures;
        }
    }

    public string this[string key] => GetString(key);

    public CultureService()
    {
        _currentCulture = CultureInfo.CurrentUICulture;
        _currentCultureField = _currentCulture;
    }

    public string GetString(string key)
    {
        return GetString(key, _currentCulture);
    }

    public string GetString(string key, CultureInfo? culture)
    {
        culture ??= _currentCulture;

        foreach (var provider in _providers)
        {
            if (provider.TryGetString(key, culture, out var value) && value != null)
            {
                return value;
            }
        }

        if (AvailableCultures.Count > 0 && !AvailableCultures.Contains(culture))
        {
            var fallbackCulture = AvailableCultures[0];
            foreach (var provider in _providers)
            {
                if (provider.TryGetString(key, fallbackCulture, out var value) && value != null)
                {
                    return value;
                }
            }
        }

        return $"#{key}#";
    }

    public string Format(string key, params object[] args)
    {
        var format = GetString(key);
        return string.Format(_currentCulture, format, args);
    }

    public void RegisterProvider(ILocalizationProvider provider)
    {
        _providers.Add(provider);
        _availableCultures = null;
    }

    public void UnregisterProvider(string providerName)
    {
        _providers.RemoveAll(p => p.Name == providerName);
        _availableCultures = null;
    }

    public void SetCulture(string cultureName, bool includeFormatting = false)
    {
        var culture = new CultureInfo(cultureName);
        
        if (includeFormatting)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
        }
        
        CurrentCulture = culture;
    }

    protected virtual void OnCultureChanged(CultureInfo newCulture, CultureInfo oldCulture)
    {
        CultureChanged?.Invoke(this, new CultureChangedEventArgs(newCulture, oldCulture));
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
