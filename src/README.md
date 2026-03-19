# Source Code (`src`)

This folder contains all C# source code for the **Azure Content Understanding – Multiple Agents** demo.

## Structure

| Folder / File | Purpose |
|---|---|
| *(to be added)* | Agent orchestration logic using [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview) |
| *(to be added)* | Document Intelligence integration using [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview) |
| *(to be added)* | Model Context Protocol (MCP) validation rules |
| *(to be added)* | Shared utilities and helpers |

## Technology Stack

- **Language:** C# (.NET 9 or later)
- **Framework:** [Azure AI Foundry SDK](https://learn.microsoft.com/azure/ai-foundry/how-to/develop/sdk-overview) / [Azure SDK for .NET](https://learn.microsoft.com/dotnet/azure/)
- **AI Orchestration:** [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview)
- **Document Processing:** [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview)

## Getting Started

1. Ensure you have [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed.
2. Copy `.env.example` to `.env` and fill in your Azure resource connection strings.
3. Build the solution:

   ```bash
   dotnet build
   ```

4. Run the application:

   ```bash
   dotnet run --project <ProjectName>
   ```

## Related Resources

- [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry)
- [Azure AI Agent Patterns](https://learn.microsoft.com/azure/ai-services/agents/concepts/agents)
- [C# Documentation](https://learn.microsoft.com/dotnet/csharp/)
