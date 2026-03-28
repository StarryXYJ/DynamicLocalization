using System;
using DynamicLocalization.Core.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicLocalization.Core.Extensions;

/// <summary>
/// Service collection extension methods for simplified localization service registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds JSON localization provider to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration delegate.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// JSON file format:
    /// </para>
    /// <code>
    /// {
    ///   "Greeting": "Hello",
    ///   "Welcome": "Welcome, {0}!"
    /// }
    /// </code>
    /// </remarks>
    /// <example>
    /// File system mode:
    /// <code>
    /// services.AddJsonLocalization(options =>
    /// {
    ///     options.BasePath = "Localization";
    /// });
    /// </code>
    /// 
    /// Embedded resource mode:
    /// <code>
    /// services.AddJsonLocalization(options =>
    /// {
    ///     options.BasePath = "Localization";
    ///     options.UseEmbeddedResources = true;
    ///     options.Assembly = typeof(App).Assembly;
    /// });
    /// </code>
    /// </example>
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

    /// <summary>
    /// Adds RESX localization provider to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration delegate.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddResxLocalization(options =>
    /// {
    ///     options.ResourceType = typeof(Resources.Strings);
    /// });
    /// </code>
    /// </example>
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

    /// <summary>
    /// Adds culture service that automatically collects all registered localization providers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method automatically collects all registered <see cref="ILocalizationProvider"/> instances
    /// and registers them with the <see cref="CultureService"/>.
    /// </para>
    /// <para>
    /// Must be called after all providers are registered.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddJsonLocalization(...);
    /// services.AddResxLocalization(...);
    /// services.AddCultureService(); // Must be called last
    /// </code>
    /// </example>
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

    /// <summary>
    /// Initializes the static localization service (call after BuildServiceProvider).
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The service provider for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method initializes the static instance of <see cref="LocalizationService"/>,
    /// enabling XAML markup extensions (like <c>{loc:Localize}</c>) to access the culture service.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// Services = services.BuildServiceProvider().InitializeLocalization();
    /// </code>
    /// </example>
    public static IServiceProvider InitializeLocalization(this IServiceProvider serviceProvider)
    {
        var cultureService = serviceProvider.GetRequiredService<ICultureService>();
        LocalizationService.Initialize(cultureService);
        return serviceProvider;
    }
}
