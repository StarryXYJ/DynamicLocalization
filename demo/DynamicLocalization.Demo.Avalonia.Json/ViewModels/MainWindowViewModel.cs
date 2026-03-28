using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DynamicLocalization.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;

namespace DynamicLocalization.Demo.Avalonia.Json.ViewModels;

/// <summary>
/// Main window view model demonstrating JSON localization usage.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly ICultureService _cultureService;

    /// <summary>
    /// Gets the application title.
    /// </summary>
    public string Title => _cultureService["App.Title"];

    /// <summary>
    /// Gets the greeting message.
    /// </summary>
    public string Greeting => _cultureService["Greeting"];

    /// <summary>
    /// Gets the welcome message.
    /// </summary>
    public string WelcomeMessage => _cultureService["WelcomeMessage"];

    /// <summary>
    /// Gets the language switch label.
    /// </summary>
    public string SwitchLanguageLabel => _cultureService["SwitchLanguage"];

    /// <summary>
    /// Gets the available cultures from all providers.
    /// </summary>
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

    /// <summary>
    /// Creates a new instance of the view model.
    /// </summary>
    /// <param name="cultureService">The injected culture service.</param>
    public MainWindowViewModel(ICultureService cultureService)
    {
        _cultureService = cultureService;
        AvailableCultures = new ObservableCollection<CultureInfo>(cultureService.AvailableCultures);
        SelectedCulture = cultureService.CurrentCulture;

        _cultureService.CultureChanged += OnCultureChanged;
    }

    /// <summary>
    /// Notify property change
    /// Only necessary when you use <b>property</b> instead of xml mark
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCultureChanged(object? sender, CultureChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
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

    /// <summary>
    /// Unsubscribes from culture change events to prevent memory leaks.
    /// </summary>
    public void Dispose()
    {
        _cultureService.CultureChanged -= OnCultureChanged;
    }
}
