# DynamicLocalization.DependencyInjection

Microsoft.Extensions.DependencyInjection integration for DynamicLocalization.

## Installation

```bash
dotnet add package DynamicLocalization.DependencyInjection
```

## Usage

### Basic Setup

```csharp
using Microsoft.Extensions.DependencyInjection;
using DynamicLocalization.Core.Extensions;

var services = new ServiceCollection();

services.AddJsonLocalization(options =>
{
    options.BasePath = "Localization";
});

services.AddCultureService();

var serviceProvider = services.BuildServiceProvider().InitializeLocalization();
```

### With Embedded Resources

```csharp
services.AddJsonLocalization(options =>
{
    options.BasePath = "Localization";
    options.UseEmbeddedResources = true;
    options.Assembly = typeof(App).Assembly;
});
```

### With RESX Resources

```csharp
services.AddResxLocalization(options =>
{
    options.ResourceType = typeof(Resources.Strings);
});
```

## Extension Methods

| Method | Description |
|--------|-------------|
| `AddJsonLocalization` | Registers JSON localization provider |
| `AddResxLocalization` | Registers RESX localization provider |
| `AddCultureService` | Registers culture service with all providers |
| `InitializeLocalization` | Initializes static LocalizationService |

## Requirements

- .NET 6.0 or later
- DynamicLocalization.Core package (automatically installed as dependency)