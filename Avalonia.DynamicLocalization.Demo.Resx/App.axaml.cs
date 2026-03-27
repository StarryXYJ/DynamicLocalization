using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.DynamicLocalization.Demo.Resx.ViewModels;
using Avalonia.DynamicLocalization.Demo.Resx.Views;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.DynamicLocalization.Extensions;

namespace Avalonia.DynamicLocalization.Demo.Resx;

/// <summary>
/// Application entry point demonstrating RESX-based localization.
/// </summary>
/// <remarks>
/// <para>Configuration steps:</para>
/// <list type="number">
///   <item><description>Call <see cref="ServiceCollectionExtensions.AddResxLocalization"/> to register RESX provider</description></item>
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
    /// RESX localization uses Visual Studio generated resource classes.
    /// File naming: Strings.resx (default), Strings.zh-CN.resx (Chinese), etc.
    /// </remarks>
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddResxLocalization(options =>
        {
            options.ResourceType = typeof(global::Avalonia.DynamicLocalization.Demo.Resx.Resources.Strings);
            options.AutoDetectCultures = false;
            options.KnownCultures = new[] { "en", "en-US", "en-GB", "zh-CN", "zh-TW", "ja", "de" };
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
