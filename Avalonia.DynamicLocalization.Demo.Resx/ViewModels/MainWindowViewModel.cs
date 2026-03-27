using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.DynamicLocalization.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.DynamicLocalization.Demo.Resx.ViewModels;

/// <summary>
/// Main window view model demonstrating RESX localization usage.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Gets the application title.
    /// </summary>
    public string Title => _languageService["App.Title"];

    /// <summary>
    /// Gets the greeting message.
    /// </summary>
    public string Greeting => _languageService["Greeting"];

    /// <summary>
    /// Gets the welcome message.
    /// </summary>
    public string WelcomeMessage => _languageService["WelcomeMessage"];

    /// <summary>
    /// Gets the language switch label.
    /// </summary>
    public string SwitchLanguageLabel => _languageService["SwitchLanguage"];

    /// <summary>
    /// Gets the available languages from all providers.
    /// </summary>
    public ObservableCollection<CultureInfo> AvailableLanguages { get; }

    [ObservableProperty]
    private CultureInfo? _selectedLanguage;

    public string CurrentCultureName => _languageService.CurrentLanguage.Name;

    public string ParentCultureName => _languageService.CurrentLanguage.Parent?.Name ?? "(none)";

    public string AvailableCulturesList => string.Join(", ", _languageService.AvailableLanguages.Select(c => c.Name));

    partial void OnSelectedLanguageChanged(CultureInfo? value)
    {
        if (value != null && _languageService.CurrentLanguage.Name != value.Name)
        {
            _languageService.CurrentLanguage = value;
        }
    }

    /// <summary>
    /// Creates a new instance of the view model.
    /// </summary>
    /// <param name="languageService">The injected language service.</param>
    public MainWindowViewModel(ILanguageService languageService)
    {
        _languageService = languageService;
        AvailableLanguages = new ObservableCollection<CultureInfo>(languageService.AvailableLanguages);
        SelectedLanguage = languageService.CurrentLanguage;

        _languageService.LanguageChanged += OnLanguageChanged;
    }

    /// <summary>
    /// Notify property change
    /// Only necessary when you use <b>property</b> instead of xml mark
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLanguageChanged(object? sender, LanguageChangedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Greeting));
            OnPropertyChanged(nameof(WelcomeMessage));
            OnPropertyChanged(nameof(SwitchLanguageLabel));
            OnPropertyChanged(nameof(CurrentCultureName));
            OnPropertyChanged(nameof(ParentCultureName));
        });
    }

    /// <summary>
    /// Unsubscribes from language change events to prevent memory leaks.
    /// </summary>
    public void Dispose()
    {
        _languageService.LanguageChanged -= OnLanguageChanged;
    }
}
