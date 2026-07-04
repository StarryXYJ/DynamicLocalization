using System.Globalization;
using System.Linq;
using DynamicLocalization.Core.Providers;
using Xunit;

namespace DynamicLocalization.Core.Tests;

public class JsonLocalizationProviderTests
{
    [Fact]
    public void LoadJsonString_WithFlatJson_ShouldPopulateCache()
    {
        var provider = new JsonLocalizationProvider();
        var json = """{"App.Title":"My App","App.Greeting":"Hello"}""";

        provider.LoadJsonString(json, "en");

        var cultures = provider.GetAvailableCultures().ToList();
        Assert.Single(cultures);
        Assert.Equal("en", cultures[0].Name);
        Assert.Equal("My App", provider.GetString("App.Title", new CultureInfo("en")));
        Assert.Equal("Hello", provider.GetString("App.Greeting", new CultureInfo("en")));
    }

    [Fact]
    public void LoadJsonString_WithNestedJson_ShouldFlattenKeys()
    {
        var provider = new JsonLocalizationProvider();
        var json = """{"App":{"Title":"My App","Description":"A great app"}}""";

        provider.LoadJsonString(json, "en");

        Assert.Equal("My App", provider.GetString("App.Title", new CultureInfo("en")));
        Assert.Equal("A great app", provider.GetString("App.Description", new CultureInfo("en")));
    }

    [Fact]
    public void LoadJsonString_WithMultipleCultures_ShouldSupportAll()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"App.Title":"My App"}""", "en");
        provider.LoadJsonString("""{"App.Title":"我的应用"}""", "zh-CN");

        Assert.Equal("My App", provider.GetString("App.Title", new CultureInfo("en")));
        Assert.Equal("我的应用", provider.GetString("App.Title", new CultureInfo("zh-CN")));
    }

    [Fact]
    public void GetString_WithExactMatch_ShouldReturnValue()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"key":"value"}""", "en");

        var result = provider.GetString("key", new CultureInfo("en"));

        Assert.Equal("value", result);
    }

    [Fact]
    public void GetString_WithMissingKey_ShouldReturnNull()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"existing":"value"}""", "en");

        var result = provider.GetString("nonexistent", new CultureInfo("en"));

        Assert.Null(result);
    }

    [Fact]
    public void GetString_WithCultureFallback_ShouldFallbackToParentCulture()
    {
        var provider = new JsonLocalizationProvider();
        // zh-CN's parent is zh-Hans, and zh-Hans's parent is zh
        provider.LoadJsonString("""{"title":"Hello"}""", "zh-Hans");

        var result = provider.GetString("title", new CultureInfo("zh-CN"));

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void GetString_WithNoFallback_ShouldReturnNull()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"title":"Hello"}""", "en");

        var result = provider.GetString("title", new CultureInfo("zh-CN"));

        Assert.Null(result);
    }

    [Fact]
    public void TryGetString_WithExistingKey_ShouldReturnTrue()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"key":"value"}""", "en");

        var success = provider.TryGetString("key", new CultureInfo("en"), out var value);

        Assert.True(success);
        Assert.Equal("value", value);
    }

    [Fact]
    public void TryGetString_WithMissingKey_ShouldReturnFalse()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"key":"value"}""", "en");

        var success = provider.TryGetString("missing", new CultureInfo("en"), out var value);

        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void GetAvailableCultures_AfterLoadingMultiple_ShouldReturnAll()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"a":"1"}""", "en");
        provider.LoadJsonString("""{"a":"2"}""", "zh-CN");
        provider.LoadJsonString("""{"a":"3"}""", "ja");

        var cultures = provider.GetAvailableCultures().Select(c => c.Name).OrderBy(x => x).ToList();

        Assert.Equal(3, cultures.Count);
        Assert.Equal(["en", "ja", "zh-CN"], cultures);
    }

    [Fact]
    public async System.Threading.Tasks.Task ReloadAsync_ShouldClearCache()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"key":"value"}""", "en");

        Assert.Single(provider.GetAvailableCultures());

        await provider.ReloadAsync();

        Assert.Empty(provider.GetAvailableCultures());
    }

    [Fact]
    public void Name_ShouldReturnJson()
    {
        var provider = new JsonLocalizationProvider();
        Assert.Equal("Json", provider.Name);
    }

    [Fact]
    public void LoadJsonString_WithEmptyCulture_ShouldOverwritePrevious()
    {
        var provider = new JsonLocalizationProvider();
        provider.LoadJsonString("""{"key":"old"}""", "en");
        provider.LoadJsonString("""{"key":"new"}""", "en");

        Assert.Equal("new", provider.GetString("key", new CultureInfo("en")));
    }

    [Fact]
    public void LoadJsonString_WithDeeplyNestedJson_ShouldFlattenCorrectly()
    {
        var provider = new JsonLocalizationProvider();
        var json = """{"a":{"b":{"c":"deep"}}}""";

        provider.LoadJsonString(json, "en");

        Assert.Equal("deep", provider.GetString("a.b.c", new CultureInfo("en")));
    }

    [Fact]
    public void Initialize_WithFileOptions_ShouldNotThrow()
    {
        var provider = new JsonLocalizationProvider();
        var options = new JsonLocalizationProviderOptions
        {
            BasePath = "NonExistentPath_XYZ",
            FilePattern = "*.json"
        };

        var exception = Record.Exception(() => provider.Initialize(options));

        Assert.Null(exception);
    }
}
