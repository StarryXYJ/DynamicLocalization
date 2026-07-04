# DynamicLocalization

[![MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/DynamicLocalization.Core.svg)](https://www.nuget.org/packages/DynamicLocalization.Core/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/DynamicLocalization.Core.svg)](https://www.nuget.org/packages/DynamicLocalization.Core/)

[English](README.md)

一个轻量级、可扩展、可插拔的国际化库，支持热重载和多种数据源，适用于 Avalonia 和 WPF。

## 特性

- **多语言** — 任意数量的语言，运行时动态切换
- **热重载** — 切换语言无需重启
- **可插拔** — 支持 JSON、YAML、RESX 提供者，可扩展至任何数据源
- **插件支持** — 动态注册/注销Provider，适用于插件架构
- **XAML 友好** — 为 Avalonia 和 WPF 提供简洁的标记扩展
- **DI 集成** — 完整的 `Microsoft.Extensions.DependencyInjection` 支持

## 包

| 包 | 描述 |
|---------|-------------|
| [DynamicLocalization.Core](https://www.nuget.org/packages/DynamicLocalization.Core/) | 核心库，平台无关 |
| [DynamicLocalization.Avalonia](https://www.nuget.org/packages/DynamicLocalization.Avalonia/) | Avalonia 平台（标记扩展） |
| [DynamicLocalization.WPF](https://www.nuget.org/packages/DynamicLocalization.WPF/) | WPF 平台（标记扩展） |

## 安装

```xml
<!-- Avalonia -->
<PackageReference Include="DynamicLocalization.Avalonia" />
<!-- 或 WPF -->
<PackageReference Include="DynamicLocalization.WPF" />
```

## 快速开始

### 1. 创建本地化文件

**JSON** — `Localization/en.json`（支持扁平或嵌套格式）：

```json
{
  "App.Title": "My Application",
  "App": { "Title": "My Application" }      // 等价
}
```

**YAML** — `Localization/en.yml`：

```yaml
App.Title: My Application
# 或嵌套格式：
App:
  Title: My Application
```

**RESX** — Visual Studio 资源文件（`Resources/Strings.resx`）。

### 2. 配置服务

```csharp
// 选择一个或多个提供者组合
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

### 3. 在 XAML 中使用

```xml
<TextBlock Text="{loc:Localize App.Title}" />
<TextBlock Text="{loc:Localize Greeting, StringFormat='你好, {0}!'}" />
```

### 4. 在代码中使用

```csharp
var greeting = cultureService["Greeting"];
cultureService.SetCulture("zh-CN");
cultureService.CultureChanged += (s, e) => Console.WriteLine($"已切换到 {e.NewCulture.Name}");
```

## Provider详情

### JSON

- 文件：`{BasePath}/{culture}.json`
- 字符串：`provider.LoadJsonString(json, "en")`
- 支持扁平与嵌套格式

### YAML

- 文件：`{BasePath}/{culture}.yml`
- 字符串：`provider.LoadYamlString(yaml, "en")`
- 支持扁平与嵌套格式、列表值

### RESX

- 使用 Visual Studio `.resx` 设计器文件
- 自动检测附属程序集

详见 **[spec/providers.md](spec/providers.md)**（所有选项、格式及编程用法）。

## API 参考

详见 **[spec/api-reference.md](spec/api-reference.md)**（ICultureService、ILocalizationProvider、LocalizedString、DI 扩展）。

## 扩展

- 实现 `ILocalizationProvider` 创建自定义数据源
- 继承 `JsonLocalizationProvider` / `YamlLocalizationProvider` 重写解析逻辑
- 动态注册/注销提供者，适用于插件场景

详见 **[spec/extending.md](spec/extending.md)**（自定义提供者、继承、插件集成）。

## 许可证

MIT License
