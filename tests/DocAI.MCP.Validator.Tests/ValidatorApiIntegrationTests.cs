using Microsoft.AspNetCore.Mvc.Testing;

namespace DocAI.MCP.Validator.Tests;

/// <summary>
/// Integration tests for the DocAI.MCP.Validator API service.
/// Uses WebApplicationFactory to spin up an in-memory test server.
/// </summary>
public class ValidatorApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ValidatorApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Application_StartsAndResponds()
    {
        var client = _factory.CreateClient();

        // A GET to any non-existent path will still return a response (not throw a connection error),
        // which proves the application started successfully.
        var response = await client.GetAsync("/health");

        // The app has no health endpoint defined, so 404 is expected – but the app is running.
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.NotFound ||
            response.IsSuccessStatusCode,
            $"Unexpected status code: {response.StatusCode}");
    }

    [Fact]
    public async Task OpenApiEndpoint_IsAvailableInDevelopment()
    {
        // Configure factory to use Development environment so MapOpenApi() is registered.
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json",
            response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task OpenApiEndpoint_IsNotAvailableInProduction()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
        });

        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UnknownEndpoint_Returns404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/does-not-exist");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
