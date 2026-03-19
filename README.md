# Azure Content Understanding – Multiple Agents

> A modern C# demo that uses **multiple AI agents** built on [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry) to extract, validate, and reason over document content using [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview) and [Model Context Protocol (MCP)](https://modelcontextprotocol.io/).

---

## 📂 Repository Structure

| Folder | Description |
|---|---|
| [`src/`](./src/README.md) | C# source code – agent orchestration, Document Intelligence integration, MCP validation, and the web frontend |
| [`tests/`](./tests/) | Unit and integration tests for the API and web projects; includes Playwright end-to-end tests |
| [`docs/`](./docs/README.md) | Architecture diagrams, design decisions, and setup guides |
| [`containers/`](./containers/README.md) | Dockerfiles for local and Azure container deployment |

---

## 🧠 What This Demo Does

This solution demonstrates a **multi-agent pattern** where several specialized AI agents collaborate to:

1. **Ingest documents** – PDFs, images, and forms are sent to [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview) for structured extraction (tables, key-value pairs, layout).
2. **Validate content** – a validation agent applies business rules using the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) to ensure extracted data meets defined criteria.
3. **Reason and respond** – an orchestrator agent, powered by [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview), coordinates the workflow and surfaces results to the user.
4. **Present results** – a modern ASP.NET Core web frontend (`DocAI.Web`) displays the pipeline status and results.

### High-Level Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                      Orchestrator Agent                      │
│             (Azure AI Agent Service on AI Foundry)           │
└────────────┬────────────────────────┬────────────────────────┘
             │                        │
     ┌───────▼────────┐     ┌─────────▼──────────┐
     │ Document Intel │     │  Validation Agent  │
     │    Agent       │     │   (MCP / Rules)    │
     └───────┬────────┘     └─────────┬──────────┘
             │                        │
     ┌───────▼────────────────────────▼──────────┐
     │         Azure AI Document Intelligence    │
     │      (layout, form, table extraction)     │
     └───────────────────────────────────────────┘
                          │
     ┌────────────────────▼────────────────────┐
     │              DocAI.Web                  │
     │   (ASP.NET Core Razor Pages frontend)   │
     └─────────────────────────────────────────┘
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- An [Azure subscription](https://azure.microsoft.com/free/)
- An [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry) hub and project
- An [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/quickstarts/get-started-sdks-rest-api) resource

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/bovrhovn/azure-demos-content-understanding-multiple-agents.git
cd azure-demos-content-understanding-multiple-agents

# 2. Configure environment variables
export DOCENDPOINT="https://<your-doc-intelligence>.cognitiveservices.azure.com/"
export FOUNDRYENDPOINTMAIN="https://<your-foundry-endpoint>/openai"
export FOUNDRYENDPOINTMINI="https://<your-foundry-endpoint>/openai"
export ORCHESTRATORMODEL="gpt-4o"
export MINIMODEL="gpt-4o-mini"
export MODELID="prebuilt-layout"
export FILEPATH="/path/to/document.pdf"

# 3. Build the solution
dotnet build src/DocAISLN/

# 4. Run the web frontend
dotnet run --project src/DocAISLN/DocAI.Web/

# 5. Run the MCP Validator service
dotnet run --project src/DocAISLN/DocAI.MCP.Validator/

# 6. Run the agent pipeline (requires all environment variables)
dotnet run --project src/DocAISLN/DocAI.Agents.Simple/
```

### Running with Docker

```bash
# Build and run the MCP Validator container
docker build -f containers/MCP-Validator -t docai-mcp-validator ./src/DocAISLN
docker run -p 8080:8080 docai-mcp-validator

# Build and run the Web frontend container
docker build -f containers/DocAI-Web -t docai-web ./src/DocAISLN
docker run -p 8081:8080 docai-web
```

See [`containers/README.md`](./containers/README.md) for full Docker and Azure deployment instructions.

### Running Tests

```bash
# Run all tests
dotnet test tests/

# Run only API tests
dotnet test tests/DocAI.MCP.Validator.Tests/

# Run only web integration tests
dotnet test tests/DocAI.Web.Tests/

# Run Playwright end-to-end tests (requires Playwright browsers installed)
dotnet test tests/DocAI.Web.Playwright/
```

---

## 🛠️ Technology Stack

| Technology | Role |
|---|---|
| **C# / .NET 10** | Primary programming language and runtime |
| **Azure AI Foundry** | Agent hosting, model deployment, evaluation |
| **Azure AI Agent Service** | Multi-agent orchestration and tool use |
| **Azure AI Document Intelligence** | Document layout and content extraction |
| **Model Context Protocol (MCP)** | Standardized context passing between agents and models |
| **ASP.NET Core Razor Pages** | Web frontend (`DocAI.Web`) |
| **Bootstrap 5 + Font Awesome** | Modern, responsive UI components |
| **Docker / Azure Container Apps** | Containerised deployment |
| **xUnit / Playwright** | Unit, integration, and end-to-end testing |

---

## 🧪 Testing

The `tests/` folder contains three test projects:

| Project | Type | Description |
|---|---|---|
| `DocAI.MCP.Validator.Tests` | Integration | Verifies the MCP Validator API starts and responds correctly |
| `DocAI.Web.Tests` | Integration | Tests Razor Pages routes, page rendering, and error handling |
| `DocAI.Web.Playwright` | End-to-end | Browser-based tests using [Microsoft Playwright](https://playwright.dev/dotnet/) |

---

## 📚 Key Resources

### Azure Services

- 📄 [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview) – extract text, tables, and structured data from documents.
- 🏭 [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry) – unified platform for building, evaluating, and deploying AI solutions.
- 🤖 [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview) – create and manage AI agents with tool use and memory.
- 🔗 [Azure AI Agent Patterns](https://learn.microsoft.com/azure/ai-services/agents/concepts/agents) – reference patterns for multi-agent architectures.

### Programming Language & Frameworks

- 💻 [C# Documentation](https://learn.microsoft.com/dotnet/csharp/) – language reference, tutorials, and best practices.
- 📦 [Azure SDK for .NET](https://learn.microsoft.com/dotnet/azure/) – client libraries for all Azure services.
- 🔵 [.NET 10 Release Notes](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/overview) – what's new in .NET 10.
- 🎭 [Playwright for .NET](https://playwright.dev/dotnet/) – reliable end-to-end browser testing.

---

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## 📝 License

This project is licensed under the [MIT License](LICENSE).
