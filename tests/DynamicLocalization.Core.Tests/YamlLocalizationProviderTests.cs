using System.Globalization;
using System.Linq;
using DynamicLocalization.Core.Providers;
using Xunit;

namespace DynamicLocalization.Core.Tests;

public class YamlLocalizationProviderTests
{
    [Fact]
    public void LoadYamlString_WithFlatYaml_ShouldPopulateCache()
    {
        var provider = new YamlLocalizationProvider();
        var yaml = """
                    App.Title: My App
                    App.Greeting: Hello
                    """;

        provider.LoadYamlString(yaml, "en");

        var cultures = provider.GetAvailableCultures().ToList();
        Assert.Single(cultures);
        Assert.Equal("en", cultures[0].Name);
        Assert.Equal("My App", provider.GetString("App.Title", new CultureInfo("en")));
        Assert.Equal("Hello", provider.GetString("App.Greeting", new CultureInfo("en")));
    }

    [Fact]
    public void LoadYamlString_WithNestedYaml_ShouldFlattenKeys()
    {
        var provider = new YamlLocalizationProvider();
        var yaml = """
                    App:
                      Title: My App
                      Description: A great app
                    """;

        provider.LoadYamlString(yaml, "en");

        Assert.Equal("My App", provider.GetString("App.Title", new CultureInfo("en")));
        Assert.Equal("A great app", provider.GetString("App.Description", new CultureInfo("en")));
    }

    [Fact]
    public void LoadYamlString_WithMultipleCultures_ShouldSupportAll()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("App.Title: My App", "en");
        provider.LoadYamlString("App.Title: 我的应用", "zh-CN");

        Assert.Equal("My App", provider.GetString("App.Title", new CultureInfo("en")));
        Assert.Equal("我的应用", provider.GetString("App.Title", new CultureInfo("zh-CN")));
    }

    [Fact]
    public void GetString_WithExactMatch_ShouldReturnValue()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("key: value", "en");

        var result = provider.GetString("key", new CultureInfo("en"));

        Assert.Equal("value", result);
    }

    [Fact]
    public void GetString_WithMissingKey_ShouldReturnNull()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("existing: value", "en");

        var result = provider.GetString("nonexistent", new CultureInfo("en"));

        Assert.Null(result);
    }

    [Fact]
    public void GetString_WithCultureFallback_ShouldFallbackToParentCulture()
    {
        var provider = new YamlLocalizationProvider();
        // zh-CN's parent is zh-Hans, and zh-Hans's parent is zh
        provider.LoadYamlString("title: Hello", "zh-Hans");

        var result = provider.GetString("title", new CultureInfo("zh-CN"));

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void GetString_WithNoFallback_ShouldReturnNull()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("title: Hello", "en");

        var result = provider.GetString("title", new CultureInfo("zh-CN"));

        Assert.Null(result);
    }

    [Fact]
    public void TryGetString_WithExistingKey_ShouldReturnTrue()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("key: value", "en");

        var success = provider.TryGetString("key", new CultureInfo("en"), out var value);

        Assert.True(success);
        Assert.Equal("value", value);
    }

    [Fact]
    public void TryGetString_WithMissingKey_ShouldReturnFalse()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("key: value", "en");

        var success = provider.TryGetString("missing", new CultureInfo("en"), out var value);

        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void GetAvailableCultures_AfterLoadingMultiple_ShouldReturnAll()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("a: 1", "en");
        provider.LoadYamlString("a: 2", "zh-CN");
        provider.LoadYamlString("a: 3", "ja");

        var cultures = provider.GetAvailableCultures().Select(c => c.Name).OrderBy(x => x).ToList();

        Assert.Equal(3, cultures.Count);
        Assert.Equal(["en", "ja", "zh-CN"], cultures);
    }

    [Fact]
    public async System.Threading.Tasks.Task ReloadAsync_ShouldClearCache()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("key: value", "en");

        Assert.Single(provider.GetAvailableCultures());

        await provider.ReloadAsync();

        Assert.Empty(provider.GetAvailableCultures());
    }

    [Fact]
    public void Name_ShouldReturnYaml()
    {
        var provider = new YamlLocalizationProvider();
        Assert.Equal("Yaml", provider.Name);
    }

    [Fact]
    public void LoadYamlString_WithEmptyCulture_ShouldOverwritePrevious()
    {
        var provider = new YamlLocalizationProvider();
        provider.LoadYamlString("key: old", "en");
        provider.LoadYamlString("key: new", "en");

        Assert.Equal("new", provider.GetString("key", new CultureInfo("en")));
    }

    [Fact]
    public void LoadYamlString_WithDeeplyNestedYaml_ShouldFlattenCorrectly()
    {
        var provider = new YamlLocalizationProvider();
        var yaml = """
                    a:
                      b:
                        c: deep
                    """;

        provider.LoadYamlString(yaml, "en");

        Assert.Equal("deep", provider.GetString("a.b.c", new CultureInfo("en")));
    }

    [Fact]
    public void LoadYamlString_WithListValue_ShouldJoinWithComma()
    {
        var provider = new YamlLocalizationProvider();
        var yaml = """
                    items:
                      - apple
                      - banana
                      - cherry
                    """;

        provider.LoadYamlString(yaml, "en");

        Assert.Equal("apple, banana, cherry", provider.GetString("items", new CultureInfo("en")));
    }

    [Fact]
    public void LoadYamlString_WithIntegerValue_ShouldConvertToString()
    {
        var provider = new YamlLocalizationProvider();
        var yaml = "count: 42";

        provider.LoadYamlString(yaml, "en");

        Assert.Equal("42", provider.GetString("count", new CultureInfo("en")));
    }

    [Fact]
    public void LoadYamlString_WithBooleanValue_ShouldConvertToString()
    {
        var provider = new YamlLocalizationProvider();
        var yaml = "enabled: true";

        provider.LoadYamlString(yaml, "en");

        Assert.Equal("true", provider.GetString("enabled", new CultureInfo("en")));
    }

    [Fact]
    public void Initialize_WithFileOptions_ShouldNotThrow()
    {
        var provider = new YamlLocalizationProvider();
        var options = new YamlLocalizationProviderOptions
        {
            BasePath = "NonExistentPath_XYZ",
            FilePattern = "*.yml"
        };

        var exception = Record.Exception(() => provider.Initialize(options));

        Assert.Null(exception);
    }

    [Fact]
    public void LoadYamlString_WithInvalidYaml_ShouldNotThrow()
    {
        var provider = new YamlLocalizationProvider();
        var invalidYaml = ": : invalid";

        var exception = Record.Exception(() => provider.LoadYamlString(invalidYaml, "en"));

        Assert.Null(exception);
    }
}
