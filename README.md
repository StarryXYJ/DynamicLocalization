# DynamicLocalization

[中文文档](README.zh-CN.md)

A lightweight, extensible, and pluggable internationalization library with hot-reload support and multiple data sources for Avalonia and WPF.

## Features

- 🌍 **Multi-language Support** - Support for any number of languages
- 🔄 **Hot Reload** - Dynamically switch languages at runtime without restart
- 🔌 **Pluggable Architecture** - Support for custom data source providers
- 📦 **JSON Support** - Built-in JSON localization file support (flat and nested formats)
- 📄 **RESX Support** - Built-in RESX resource file support
- 🎯 **XAML Friendly** - Provides clean XAML markup extensions
- 💉 **DI Integration** - Full dependency injection support
- 🖥️ **Multi-Platform** - Support for Avalonia and WPF

## Packages

| Package | Description | Platform |
|---------|-------------|----------|
| [DynamicLocalization.Core](src/DynamicLocalization.Core) | Core library with platform-independent logic | .NET 6+ |
| [DynamicLocalization.Avalonia](src/DynamicLocalization.Avalonia) | Avalonia platform implementation | Avalonia 11+ |
| [DynamicLocalization.WPF](src/DynamicLocalization.WPF) | WPF platform implementation | WPF (.NET 6+) |

## Installation

### Avalonia

```xml
<PackageReference Include="DynamicLocalization.Avalonia" />
```

### WPF

```xml
<PackageReference Include="DynamicLocalization.WPF" />
```

## Quick Start

### 1. Create Localization Files

#### Option A: JSON Files

Create a `Localization` folder in your project and add JSON files.

**Flat Format (Traditional):**

**Localization/en.json**
```json
{
  "App.Title": "My Application",
  "Greeting": "Hello, World!",
  "WelcomeMessage": "Welcome to our application."
}
```

**Nested Format (Recommended for better organization):**

**Localization/en.json**
```json
{
  "App": {
    "Title": "My Application",
    "Version": "1.0.0"
  },
  "Greeting": "Hello, World!",
  "WelcomeMessage": "Welcome to our application.",
  "Features": {
    "Title": "Features:",
    "HotReload": "Hot reload support",
    "Pluggable": "Pluggable provider system"
  }
}
```

Both formats produce the same keys: `App.Title`, `App.Version`, `Greeting`, `WelcomeMessage`, `Features.Title`, etc.

**Localization/zh-CN.json**
```json
{
  "App": {
    "Title": "我的应用",
    "Version": "1.0.0"
  },
  "Greeting": "你好，世界！",
  "WelcomeMessage": "欢迎使用我们的应用程序。",
  "Features": {
    "Title": "特性：",
    "HotReload": "热重载支持",
    "Pluggable": "可插拔的提供程序系统"
  }
}
```

#### Option B: RESX Files

Add RESX resource files to your project:

**Resources/Strings.resx** (Default/English)
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="App.Title" xml:space="preserve">
    <value>My Application</value>
  </data>
  <data name="Greeting" xml:space="preserve">
    <value>Hello, World!</value>
  </data>
</root>
```

**Resources/Strings.zh-CN.resx** (Chinese)
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="App.Title" xml:space="preserve">
    <value>我的应用</value>
  </data>
  <data name="Greeting" xml:space="preserve">
    <value>你好，世界！</value>
  </data>
</root>
```

### 2. Configure Services

#### Avalonia (App.axaml.cs)

```csharp
using DynamicLocalization.Avalonia.Extensions;
using DynamicLocalization.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        var services = new ServiceCollection();
        
        // Option A: JSON files
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.UseEmbeddedResources = true;
            options.Assembly = typeof(App).Assembly;
        });

        // Option B: RESX files
        // services.AddResxLocalization(options =>
        // {
        //     options.ResourceType = typeof(Resources.Strings);
        // });

        services.AddLanguageService();
        Services = services.BuildServiceProvider().InitializeLocalization();
        AvaloniaXamlLoader.Load(this);
    }
}
```

#### WPF (App.xaml.cs)

```csharp
using DynamicLocalization.WPF.Extensions;
using DynamicLocalization.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        // Option A: JSON files
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.UseEmbeddedResources = true;
            options.Assembly = typeof(App).Assembly;
        });

        // Option B: RESX files
        // services.AddResxLocalization(options =>
        // {
        //     options.ResourceType = typeof(Properties.Resources);
        // });

        services.AddLanguageService();
        Services = services.BuildServiceProvider().InitializeLocalization();
        
        base.OnStartup(e);
    }
}
```

### 3. Using in XAML

#### Avalonia

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:loc="clr-namespace:DynamicLocalization.Avalonia.MarkupExtensions;assembly=DynamicLocalization.Avalonia"
        Title="{loc:Localize App.Title}">

    <StackPanel Margin="20">
        <TextBlock Text="{loc:Localize Greeting}" FontSize="24"/>
        <TextBlock Text="{loc:Localize WelcomeMessage}"/>
        <TextBlock Text="{loc:Localize Features.HotReload}"/>
    </StackPanel>
</Window>
```

#### WPF

```xml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:loc="clr-namespace:DynamicLocalization.WPF.MarkupExtensions;assembly=DynamicLocalization.WPF"
        Title="{loc:Localize App.Title}">

    <StackPanel Margin="20">
        <TextBlock Text="{loc:Localize Greeting}" FontSize="24"/>
        <TextBlock Text="{loc:Localize WelcomeMessage}"/>
        <TextBlock Text="{loc:Localize Features.HotReload}"/>
    </StackPanel>
</Window>
```

### 4. Using in ViewModel

```csharp
using DynamicLocalization.Core;
using System.Globalization;

public class MainViewModel
{
    private readonly ILanguageService _languageService;

    public string Greeting => _languageService["Greeting"];
    
    public IReadOnlyList<CultureInfo> AvailableLanguages => _languageService.AvailableLanguages;

    public MainViewModel(ILanguageService languageService)
    {
        _languageService = languageService;
        _languageService.LanguageChanged += OnLanguageChanged;
    }

    public void ChangeLanguage(CultureInfo culture)
    {
        _languageService.CurrentLanguage = culture;
    }

    private void OnLanguageChanged(object? sender, LanguageChangedEventArgs e)
    {
        // Update bound properties
    }
}
```

## JSON Format Details

The JSON provider supports two formats:

### Flat Format
```json
{
  "App.Title": "My App",
  "App.Version": "1.0",
  "Features.HotReload": "Hot reload support"
}
```

### Nested Format
```json
{
  "App": {
    "Title": "My App",
    "Version": "1.0"
  },
  "Features": {
    "HotReload": "Hot reload support"
  }
}
```

Both formats result in the same keys: `App.Title`, `App.Version`, `Features.HotReload`.

The nested format is recommended for better organization and readability, especially for large projects.

## API Reference

### ILanguageService

Core language service interface:

| Property/Method | Description |
|-----------------|-------------|
| `CurrentLanguage` | Gets or sets the current language |
| `AvailableLanguages` | Gets the list of all available languages |
| `this[string key]` | Gets the localized string for the specified key |
| `GetString(string key)` | Gets a localized string |
| `GetString(string key, CultureInfo? culture)` | Gets a localized string for the specified culture |
| `Format(string key, params object[] args)` | Formats a localized string |
| `LanguageChanged` | Language changed event |

### ILocalizationProvider

Data source provider interface for custom implementations:

```csharp
public interface ILocalizationProvider
{
    string Name { get; }
    IEnumerable<CultureInfo> GetAvailableCultures();
    string? GetString(string key, CultureInfo culture);
    bool TryGetString(string key, CultureInfo culture, out string? value);
    Task ReloadAsync(CancellationToken cancellationToken = default);
}
```

## Provider Options

### JsonLocalizationProvider

| Option | Description | Default |
|--------|-------------|---------|
| `BasePath` | Directory containing JSON files | `"Localization"` |
| `FilePattern` | File matching pattern | `"*.json"` |
| `UseEmbeddedResources` | Whether to load from embedded resources | `false` |
| `Assembly` | Specified assembly (embedded resource mode) | Calling assembly |

### ResxLocalizationProvider

| Option | Description | Default |
|--------|-------------|---------|
| `ResourceType` | Type of the RESX resource file | Required |
| `AutoDetectCultures` | Whether to auto-detect available cultures | `true` |
| `KnownCultures` | Manually specify known culture list | `null` |

## Custom Provider

Implement the `ILocalizationProvider` interface to create custom data sources:

```csharp
public class DatabaseLocalizationProvider : ILocalizationProvider
{
    public string Name => "Database";

    public IEnumerable<CultureInfo> GetAvailableCultures()
    {
        // Get supported languages from database
    }

    public string? GetString(string key, CultureInfo culture)
    {
        // Get localized string from database
    }

    public bool TryGetString(string key, CultureInfo culture, out string? value)
    {
        value = GetString(key, culture);
        return value != null;
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        // Reload data
    }
}
```

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                      DynamicLocalization                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                  DynamicLocalization.Core               │    │
│  │  - ILanguageService, LanguageService                    │    │
│  │  - ILocalizationProvider, Providers                     │    │
│  │  - Platform-independent logic                           │    │
│  └─────────────────────────────────────────────────────────┘    │
│                              │                                  │
│              ┌───────────────┴───────────────┐                  │
│              ▼                               ▼                  │
│  ┌─────────────────────┐         ┌─────────────────────┐        │
│  │ DynamicLocalization │         │ DynamicLocalization │        │
│  │      .Avalonia      │         │        .WPF         │        │
│  │  - Avalonia Binding │         │  - WPF Binding      │        │
│  └─────────────────────┘         └─────────────────────┘        │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## License

MIT License
