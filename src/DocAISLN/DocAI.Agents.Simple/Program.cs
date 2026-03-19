using DocAI.Console.Helpers;
using Spectre.Console;

AnsiConsole.MarkupLine("[bold green]Welcome to document understanding tests![/]");

#region ENV variables

var docEndpoint = Environment.GetEnvironmentVariable("DOCENDPOINT");
ArgumentException.ThrowIfNullOrEmpty(docEndpoint, "Please set the DOCENDPOINT environment variable.");
AnsiConsole.MarkupLine($"Doc intelligence at: [bold blue]{docEndpoint}[/]");
var modelId = Environment.GetEnvironmentVariable("MODELID");
if (string.IsNullOrEmpty(modelId)) modelId = "prebuilt-layout";

AnsiConsole.MarkupLine($"Using model: [bold blue]{modelId}[/]");

var filePath = Environment.GetEnvironmentVariable("FILEPATH");
ArgumentException.ThrowIfNullOrEmpty(filePath, "Please set the FILEPATH environment variable.");
if (!new FileInfo(filePath).Exists)
{
    throw new ArgumentException("File doesn't exist. Pick another one.");
}

AnsiConsole.MarkupLine($"File to check at: [bold blue]{filePath}[/]");

var foundryEndpointMain = Environment.GetEnvironmentVariable("FOUNDRYENDPOINTMAIN");
ArgumentException.ThrowIfNullOrEmpty(foundryEndpointMain, "Please set the FOUNDRYENDPOINTMAIN environment variable.");
AnsiConsole.MarkupLine($"Azure Foundry at: [bold blue]{foundryEndpointMain}[/]");

var foundryEndpointMini = Environment.GetEnvironmentVariable("FOUNDRYENDPOINTMINI");
ArgumentException.ThrowIfNullOrEmpty(foundryEndpointMini, "Please set the FOUNDRYENDPOINTMINI environment variable.");
AnsiConsole.MarkupLine($"Azure Foundry at: [bold blue]{foundryEndpointMini}[/]");

var orchestratorModel = Environment.GetEnvironmentVariable("ORCHESTRATORMODEL");
ArgumentException.ThrowIfNullOrEmpty(orchestratorModel, "Please set the ORCHESTRATORMODEL environment variable.");
AnsiConsole.MarkupLine($"Orchestrator model: [bold blue]{orchestratorModel}[/]");

var miniModel = Environment.GetEnvironmentVariable("MINIMODEL");
ArgumentException.ThrowIfNullOrEmpty(miniModel, "Please set the MINIMODEL environment variable.");
AnsiConsole.MarkupLine($"Mini model: [bold blue]{miniModel}[/]");

#endregion

var docIntelligence = new DocAI.Console.Helpers.DocAI(docEndpoint, modelId);
await docIntelligence.AnalyzeAsync(filePath);

AnsiConsole.MarkupLine("[green]Done with Azure Document Intelligence.[/] Continue with agents.");

var agentsIntelligence = new AgentsRead(foundryEndpointMain,foundryEndpointMini, orchestratorModel, miniModel);
await agentsIntelligence.AnalyzeAsync(filePath);

AnsiConsole.MarkupLine("[green]Done with both options.[/].");