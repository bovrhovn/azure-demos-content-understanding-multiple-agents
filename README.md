# Azure Content Understanding – Multiple Agents

> A modern C# demo that uses **multiple AI agents** built on [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry) to extract, validate, and reason over document content using [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview) and [Model Context Protocol (MCP)](https://modelcontextprotocol.io/).

---

## 📂 Repository Structure

| Folder | Description |
|---|---|
| [`src/`](./src/README.md) | C# source code – agent orchestration, Document Intelligence integration, MCP validation |
| [`docs/`](./docs/README.md) | Architecture diagrams, design decisions, setup guides |
| [`containers/`](./containers/README.md) | Dockerfiles and Kubernetes manifests for local and Azure deployment |

---

## 🧠 What This Demo Does

This solution demonstrates a **multi-agent pattern** where several specialized AI agents collaborate to:

1. **Ingest documents** – PDFs, images, and forms are sent to [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview) for structured extraction (tables, key-value pairs, layout).
2. **Validate content** – a validation agent applies business rules using the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) to ensure extracted data meets defined criteria.
3. **Reason and respond** – an orchestrator agent, powered by [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview), coordinates the workflow and surfaces results to the user.

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
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- An [Azure subscription](https://azure.microsoft.com/free/)
- An [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry) hub and project
- An [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/quickstarts/get-started-sdks-rest-api) resource

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/bovrhovn/azure-demos-content-understanding-multiple-agents.git
cd azure-demos-content-understanding-multiple-agents

# 2. Configure environment variables
cp .env.example .env
# Edit .env with your Azure connection strings

# 3. Build
dotnet build src/

# 4. Run
dotnet run --project src/<ProjectName>
```

See [`src/README.md`](./src/README.md) for detailed build and run instructions, and [`containers/README.md`](./containers/README.md) for Docker-based setup.

---

## 🛠️ Technology Stack

| Technology | Role |
|---|---|
| **C# / .NET 9** | Primary programming language and runtime |
| **Azure AI Foundry** | Agent hosting, model deployment, evaluation |
| **Azure AI Agent Service** | Multi-agent orchestration and tool use |
| **Azure AI Document Intelligence** | Document layout and content extraction |
| **Model Context Protocol (MCP)** | Standardized context passing between agents and models |
| **Docker / Azure Container Apps** | Containerised deployment |

---

## 📚 Key Resources

### Azure Services

- 📄 [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview) – extract text, tables, and structured data from documents.
- 🏭 [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry) – unified platform for building, evaluating, and deploying AI solutions.
- 🤖 [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview) – create and manage AI agents with tool use and memory.
- 🔗 [Azure AI Agent Patterns](https://learn.microsoft.com/azure/ai-services/agents/concepts/agents) – reference patterns for multi-agent architectures.

### Programming Language

- 💻 [C# Documentation](https://learn.microsoft.com/dotnet/csharp/) – language reference, tutorials, and best practices.
- 📦 [Azure SDK for .NET](https://learn.microsoft.com/dotnet/azure/) – client libraries for all Azure services.
- 🔵 [.NET 9 Release Notes](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9/overview) – what's new in .NET 9.

---

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## 📝 License

This project is licensed under the [MIT License](LICENSE).
