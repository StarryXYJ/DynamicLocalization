# Avalonia.DynamicLocalization

[中文文档](README.zh-CN.md)

A lightweight, extensible, and pluggable Avalonia internationalization library with hot-reload support and multiple data sources.

## Features

- 🌍 **Multi-language Support** - Support for any number of languages
- 🔄 **Hot Reload** - Dynamically switch languages at runtime without restart
- 🔌 **Pluggable Architecture** - Support for custom data source providers
- 📦 **JSON Support** - Built-in JSON localization file support
- 📄 **RESX Support** - Built-in RESX resource file support
- 🎯 **XAML Friendly** - Provides clean XAML markup extensions
- 💉 **DI Integration** - Full dependency injection support

# Quick Start

## Installation

### Install via NuGet Package Manager:

Avalonia.DynamicLocalization

### Package Reference
```xml
<PackageReference Include="Avalonia.DynamicLocalization"  />
```


## Quick Start

### 1. Data Source and Service Configuration

#### Method 1: Using JSON Files

##### 1. Create Localization Files

Create a `Localization` folder in your project and add JSON files:

**Localization/en.json**
```json
{
  "App.Title": "My Application",
  "Greeting": "Hello, World!",
  "WelcomeMessage": "Welcome to our application."
}
```

**Localization/zh-CN.json**
```json
{
  "App.Title": "我的应用",
  "Greeting": "你好，世界！",
  "WelcomeMessage": "欢迎使用我们的应用程序。"
}
```

##### 2. Configure Services

**Option 1: Load from File System (Default)**

```csharp
services.AddJsonLocalization(options =>
{
    options.BasePath = "Localization";
    options.FilePattern = "*.json";
});
```

**Option 2: Load from Embedded Resources**

First, configure JSON files as embedded resources in your `.csproj`:

```xml
<ItemGroup>
  <EmbeddedResource Include="Localization\*.json" />
</ItemGroup>
```

Most IDEs also support setting files as embedded resources via right-click file properties.

Then configure services:

```csharp
services.AddJsonLocalization(options =>
{
    options.BasePath = "Localization";
    options.UseEmbeddedResources = true;
    // options.Assembly = typeof(App).Assembly; // Optional, defaults to calling assembly
});
```

#### Method 2: Using RESX Files

##### 1. Create RESX Resource Files

Add RESX resource files to your project:

You can typically create resource files directly through the IDE's right-click menu.

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
  <data name="WelcomeMessage" xml:space="preserve">
    <value>Welcome to our application.</value>
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
  <data name="WelcomeMessage" xml:space="preserve">
    <value>欢迎使用我们的应用程序。</value>
  </data>
</root>
```

##### 2. Configure Services

```csharp
services.AddResxLocalization(options =>
{
    options.ResourceType = typeof(Resources.Strings);
});
```

#### Method 3: Custom Data Sources and Combinations

[See Custom Data Sources section below](#custom-provider)

Combination - You can inject multiple data sources simultaneously, and all will be searched when used.


#### Complete Configuration Example

Configure services in `App.axaml.cs`:

```csharp
using Avalonia.DynamicLocalization;
using Avalonia.DynamicLocalization.Core;
using Avalonia.DynamicLocalization.Extensions;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        var services = new ServiceCollection();
        
        // Method 1: Using JSON files
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.FilePattern = "*.json";
        });

        // Method 2: Using RESX files
        // services.AddResxLocalization(options =>
        // {
        //     options.ResourceType = typeof(Resources.Strings);
        // });

        // Configure language service
        services.AddLanguageService();

        // Initialize global services
        // Services = services.BuildServiceProvider().InitializeLocalization();
        services.BuildServiceProvider().InitializeLocalization();
        AvaloniaXamlLoader.Load(this);
    }
}
```

### 2. Using Localized Strings and Properties

#### Using in XAML

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:loc="clr-namespace:Avalonia.DynamicLocalization.MarkupExtensions;assembly=Avalonia.DynamicLocalization"
        Title="{loc:Localize App.Title}">

    <StackPanel Margin="20">
        <TextBlock Text="{loc:Localize Greeting}" FontSize="24"/>
        <TextBlock Text="{loc:Localize WelcomeMessage}"/>
    </StackPanel>
</Window>
```

#### Using in ViewModel

```csharp
using Avalonia.DynamicLocalization.Core;

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

# Project Analysis

## Core Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    Avalonia.DynamicLocalization                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                  ILanguageService                       │    │
│  │  - Core service interface                               │    │
│  │  - Language management, string retrieval, notifications │    │
│  └─────────────────────────────────────────────────────────┘    │
│                              │                                  │
│                              ▼                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                  ILocalizationProvider                  │    │
│  │  - Data source provider interface                       │    │
│  │  - Defines unified resource retrieval methods           │    │
│  └─────────────────────────────────────────────────────────┘    │
│                              │                                  │
│          ┌───────────────────┼───────────────────┐              │
│          ▼                   ▼                   ▼              │
│  ┌─────────────┐     ┌─────────────┐     ┌─────────────┐        │
│  │ JsonProvider│     │ResxProvider │     │CustomProviders│      │
│  └─────────────┘     └─────────────┘     └─────────────┘        │
│          │                   │                   │              │
│          └───────────────────┼───────────────────┘              │
│                              ▼                                  │              │
│                          I18n  Files                            │
└─────────────────────────────────────────────────────────────────┘
```

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

Data source provider interface, can be custom implemented:

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
### LocalizeExtension

XAML markup extension:

```xml
<!-- Basic usage -->
<TextBlock Text="{loc:Localize KeyName}"/>

<!-- With formatting -->
<TextBlock Text="{loc:Localize KeyName, StringFormat='{}{0}!'}"/>
```

## Project Structure

```
Avalonia.DynamicLocalization/
├── Core/
│   ├── ILanguageService.cs          # Core service interface
│   ├── LanguageService.cs           # Core service implementation
│   ├── LocalizedString.cs           # Observable localized string
│   └── LanguageChangedEventArgs.cs  # Language changed event args
│
├── Providers/
│   ├── ILocalizationProvider.cs     # Data source provider interface
│   ├── JsonLocalizationProvider.cs  # JSON data source implementation
│   └── ResxLocalizationProvider.cs  # RESX data source implementation
│
├── MarkupExtensions/
│   └── LocalizeExtension.cs         # XAML markup extension
│
├── Extensions/
│   └── ServiceCollectionExtensions.cs # DI extension methods
│
└── LocalizationService.cs           # Global service access point
```

## Built-in Providers

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

```csharp
// Auto-detect (default)
services.AddResxLocalization(options =>
{
    options.ResourceType = typeof(Resources.Strings);
});

// Manually specify culture list
services.AddResxLocalization(options =>
{
    options.ResourceType = typeof(Resources.Strings);
    options.AutoDetectCultures = false;
    options.KnownCultures = new[] { "en", "zh-CN", "ja", "ko" };
});
```

# Extensibility

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



# License

MIT License
