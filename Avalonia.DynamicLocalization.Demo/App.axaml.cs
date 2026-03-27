using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaLab.ViewModels;
using AvaloniaLab.Views;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.DynamicLocalization.Extensions;

namespace AvaloniaLab;

/// <summary>
/// Application entry point demonstrating JSON-based localization.
/// </summary>
/// <remarks>
/// <para>Configuration steps:</para>
/// <list type="number">
///   <item><description>Call <see cref="ServiceCollectionExtensions.AddJsonLocalization"/> to register JSON provider</description></item>
///   <item><description>Call <see cref="ServiceCollectionExtensions.AddLanguageService"/> to register language service</description></item>
///   <item><description>Call <see cref="ServiceCollectionExtensions.InitializeLocalization"/> to initialize static service</description></item>
/// </list>
/// </remarks>
public partial class App : Application
{
    /// <summary>
    /// Gets the current application instance.
    /// </summary>
    public new static App Current => (App)Application.Current!;

    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider().InitializeLocalization();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Configures localization services.
    /// </summary>
    /// <remarks>
    /// JSON localization uses embedded resources for deployment.
    /// Resource naming: {Assembly}.Localization.{Culture}.json (e.g., AvaloniaLab.Localization.en.json)
    /// </remarks>
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.UseEmbeddedResources = true;
            options.Assembly = typeof(App).Assembly;
        });

        services.AddLanguageService();
        services.AddSingleton<MainWindowViewModel>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
