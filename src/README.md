# Source Code (`src`)

This folder contains all C# source code for the **Azure Content Understanding – Multiple Agents** demo.

## Structure

```
src/DocAISLN/
├── DocAISLN.slnx                    # Solution file (.NET 10)
├── DocAI.Agents.Simple/             # Multi-agent console application
│   ├── Agents/                      # Agent implementations
│   │   ├── OrchestratorAgent.cs     # Coordinates the pipeline
│   │   ├── PdfReaderAgent.cs        # Reads and extracts PDF content
│   │   ├── AnalyzerAgent.cs         # Extracts structured data
│   │   └── ValidatorAgent.cs        # Applies business rules
│   ├── Helpers/                     # Utility classes
│   │   ├── AgentsAI.cs              # Azure AI Agent orchestration
│   │   ├── DocAI.cs                 # Document Intelligence integration
│   │   └── PdfService.cs            # PDF processing service
│   └── Models/                      # Data models
│       ├── ExtractedData.cs
│       ├── ValidationResult.cs
│       ├── Person.cs
│       ├── EmailAddress.cs
│       └── TableData.cs
├── DocAI.MCP.Validator/             # MCP Validator API service
│   ├── Program.cs                   # Application startup
│   └── appsettings.json
├── DocAI.Models/                    # Shared data models library
├── DocAI.Services/                  # Shared services library
└── DocAI.Web/                       # Web frontend (Razor Pages)
    ├── Pages/
    │   ├── Docs/
    │   │   └── UploadAndProcess.cshtml   # File upload & processing page
    │   ├── Info/                    # Application pages
    │   │   ├── Index.cshtml         # Home page
    │   │   ├── Privacy.cshtml       # Privacy policy page
    │   │   └── Error.cshtml         # Error page
    │   └── Shared/
    │       └── _Layout.cshtml       # Master layout with Bootstrap + Font Awesome
    ├── wwwroot/                     # Static assets
    │   ├── css/site.css             # Custom styles (includes drop-zone styles)
    │   └── js/site.js               # Custom scripts
    └── Program.cs                   # Application startup
```

---

## Projects

### DocAI.Agents.Simple

A console application that demonstrates the full **multi-agent document processing pipeline**:

1. Reads a PDF document using Azure AI Document Intelligence or PdfPig
2. Analyzes the content with an Azure AI chat model
3. Extracts structured data (persons, emails, tables)
4. Validates the data against business rules
5. Outputs results using Spectre.Console

**Required environment variables:**

| Variable | Description |
|---|---|
| `DOCENDPOINT` | Azure AI Document Intelligence endpoint |
| `FILEPATH` | Path to the PDF document |
| `FOUNDRYENDPOINTMAIN` | Azure AI Foundry endpoint (primary model) |
| `FOUNDRYENDPOINTMINI` | Azure AI Foundry endpoint (mini model) |
| `ORCHESTRATORMODEL` | Primary model deployment name |
| `MINIMODEL` | Lightweight model deployment name |
| `MODELID` | Document Intelligence model ID (`prebuilt-layout`) |

**Run:**

```bash
dotnet run --project src/DocAISLN/DocAI.Agents.Simple/
```

---

### DocAI.MCP.Validator

A standalone **ASP.NET Core Web API** that exposes document validation rules via the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/). Other agents can call this service to validate extracted data.

**Key packages:**
- `Microsoft.AspNetCore.OpenApi` – OpenAPI / Swagger documentation (available in Development only)
- `ModelContextProtocol` – MCP protocol implementation

**Run:**

```bash
dotnet run --project src/DocAISLN/DocAI.MCP.Validator/
# OpenAPI UI available at http://localhost:5000/openapi/v1.json (Development only)
# MCP endpoint available at http://localhost:5000/mcp
```

**Docker:**

```bash
docker build -f containers/MCP-Validator -t docai-mcp-validator ./src/DocAISLN
docker run -p 8080:8080 docai-mcp-validator
```

---

### DocAI.Web

A modern **ASP.NET Core Razor Pages** web application that provides a browser-based interface for the Document AI pipeline. Built with Bootstrap 5 and Font Awesome for a polished, responsive UI.

**Features:**
- Home page with pipeline overview and quick-start cards
- **Upload & Process page** – drag-and-drop file upload with live file preview, progress indicator, and step-by-step pipeline explanation
- Privacy policy page
- Responsive navigation with Font Awesome icons
- Modern card-based layout with gradient accents

**Pages:**

| Page | Route | Description |
|---|---|---|
| Home | `/` | Pipeline overview, key components, and technology stack |
| Upload & Process | `/Docs/UploadAndProcess` | Drag-and-drop PDF upload form with agent pipeline explanation |
| Privacy | `/Info/Privacy` | Privacy policy |

**Run:**

```bash
dotnet run --project src/DocAISLN/DocAI.Web/
# Web app available at http://localhost:5001
```

**Docker:**

```bash
docker build -f containers/DocAI-Web -t docai-web ./src/DocAISLN
docker run -p 8081:8080 docai-web
```

---

## Building the Solution

```bash
# Build the entire solution
dotnet build src/DocAISLN/

# Build a specific project
dotnet build src/DocAISLN/DocAI.Web/
dotnet build src/DocAISLN/DocAI.MCP.Validator/
dotnet build src/DocAISLN/DocAI.Agents.Simple/

# Build in Release configuration
dotnet build src/DocAISLN/ -c Release
```

---

## Technology Stack

- **Language:** C# (.NET 10)
- **Framework:** [Azure AI Foundry SDK](https://learn.microsoft.com/azure/ai-foundry/how-to/develop/sdk-overview) / [Azure SDK for .NET](https://learn.microsoft.com/dotnet/azure/)
- **AI Orchestration:** [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview)
- **Document Processing:** [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview)
- **Web UI:** Bootstrap 5, Font Awesome 6, ASP.NET Core Razor Pages
- **Testing:** xUnit, Microsoft.AspNetCore.Mvc.Testing, Playwright for .NET

---

## Related Resources

- [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry)
- [Azure AI Agent Patterns](https://learn.microsoft.com/azure/ai-services/agents/concepts/agents)
- [C# Documentation](https://learn.microsoft.com/dotnet/csharp/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.3/)
- [Font Awesome Documentation](https://fontawesome.com/docs)
