# Documentation (`docs`)

This folder contains all project documentation for the **Azure Content Understanding – Multiple Agents** demo.

## Contents

| Document | Description |
|---|---|
| [Architecture Overview](#architecture-overview) | System design and agent interaction patterns |
| [Setup Guide](#setup-and-configuration) | Step-by-step environment configuration |
| [Agent Patterns](#agent-patterns) | Description of each agent and its responsibilities |
| [Troubleshooting](#troubleshooting) | Common issues and solutions |

---

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
                          │
     ┌────────────────────▼────────────────────┐
     │              DocAI.Web                  │
     │   (ASP.NET Core Razor Pages frontend)   │
     └─────────────────────────────────────────┘
```

### Component Breakdown

| Component | Project | Description |
|---|---|---|
| **Orchestrator Agent** | `DocAI.Agents.Simple` | Coordinates the document processing pipeline using Azure AI Agent Service |
| **PDF Reader Agent** | `DocAI.Agents.Simple` | Reads and extracts raw text from PDF documents |
| **Analyzer Agent** | `DocAI.Agents.Simple` | Extracts structured data (persons, emails, tables) from text |
| **Validator Agent** | `DocAI.Agents.Simple` | Applies business rules to validate extracted data |
| **MCP Validator Service** | `DocAI.MCP.Validator` | Standalone ASP.NET Core API providing validation rules via MCP |
| **Web Frontend** | `DocAI.Web` | Razor Pages web application with modern Bootstrap + Font Awesome UI |

---

## Setup and Configuration

### 1. Azure Resources

You need the following Azure resources:

| Resource | Purpose | Documentation |
|---|---|---|
| Azure AI Foundry Hub | Hosts AI models and agents | [Quickstart](https://learn.microsoft.com/azure/ai-foundry/quickstarts/get-started-playground) |
| Azure AI Document Intelligence | Extracts document content | [Quickstart](https://learn.microsoft.com/azure/ai-services/document-intelligence/quickstarts/get-started-sdks-rest-api) |
| Azure Container Registry (optional) | Stores Docker images | [Quickstart](https://learn.microsoft.com/azure/container-registry/container-registry-get-started-portal) |

### 2. Environment Variables

Set the following environment variables before running the agent pipeline:

| Variable | Description | Example |
|---|---|---|
| `DOCENDPOINT` | Azure AI Document Intelligence endpoint URL | `https://myresource.cognitiveservices.azure.com/` |
| `FOUNDRYENDPOINTMAIN` | Azure AI Foundry endpoint for primary model | `https://myhub.openai.azure.com/` |
| `FOUNDRYENDPOINTMINI` | Azure AI Foundry endpoint for lightweight model | `https://myhub.openai.azure.com/` |
| `ORCHESTRATORMODEL` | Model deployment name for orchestration | `gpt-4o` |
| `MINIMODEL` | Model deployment name for lightweight tasks | `gpt-4o-mini` |
| `MODELID` | Document Intelligence model ID | `prebuilt-layout` |
| `FILEPATH` | Path to the document to process | `/data/sample.pdf` |

### 3. Authentication

The solution uses **DefaultAzureCredential** for authentication. This supports:

- Local development: Azure CLI (`az login`)
- Production: Managed Identity on Azure Container Apps / AKS

---

## Agent Patterns

### OrchestratorAgent

Coordinates the full document processing pipeline:
1. Invoke `PdfReaderAgent` to extract raw text
2. Invoke `AnalyzerAgent` to identify persons, emails, and tables
3. Invoke `ValidatorAgent` to apply business rules
4. Return the final structured result

### PdfReaderAgent

Uses Azure AI Document Intelligence (`prebuilt-layout` model) to extract text content from PDF documents. Falls back to the `PdfPig` library for local processing.

### AnalyzerAgent

Uses an Azure AI chat model to analyze the extracted text and produce structured `ExtractedData` objects containing:
- `List<Person>` – identified persons with names and roles
- `List<EmailAddress>` – email addresses found in the document
- `List<TableData>` – tabular data extracted from the document

### ValidatorAgent

Applies a set of business validation rules to the extracted data:
- Required fields are present
- Email addresses are in valid format
- Tables contain expected columns
- Custom rules via the MCP Validator service

---

## Troubleshooting

### `DOCENDPOINT` not set

```
Error: Environment variable DOCENDPOINT is not set.
```

Make sure all required environment variables are exported. See [Setup and Configuration](#2-environment-variables).

### `DefaultAzureCredential` fails locally

Run `az login` to authenticate with the Azure CLI, or set the `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`, and `AZURE_TENANT_ID` environment variables for service principal authentication.

### Playwright tests fail with "browser not found"

Install Playwright browsers after restoring NuGet packages:

```bash
dotnet build tests/DocAI.Web.Playwright/
pwsh tests/DocAI.Web.Playwright/bin/Debug/net10.0/playwright.ps1 install --with-deps chromium
```

### Docker build fails

Ensure you run the Docker build from the `src/DocAISLN/` directory as the build context:

```bash
docker build -f ../../containers/MCP-Validator -t docai-mcp-validator .
```

---

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
- [Playwright for .NET](https://playwright.dev/dotnet/)
