using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using DynamicLocalization.Core.Extensions;
using DynamicLocalization.Demo.Wpf.Resx.ViewModels;

namespace DynamicLocalization.Demo.Wpf.Resx;

public partial class App : Application
{
    public new static App Current => (App)Application.Current!;

    public IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider().InitializeLocalization();

        var mainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<MainWindowViewModel>()
        };
        mainWindow.Show();

        base.OnStartup(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddResxLocalization(options =>
        {
            options.ResourceType = typeof(global::DynamicLocalization.Demo.Wpf.Resx.Resources.Strings);
            options.AutoDetectCultures = false;
            options.KnownCultures = ["en", "en-US", "en-GB", "zh-CN", "zh-TW", "ja", "de"];
        });

        services.AddCultureService();
        services.AddSingleton<MainWindowViewModel>();
    }
}
