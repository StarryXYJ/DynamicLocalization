# DynamicLocalization

[English](README.md)

一个轻量级、可扩展、可插拔的国际化库，支持热重载和多种数据源，适用于 Avalonia 和 WPF。

## 特性

- 🌍 **多语言支持** - 支持任意数量的语言
- 🔄 **热重载** - 运行时动态切换语言，无需重启
- 🔌 **可插拔架构** - 支持自定义数据源提供者
- 📦 **JSON 支持** - 内置 JSON 本地化文件支持（扁平格式和嵌套格式）
- 📄 **RESX 支持** - 内置 RESX 资源文件支持
- 🎯 **XAML 友好** - 提供简洁的 XAML 标记扩展
- 💉 **DI 集成** - 完整的依赖注入支持
- 🖥️ **多平台** - 支持 Avalonia 和 WPF

## 包

| 包 | 描述 | 平台 |
|---------|-------------|----------|
| [DynamicLocalization.Core](src/DynamicLocalization.Core) | 核心库，包含平台无关逻辑 | .NET 6+ |
| [DynamicLocalization.Avalonia](src/DynamicLocalization.Avalonia) | Avalonia 平台实现 | Avalonia 11+ |
| [DynamicLocalization.WPF](src/DynamicLocalization.WPF) | WPF 平台实现 | WPF (.NET 6+) |

## 安装

### Avalonia

```xml
<PackageReference Include="DynamicLocalization.Avalonia" />
```

### WPF

```xml
<PackageReference Include="DynamicLocalization.WPF" />
```

## 快速开始

### 1. 创建本地化文件

#### 方式 A: JSON 文件

在项目中创建 `Localization` 文件夹并添加 JSON 文件。

**扁平格式（传统）：**

**Localization/en.json**
```json
{
  "App.Title": "My Application",
  "Greeting": "Hello, World!",
  "WelcomeMessage": "Welcome to our application."
}
```

**嵌套格式（推荐，结构更清晰）：**

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

两种格式生成的键相同：`App.Title`、`App.Version`、`Greeting`、`WelcomeMessage`、`Features.Title` 等。

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

#### 方式 B: RESX 文件

在项目中添加 RESX 资源文件：

**Resources/Strings.resx** (默认/英文)
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

**Resources/Strings.zh-CN.resx** (中文)
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

### 2. 配置服务

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
        
        // 方式 A: JSON 文件
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.UseEmbeddedResources = true;
            options.Assembly = typeof(App).Assembly;
        });

        // 方式 B: RESX 文件
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
        
        // 方式 A: JSON 文件
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.UseEmbeddedResources = true;
            options.Assembly = typeof(App).Assembly;
        });

        // 方式 B: RESX 文件
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

### 3. 在 XAML 中使用

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

### 4. 在 ViewModel 中使用

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
        // 更新绑定属性
    }
}
```

## JSON 格式详解

JSON 提供者支持两种格式：

### 扁平格式
```json
{
  "App.Title": "My App",
  "App.Version": "1.0",
  "Features.HotReload": "Hot reload support"
}
```

### 嵌套格式
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

两种格式生成的键相同：`App.Title`、`App.Version`、`Features.HotReload`。

推荐使用嵌套格式，结构更清晰，特别适合大型项目。

## API 参考

### ILanguageService

核心语言服务接口：

| 属性/方法 | 描述 |
|-----------------|-------------|
| `CurrentLanguage` | 获取或设置当前语言 |
| `AvailableLanguages` | 获取所有可用语言列表 |
| `this[string key]` | 获取指定键的本地化字符串 |
| `GetString(string key)` | 获取本地化字符串 |
| `GetString(string key, CultureInfo? culture)` | 获取指定区域性的本地化字符串 |
| `Format(string key, params object[] args)` | 格式化本地化字符串 |
| `LanguageChanged` | 语言更改事件 |

### ILocalizationProvider

数据源提供者接口，用于自定义实现：

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

## 提供者选项

### JsonLocalizationProvider

| 选项 | 描述 | 默认值 |
|--------|-------------|---------|
| `BasePath` | JSON 文件所在目录 | `"Localization"` |
| `FilePattern` | 文件匹配模式 | `"*.json"` |
| `UseEmbeddedResources` | 是否从嵌入资源加载 | `false` |
| `Assembly` | 指定程序集（嵌入资源模式） | 调用程序集 |

### ResxLocalizationProvider

| 选项 | 描述 | 默认值 |
|--------|-------------|---------|
| `ResourceType` | RESX 资源文件类型 | 必需 |
| `AutoDetectCultures` | 是否自动检测可用区域性 | `true` |
| `KnownCultures` | 手动指定已知区域性列表 | `null` |

## 自定义提供者

实现 `ILocalizationProvider` 接口来创建自定义数据源：

```csharp
public class DatabaseLocalizationProvider : ILocalizationProvider
{
    public string Name => "Database";

    public IEnumerable<CultureInfo> GetAvailableCultures()
    {
        // 从数据库获取支持的语言
    }

    public string? GetString(string key, CultureInfo culture)
    {
        // 从数据库获取本地化字符串
    }

    public bool TryGetString(string key, CultureInfo culture, out string? value)
    {
        value = GetString(key, culture);
        return value != null;
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        // 重新加载数据
    }
}
```

## 架构

```
┌─────────────────────────────────────────────────────────────────┐
│                      DynamicLocalization                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                  DynamicLocalization.Core               │    │
│  │  - ILanguageService, LanguageService                    │    │
│  │  - ILocalizationProvider, Providers                     │    │
│  │  - 平台无关逻辑                                          │    │
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

## 许可证

MIT License
