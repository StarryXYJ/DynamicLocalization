using System;
using DynamicLocalization.Core;
using DynamicLocalization.Core.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicLocalization.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonLocalization(
        this IServiceCollection services,
        Action<JsonLocalizationProviderOptions>? configure = null)
    {
        var options = new JsonLocalizationProviderOptions();
        configure?.Invoke(options);

        services.AddSingleton<ILocalizationProvider>(sp =>
        {
            var provider = new JsonLocalizationProvider();
            provider.Initialize(options);
            return provider;
        });

        return services;
    }

    public static IServiceCollection AddResxLocalization(
        this IServiceCollection services,
        Action<ResxLocalizationProviderOptions>? configure = null)
    {
        var options = new ResxLocalizationProviderOptions();
        configure?.Invoke(options);

        services.AddSingleton<ILocalizationProvider>(sp =>
        {
            var provider = new ResxLocalizationProvider();
            provider.Initialize(options);
            return provider;
        });

        return services;
    }

    public static IServiceCollection AddYamlLocalization(
        this IServiceCollection services,
        Action<YamlLocalizationProviderOptions>? configure = null)
    {
        var options = new YamlLocalizationProviderOptions();
        configure?.Invoke(options);

        services.AddSingleton<ILocalizationProvider>(sp =>
        {
            var provider = new YamlLocalizationProvider();
            provider.Initialize(options);
            return provider;
        });

        return services;
    }

    public static IServiceCollection AddCultureService(this IServiceCollection services)
    {
        services.AddSingleton<ICultureService>(sp =>
        {
            var cultureService = new CultureService();
            foreach (var provider in sp.GetServices<ILocalizationProvider>())
            {
                cultureService.RegisterProvider(provider);
            }
            return cultureService;
        });

        return services;
    }

    public static IServiceProvider InitializeLocalization(this IServiceProvider serviceProvider)
    {
        var cultureService = serviceProvider.GetRequiredService<ICultureService>();
        LocalizationService.Initialize(cultureService);
        return serviceProvider;
    }
}