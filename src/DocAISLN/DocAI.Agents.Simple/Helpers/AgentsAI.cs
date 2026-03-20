using Azure.AI.Inference;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using DocAI.Console.Agents;
using DocAI.Services.General;
using Spectre.Console;

namespace DocAI.Console.Helpers;

public class AgentsRead(string endpointMain, string endpointMini, string mainModelName, string miniModelName)
{
    public async Task AnalyzeAsync(string filePath)
    {
        AnsiConsole.WriteLine("╔════════════════════════════════════════════════════════════╗");
        AnsiConsole.WriteLine("║         PDF Data Extraction Multi-Agent System             ║");
        AnsiConsole.WriteLine("╚════════════════════════════════════════════════════════════╝");
        var credential = new DefaultAzureCredential();
        var clientOptions = new AzureAIInferenceClientOptions();
        var tokenPolicy = new BearerTokenAuthenticationPolicy(credential,
            ["https://cognitiveservices.azure.com/.default"]);
        clientOptions.AddPolicy(tokenPolicy, HttpPipelinePosition.PerRetry);
        var orchestratorModel =
            new SimpleChatClient(new ChatCompletionsClient(new Uri(endpointMain), credential, clientOptions), mainModelName);
        var miniModel = new SimpleChatClient(new ChatCompletionsClient(new Uri(endpointMini), credential, clientOptions), miniModelName);

        var orchestratorAgent = new OrchestratorAgent(new PdfService(),
            new PdfReaderAgent(miniModel), 
            new AnalyzerAgent(orchestratorModel), 
            new ValidatorAgent(miniModel),
            orchestratorModel);
        var (data, validation) = await orchestratorAgent.ProcessPdfAsync(filePath);
        // Display valid items
        if (validation.ValidItems.Count > 0)
        {
            System.Console.WriteLine("\n✅ VALID DATA:");
            foreach (var item in validation.ValidItems)
            {
                System.Console.WriteLine($"   [{item.Type}] {item.Value}");
                if (!string.IsNullOrEmpty(item.Reason))
                    System.Console.WriteLine($"      → {item.Reason}");
            }
        }

        // Display invalid items
        if (validation.InvalidItems.Count > 0)
        {
            System.Console.WriteLine("\n❌ INVALID DATA:");
            foreach (var item in validation.InvalidItems)
            {
                System.Console.WriteLine($"   [{item.Type}] {item.Value}");
                if (!string.IsNullOrEmpty(item.Reason))
                    System.Console.WriteLine($"      → {item.Reason}");
            }
        }

        // Display unconfirmed items
        if (validation.UnconfirmedItems.Count > 0)
        {
            System.Console.WriteLine("\n⚠️  UNCONFIRMED DATA:");
            foreach (var item in validation.UnconfirmedItems)
            {
                System.Console.WriteLine($"   [{item.Type}] {item.Value}");
                if (!string.IsNullOrEmpty(item.Reason))
                    System.Console.WriteLine($"      → {item.Reason}");
            }
        }

        System.Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║                        SUMMARY                             ║");
        System.Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine(
            $"\nTotal Items Extracted: {validation.ValidItems.Count + validation.InvalidItems.Count + validation.UnconfirmedItems.Count}");
        System.Console.WriteLine($"✅ Valid: {validation.ValidItems.Count}");
        System.Console.WriteLine($"❌ Invalid: {validation.InvalidItems.Count}");
        System.Console.WriteLine($"⚠️  Unconfirmed: {validation.UnconfirmedItems.Count}");
        System.Console.WriteLine($"\nOverall Status: {(validation.IsValid ? "✅ PASSED" : "❌ FAILED")}");
    }
}