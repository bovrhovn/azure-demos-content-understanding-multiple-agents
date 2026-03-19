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

1. **Ingest documents** – PDFs are uploaded through a modern web interface with drag-and-drop support
2. **Process with agents** – A multi-agent pipeline (PDF Reader → Analyzer → Validator → Orchestrator) extracts structured data
3. **Alternative processing** – Azure AI Document Intelligence provides direct document analysis with prebuilt models
4. **File management** – Browse uploaded files, select any document for processing, or upload new files
5. **Present results** – Modern web UI displays extracted persons, emails, tables, and validation results

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    DocAI.Web (Frontend)                         │
│  File Upload • File Selection • Results Display • Progress      │
└──────────────────┬─────────────────────────┬────────────────────┘
                   │                         │
       ┌───────────▼─────────────┐  ┌────────▼─────────────────┐
       │  Multi-Agent Pipeline   │  │  Azure Doc Intelligence  │
       │  ┌────────────────────┐ │  │  Direct API Integration  │
       │  │ OrchestratorAgent  │ │  │                          │
       │  │  ┌──────────────┐  │ │  │  Prebuilt Models:        │
       │  │  │ PdfReaderAgt │  │ │  │  - Layout extraction     │
       │  │  │ AnalyzerAgent│  │ │  │  - Table detection       │
       │  │  │ ValidatorAgt │  │ │  │  - Text recognition      │
       │  │  └──────────────┘  │ │  │                          │
       │  └────────────────────┘ │  └──────────────────────────┘
       └─────────────────────────┘
                   │
       ┌───────────▼─────────────────┐
       │   DocAI.Services Layer      │
       │ • IDocumentProcessingService│
       │ • IAzureDocIntellService    │
       │ • PdfService                │
       └─────────────────────────────┘
```

### Key Features

✅ **File Management** - Upload, list, and select PDF documents  
✅ **Dual Processing Modes** - Choose between multi-agent pipeline or Azure Document Intelligence  
✅ **Real-time Progress** - Visual feedback during document processing  
✅ **Rich Results Display** - Structured view of persons, emails, tables with validation status  
✅ **Modern UI** - Responsive Bootstrap 5 interface with drag-and-drop upload  
✅ **Background Processing** - Non-blocking document analysis with TempData results  
✅ **Comprehensive Testing** - Integration tests (xUnit) and E2E tests (Playwright)  

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- An [Azure subscription](https://azure.microsoft.com/free/)
- An [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry) hub and project
- An [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/quickstarts/get-started-sdks-rest-api) resource

### Configuration

Update `src/DocAISLN/DocAI.Web/appsettings.json` or set environment variables:

```json
{
  "AzureAI": {
    "FoundryEndpointMain": "https://<your-foundry-endpoint>/openai",
    "FoundryEndpointMini": "https://<your-foundry-endpoint>/openai",
    "MainModelName": "gpt-4o",
    "MiniModelName": "gpt-4o-mini",
    "DocumentEndpoint": "https://<your-doc-intelligence>.cognitiveservices.azure.com/",
    "DocumentModelId": "prebuilt-layout"
  }
}
```

Or use environment variables:

```bash
export FOUNDRYENDPOINTMAIN="https://<your-foundry-endpoint>/openai"
export FOUNDRYENDPOINTMINI="https://<your-foundry-endpoint>/openai"
export ORCHESTRATORMODEL="gpt-4o"
export MINIMODEL="gpt-4o-mini"
export DOCENDPOINT="https://<your-doc-intelligence>.cognitiveservices.azure.com/"
export DOCMODELID="prebuilt-layout"
```

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/bovrhovn/azure-demos-content-understanding-multiple-agents.git
cd azure-demos-content-understanding-multiple-agents

# 2. Build the solution
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
# Run web integration tests (no Azure credentials required)
dotnet test tests/DocAI.Web.Tests/

# Run MCP Validator integration tests (no Azure credentials required)
dotnet test tests/DocAI.MCP.Validator.Tests/

# Run Playwright end-to-end tests (requires Playwright browsers – see below)
dotnet test tests/DocAI.Web.Playwright/

# Install Playwright browsers (run once after first build)
dotnet build tests/DocAI.Web.Playwright/
pwsh tests/DocAI.Web.Playwright/bin/Debug/net10.0/playwright.ps1 install --with-deps chromium
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
| `DocAI.MCP.Validator.Tests` | Integration | Verifies the MCP Validator API starts, responds correctly, and exposes OpenAPI in Development |
| `DocAI.Web.Tests` | Integration | Tests Razor Pages routes (Home, Privacy, Upload & Process), page rendering, and error handling |
| `DocAI.Web.Playwright` | End-to-end | Browser-based tests using [Microsoft Playwright](https://playwright.dev/dotnet/) |

> The web integration and MCP validator integration tests do **not** require Azure credentials and can be run offline.

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
