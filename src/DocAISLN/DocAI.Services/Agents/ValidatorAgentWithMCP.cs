using System.Text.Json;
using System.Text.Json.Serialization;
using DocAI.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ValidationResult = DocAI.Models.ValidationResult;

namespace DocAI.Services.Agents;

public class ValidatorAgentWithMCP(
    string mcpEndpoint,
    IChatClient client,
    ILogger<ValidatorAgent> logger)
{
    public async Task<ValidationResult> ValidateExtractedData(ExtractedData data)
    {
        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(mcpEndpoint),
            TransportMode = HttpTransportMode.StreamableHttp,
            ConnectionTimeout = TimeSpan.FromSeconds(30)
        });

        await using var mcpClient = await McpClient.CreateAsync(transport);
        var tools = await mcpClient.ListToolsAsync();
        //here we can use also agent to call - but we are directly calling the tool
        // var currentClient = new ChatClientBuilder(client)
        //     .UseFunctionInvocation()
        //     .Build();
        var toolResult = tools.FirstOrDefault(currentTool => currentTool.Name.ToUpper() == "VALIDATE");
        var validationResult = new ValidationResult { IsValid = false };
        
        if (toolResult == null) return validationResult;
        
        var response = await mcpClient.CallToolAsync(
            toolName: "Validate",
            arguments: new Dictionary<string, object?>
            {
                ["data"] = data
            }, cancellationToken: CancellationToken.None);
        
        var structured = response.StructuredContent; 

        if (structured is null)
        {
            // fallback: you can inspect response.Result.Content text blocks
            throw new InvalidOperationException("Tool returned no structuredContent.");
        }

        var validation = structured.Value.Deserialize<ValidationResult>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        }) ?? new ValidationResult { IsValid = false };

        return validationResult;
    }
}