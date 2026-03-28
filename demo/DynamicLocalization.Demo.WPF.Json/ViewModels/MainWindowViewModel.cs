using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using DynamicLocalization.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DynamicLocalization.Demo.WPF.Json.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly ICultureService _cultureService;

    public string Title => _cultureService["App.Title"];

    public string Greeting => _cultureService["Greeting"];

    public string WelcomeMessage => _cultureService["WelcomeMessage"];

    public string SwitchLanguageLabel => _cultureService["SwitchLanguage"];

    public ObservableCollection<CultureInfo> AvailableCultures { get; }

    [ObservableProperty]
    private CultureInfo? _selectedCulture;

    public string CurrentCultureName => _cultureService.CurrentCulture.Name;

    public string ParentCultureName => _cultureService.CurrentCulture.Parent?.Name ?? "(none)";

    public string AvailableCulturesList => string.Join(", ", _cultureService.AvailableCultures.Select(c => c.Name));

    public string FormatWelcome => _cultureService.Format("Format.Welcome", "John");
    
    public string FormatItemsCount => _cultureService.Format("Format.ItemsCount", 5);
    
    public string FormatPriceDisplay => _cultureService.Format("Format.PriceDisplay", "Laptop", 1299.99);
    
    public string FormatDateDisplay => _cultureService.Format("Format.DateDisplay", DateTime.Now);
    
    public string FormatNumberDisplay => _cultureService.Format("Format.NumberDisplay", 12345.6789);

    partial void OnSelectedCultureChanged(CultureInfo? value)
    {
        if (value != null && _cultureService.CurrentCulture.Name != value.Name)
        {
            _cultureService.CurrentCulture = value;
        }
    }

    public MainWindowViewModel(ICultureService cultureService)
    {
        _cultureService = cultureService;
        AvailableCultures = new ObservableCollection<CultureInfo>(cultureService.AvailableCultures);
        SelectedCulture = cultureService.CurrentCulture;

        _cultureService.CultureChanged += OnCultureChanged;
    }

    private void OnCultureChanged(object? sender, CultureChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Greeting));
            OnPropertyChanged(nameof(WelcomeMessage));
            OnPropertyChanged(nameof(SwitchLanguageLabel));
            OnPropertyChanged(nameof(CurrentCultureName));
            OnPropertyChanged(nameof(ParentCultureName));
            OnPropertyChanged(nameof(FormatWelcome));
            OnPropertyChanged(nameof(FormatItemsCount));
            OnPropertyChanged(nameof(FormatPriceDisplay));
            OnPropertyChanged(nameof(FormatDateDisplay));
            OnPropertyChanged(nameof(FormatNumberDisplay));
        });
    }

    public void Dispose()
    {
        _cultureService.CultureChanged -= OnCultureChanged;
    }
}
