# DynamicLocalization

[![MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/DynamicLocalization.Core.svg)](https://www.nuget.org/packages/DynamicLocalization.Core/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/DynamicLocalization.Core.svg)](https://www.nuget.org/packages/DynamicLocalization.Core/)

[中文文档](README.zh-CN.md)

A lightweight, extensible internationalization library with hot-reload and multiple data sources for Avalonia and WPF.

## Features

- **Multi-language** — Any number of languages with runtime switching
- **Hot Reload** — Switch languages without restart
- **Pluggable** — JSON, YAML, RESX providers; extendable to any data source
- **Plugin Support** — Dynamic provider register/unregister for plugin architectures
- **XAML Friendly** — Clean markup extensions for Avalonia and WPF
- **DI Integration** — Full `Microsoft.Extensions.DependencyInjection` support

## Packages

| Package | Description |
|---------|-------------|
| [DynamicLocalization.Core](https://www.nuget.org/packages/DynamicLocalization.Core/) | Core library, platform-independent |
| [DynamicLocalization.Avalonia](https://www.nuget.org/packages/DynamicLocalization.Avalonia/) | Avalonia platform (markup extensions) |
| [DynamicLocalization.WPF](https://www.nuget.org/packages/DynamicLocalization.WPF/) | WPF platform (markup extensions) |

## Installation

```xml
<!-- Avalonia -->
<PackageReference Include="DynamicLocalization.Avalonia" />
<!-- or WPF -->
<PackageReference Include="DynamicLocalization.WPF" />
```

## Quick Start

### 1. Create Localization Files

**JSON** — `Localization/en.json` (flat or nested format):

```json
{
  "App.Title": "My Application",
  "App": { "Title": "My Application" }      // same result
}
```

**YAML** — `Localization/en.yml`:

```yaml
App.Title: My Application
# or nested:
App:
  Title: My Application
```

**RESX** — Visual Studio resource files (`Resources/Strings.resx`).

### 2. Configure Services

```csharp
// Choose one or combine multiple providers
services.AddJsonLocalization(o =>
{
    o.BasePath = "Localization";
    o.UseEmbeddedResources = true;
    o.Assembly = typeof(App).Assembly;
});
// services.AddYamlLocalization(o => o.BasePath = "Localization");
// services.AddResxLocalization(o => o.ResourceType = typeof(Resources.Strings));

services.AddCultureService();
var sp = services.BuildServiceProvider().InitializeLocalization();
```

### 3. Use in XAML

```xml
<TextBlock Text="{loc:Localize App.Title}" />
<TextBlock Text="{loc:Localize Greeting, StringFormat='Hello, {0}!'}" />
```

### 4. Use in Code

```csharp
var greeting = cultureService["Greeting"];
cultureService.SetCulture("zh-CN");
cultureService.CultureChanged += (s, e) => Console.WriteLine($"Switched to {e.NewCulture.Name}");
```

## Provider Details

### JSON

- File: `{BasePath}/{culture}.json`
- String: `provider.LoadJsonString(json, "en")`
- Supports flat and nested formats

### YAML

- File: `{BasePath}/{culture}.yml`
- String: `provider.LoadYamlString(yaml, "en")`
- Supports flat and nested formats, list values

### RESX

- Uses Visual Studio `.resx` designer files
- Auto-detects satellite assemblies

See **[spec/providers.md](spec/providers.md)** for all options, formats, and programmatic usage.

## API Reference

See **[spec/api-reference.md](spec/api-reference.md)** for full API documentation (ICultureService, ILocalizationProvider, LocalizedString, DI extensions).

## Extending

- Implement `ILocalizationProvider` for custom data sources
- Inherit from `JsonLocalizationProvider` / `YamlLocalizationProvider` to override parsing
- Register/unregister providers dynamically for plugin scenarios

See **[spec/extending.md](spec/extending.md)** for custom providers, inheritance, and plugin integration.

## License

MIT License
