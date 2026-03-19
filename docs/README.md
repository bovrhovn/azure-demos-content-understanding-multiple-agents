# Documentation (`docs`)

This folder contains all project documentation for the **Azure Content Understanding – Multiple Agents** demo.

## Contents

| Document | Description |
|---|---|
| *(to be added)* | Architecture overview and design decisions |
| *(to be added)* | Setup and configuration guide |
| *(to be added)* | Agent pattern descriptions |
| *(to be added)* | Troubleshooting and FAQ |

## Architecture Overview

The solution uses a **multi-agent pattern** built on Azure AI Foundry to extract, validate, and reason over document content:

```
┌─────────────────────────────────────────────────────────┐
│                     Orchestrator Agent                  │
│           (coordinates the multi-agent workflow)        │
└────────────┬───────────────────────┬────────────────────┘
             │                       │
     ┌───────▼────────┐    ┌─────────▼──────────┐
     │ Document Intel │    │  Validation Agent  │
     │    Agent       │    │   (MCP / Rules)    │
     └───────┬────────┘    └─────────┬──────────┘
             │                       │
     ┌───────▼───────────────────────▼──────────┐
     │          Azure AI Document Intelligence  │
     │       (layout, form, table extraction)   │
     └──────────────────────────────────────────┘
```

## Key Concepts

- **[Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry)** – unified platform for building, evaluating, and deploying AI models and agents.
- **[Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview)** – managed service for creating and running intelligent AI agents.
- **[Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview)** – AI-powered document extraction and understanding.
- **[Model Context Protocol (MCP)](https://modelcontextprotocol.io/) ↗** – open standard for providing context to AI models (external specification site).

## References

- [Azure AI Agent Patterns](https://learn.microsoft.com/azure/ai-services/agents/concepts/agents)
- [Azure AI Foundry Documentation](https://learn.microsoft.com/azure/ai-foundry/)
- [Azure AI Document Intelligence Documentation](https://learn.microsoft.com/azure/ai-services/document-intelligence/)
- [C# / .NET Documentation](https://learn.microsoft.com/dotnet/csharp/)
