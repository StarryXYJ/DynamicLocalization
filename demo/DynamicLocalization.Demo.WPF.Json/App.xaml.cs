using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using DynamicLocalization.Core.Extensions;
using DynamicLocalization.Demo.WPF.Json.ViewModels;

namespace DynamicLocalization.Demo.WPF.Json;

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
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.UseEmbeddedResources = true;
            options.Assembly = typeof(App).Assembly;
        });

        services.AddCultureService();
        services.AddSingleton<MainWindowViewModel>();
    }
}
