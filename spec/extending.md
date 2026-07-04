# Extending DynamicLocalization

## Custom Provider

Implement `ILocalizationProvider` to create your own data source (database, API, etc.):

```csharp
public class DatabaseLocalizationProvider : ILocalizationProvider
{
    public string Name => "Database";

    public IEnumerable<CultureInfo> GetAvailableCultures() { /* ... */ }
    public string? GetString(string key, CultureInfo culture) { /* ... */ }
    public bool TryGetString(string key, CultureInfo culture, out string? value) { /* ... */ }
    public Task ReloadAsync(CancellationToken cancellationToken = default) { /* ... */ }
}
```

## Provider Inheritance

Both `JsonLocalizationProvider` and `YamlLocalizationProvider` have `protected virtual` methods for easy customization.

### Extend JsonLocalizationProvider

```csharp
public class CustomJsonProvider : JsonLocalizationProvider
{
    public override string Name => "CustomJson";

    protected override string? ExtractCultureName(string resourceName)
    {
        // Custom resource name parsing
        return base.ExtractCultureName(resourceName);
    }

    protected override Dictionary<string, string>? ParseJsonToFlatDictionary(string json)
    {
        // Custom parsing logic
        return base.ParseJsonToFlatDictionary(json);
    }
}
```

### Extend YamlLocalizationProvider

```csharp
public class CustomYamlProvider : YamlLocalizationProvider
{
    public override string Name => "CustomYaml";

    protected override Dictionary<string, string>? ParseYamlToFlatDictionary(string yaml)
    {
        // Custom YAML handling
        return base.ParseYamlToFlatDictionary(yaml);
    }
}
```

### Overridable Members

**JsonLocalizationProvider:**

| Member | Description |
|--------|-------------|
| `Name` | Provider identifier |
| `LoadAll()` | Load all resources |
| `LoadFromEmbeddedResources()` | Load from embedded resources |
| `LoadFromFiles()` | Load from file system |
| `ExtractCultureName()` | Extract culture from resource name |
| `ParseJsonToFlatDictionary()` | Parse JSON to dictionary |
| `FlattenJsonObject()` | Flatten nested JSON objects |
| `TryGetFromCulture()` | Get string from specific culture |

**YamlLocalizationProvider:**

| Member | Description |
|--------|-------------|
| `Name` | Provider identifier |
| `LoadAll()` | Load all resources |
| `LoadFromFiles()` | Load from file system |
| `ParseYamlToFlatDictionary()` | Parse YAML to dictionary |
| `FlattenYamlObject()` | Flatten nested YAML objects |
| `TryGetFromCulture()` | Get string from specific culture |

## Plugin Integration

Providers support dynamic registration/unregistration, ideal for plugin architectures.

```csharp
public class PluginEntryPoint
{
    private readonly ICultureService _cultureService;
    private readonly PluginLocalizationProvider _provider;

    public PluginEntryPoint(ICultureService cultureService)
    {
        _cultureService = cultureService;
        _provider = new PluginLocalizationProvider();
    }

    public void Initialize()
    {
        _cultureService.RegisterProvider(_provider);
        // UI automatically refreshes with plugin translations
    }

    public void Unload()
    {
        _cultureService.UnregisterProvider(_provider.Name);
        // UI removes plugin translations
    }
}
```

### Key Naming Convention

Use a prefix to avoid conflicts:

| Format | Example |
|--------|---------|
| `{PluginName}.{Feature}.{Item}` | `MyPlugin.Menu.Open` |
| `{PluginName}.{Item}` | `MyPlugin.Title` |
