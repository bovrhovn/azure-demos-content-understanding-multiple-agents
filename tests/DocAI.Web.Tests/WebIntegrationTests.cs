using Microsoft.AspNetCore.Mvc.Testing;

namespace DocAI.Web.Tests;

/// <summary>
/// Integration tests for the DocAI.Web Razor Pages application.
/// Uses WebApplicationFactory to spin up an in-memory test server.
/// </summary>
public class WebIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WebIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task HomePage_ReturnsHtmlContentType()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal("text/html; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task HomePage_ContainsDocAIBranding()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("DocAI", content);
    }

    [Fact]
    public async Task PrivacyPage_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Info/Privacy");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PrivacyPage_ContainsPrivacyTitle()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Info/Privacy");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Privacy", content);
    }

    [Fact]
    public async Task NonExistentPage_Returns404()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/does-not-exist-at-all");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Info/Privacy")]
    public async Task AllMainPages_ReturnSuccessAndCorrectContentType(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        Assert.StartsWith("text/html", response.Content.Headers.ContentType?.MediaType);
    }
}
