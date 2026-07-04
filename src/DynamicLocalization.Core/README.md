# DynamicLocalization.Core

Core library for DynamicLocalization - platform-independent localization services.

This package contains:
- `ICultureService` - Core culture service interface
- `CultureService` - Culture service implementation
- `ILocalizationProvider` - Localization provider interface
- `JsonLocalizationProvider` - JSON file provider
- `ResxLocalizationProvider` - RESX resource provider
- `LocalizedString` - Observable localized string wrapper

## Installation

```bash
dotnet add package DynamicLocalization.Core
```

## Manual Instantiation (No DI)

```csharp
using DynamicLocalization.Core;
using DynamicLocalization.Core.Providers;

var cultureService = new CultureService();

var jsonProvider = new JsonLocalizationProvider();
jsonProvider.Initialize(new JsonLocalizationProviderOptions
{
    BasePath = "Localization",
    UseEmbeddedResources = true,
    Assembly = typeof(App).Assembly
});

cultureService.RegisterProvider(jsonProvider);
LocalizationService.Initialize(cultureService);

// Get localized string
string greeting = cultureService["Greeting"];

// Switch culture
cultureService.SetCulture("zh-CN");
```

## With DI (Microsoft.Extensions.DependencyInjection)

For DI integration, install the separate package:

```bash
dotnet add package DynamicLocalization.DependencyInjection
```

See [DynamicLocalization.DependencyInjection](../DynamicLocalization.DependencyInjection/README.md) for usage.

## Supported Platforms

- .NET Standard 2.0+
- .NET 6.0+
- .NET 7.0+
- .NET 8.0+
- .NET 9.0+
- Unity 2021.3+ (via .NET Standard 2.1)

## GitHub

https://github.com/StarryXYJ/Avalonia.DynamicLocalization