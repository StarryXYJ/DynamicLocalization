# Avalonia.DynamicLocalization

一个轻量级、可扩展、可插拔的 Avalonia 国际化库，支持热更新和多数据源。

## 特性

- 🌍 **多语言支持** - 支持任意数量的语言
- 🔄 **热更新** - 运行时动态切换语言，无需重启
- 🔌 **可插拔架构** - 支持自定义数据源提供者
- 📦 **JSON 支持** - 内置 JSON 本地化文件支持
- 📄 **RESX 支持** - 内置 RESX 资源文件支持
- 🎯 **XAML 友好** - 提供简洁的 XAML 标记扩展
- 💉 **DI 集成** - 完整依赖注入支持

# 快速开始

## 安装
### 使用 NuGet 包管理器安装：

Avalonia.DynamicLocalization

### 包引用
```xml
<PackageReference Include="Avalonia.DynamicLocalization"  />
```


## 快速开始

### 一、数据源与服务配置

#### 方法一、使用 JSON 文件

##### 1. 创建本地化文件

在项目中创建 `Localization` 文件夹，添加 JSON 文件：

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

##### 2. 配置服务

**方式一：从文件系统加载（默认）**

```csharp
services.AddJsonLocalization(options =>
{
    options.BasePath = "Localization";
    options.FilePattern = "*.json";
});
```

**方式二：从嵌入资源加载**

首先在 `.csproj` 中配置 JSON 文件为嵌入资源：

```xml
<ItemGroup>
  <EmbeddedResource Include="Localization\*.json" />
</ItemGroup>
```

大部分IDE也支持右键文件属性设置为嵌入资源。

然后配置服务：

```csharp
services.AddJsonLocalization(options =>
{
    options.BasePath = "Localization";
    options.UseEmbeddedResources = true;
    // options.Assembly = typeof(App).Assembly; // 可选，默认使用调用程序集
});
```

#### 方法二、使用 RESX 文件

##### 1. 创建 RESX 资源文件

在项目中添加 RESX 资源文件：

一般也是可以直接通过IDE的右键菜单添加资源文件来创建的

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
  <data name="WelcomeMessage" xml:space="preserve">
    <value>Welcome to our application.</value>
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
  <data name="WelcomeMessage" xml:space="preserve">
    <value>欢迎使用我们的应用程序。</value>
  </data>
</root>
```

##### 2. 配置服务

```csharp
services.AddResxLocalization(options =>
{
    options.ResourceType = typeof(Resources.Strings);
});
```

#### 方法三、自定义数据源与组合

[自定义数据源见后文](#自定义-provider)

组合-可以同时注入多个数据源，同时使用则全部查找


#### 完整配置示例

在 `App.axaml.cs` 中配置服务：

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
        
        // 方式一：使用 JSON 文件
        services.AddJsonLocalization(options =>
        {
            options.BasePath = "Localization";
            options.FilePattern = "*.json";
        });

        // 方式二：使用 RESX 文件
        // services.AddResxLocalization(options =>
        // {
        //     options.ResourceType = typeof(Resources.Strings);
        // });

        // 配置语言服务
        services.AddLanguageService();

        // 初始化全局服务
        // Services = services.BuildServiceProvider().InitializeLocalization();
        services.BuildServiceProvider().InitializeLocalization();
        AvaloniaXamlLoader.Load(this);
    }
}
```

### 二、使用本地化字符串与属性

#### 在 XAML 中使用

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

#### 在 ViewModel 中使用

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
        // 更新绑定属性
    }
}
```

# 项目分析

## 核心架构

```
┌─────────────────────────────────────────────────────────────────┐
│                    Avalonia.DynamicLocalization                 │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                  ILanguageService                       │    │
│  │  - 核心服务接口                                           │    │
│  │  - 语言管理、字符串获取、事件通知                            │    │
│  └─────────────────────────────────────────────────────────┘    │
│                              │                                  │
│                              ▼                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                  ILocalizationProvider                  │    │
│  │  - 数据源提供者接口                                        │    │
│  │  - 定义统一的资源获取方式                                   │    │
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

## API 参考

### ILanguageService

核心语言服务接口：

| 属性/方法 | 说明 |
|-----------|------|
| `CurrentLanguage` | 获取或设置当前语言 |
| `AvailableLanguages` | 获取所有可用语言列表 |
| `this[string key]` | 获取指定键的本地化字符串 |
| `GetString(string key)` | 获取本地化字符串 |
| `GetString(string key, CultureInfo? culture)` | 获取指定文化的本地化字符串 |
| `Format(string key, params object[] args)` | 格式化本地化字符串 |
| `LanguageChanged` | 语言变更事件 |

### ILocalizationProvider

数据源提供者接口，可自定义实现：

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

XAML 标记扩展：

```xml
<!-- 基本用法 -->
<TextBlock Text="{loc:Localize KeyName}"/>

<!-- 带格式化 -->
<TextBlock Text="{loc:Localize KeyName, StringFormat='{}{0}!'}"/>
```

## 项目结构

```
Avalonia.DynamicLocalization/
├── Core/
│   ├── ILanguageService.cs          # 核心服务接口
│   ├── LanguageService.cs           # 核心服务实现
│   ├── LocalizedString.cs           # 可观察的本地化字符串
│   └── LanguageChangedEventArgs.cs  # 语言变更事件参数
│
├── Providers/
│   ├── ILocalizationProvider.cs     # 数据源提供者接口
│   ├── JsonLocalizationProvider.cs  # JSON 数据源实现
│   └── ResxLocalizationProvider.cs  # RESX 数据源实现
│
├── MarkupExtensions/
│   └── LocalizeExtension.cs         # XAML 标记扩展
│
├── Extensions/
│   └── ServiceCollectionExtensions.cs # DI 扩展方法
│
└── LocalizationService.cs           # 全局服务访问点
```

## 内置 Provider

### JsonLocalizationProvider

| 选项 | 说明 | 默认值 |
|------|------|--------|
| `BasePath` | JSON 文件所在目录 | `"Localization"` |
| `FilePattern` | 文件匹配模式 | `"*.json"` |
| `UseEmbeddedResources` | 是否从嵌入资源加载 | `false` |
| `Assembly` | 指定程序集（嵌入资源模式） | 调用程序集 |

### ResxLocalizationProvider

| 选项 | 说明 | 默认值 |
|------|------|--------|
| `ResourceType` | RESX 资源文件的类型 | 必填 |
| `AutoDetectCultures` | 是否自动检测可用文化 | `true` |
| `KnownCultures` | 手动指定已知文化列表 | `null` |

```csharp
// 自动检测（默认）
services.AddResxLocalization(options =>
{
    options.ResourceType = typeof(Resources.Strings);
});

// 手动指定文化列表
services.AddResxLocalization(options =>
{
    options.ResourceType = typeof(Resources.Strings);
    options.AutoDetectCultures = false;
    options.KnownCultures = new[] { "en", "zh-CN", "ja", "ko" };
});
```

# 可拓展内容

## 自定义 Provider

实现 `ILocalizationProvider` 接口创建自定义数据源：

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



# 许可证

MIT License
