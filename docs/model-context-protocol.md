# Model Context Protocol (MCP) Integration

This document explains how the DocAI project uses the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) to enable standardized communication between AI agents and models.

---

## Table of Contents

- [What is Model Context Protocol?](#what-is-model-context-protocol)
- [MCP in DocAI Architecture](#mcp-in-docai-architecture)
- [MCP Validator Service](#mcp-validator-service)
- [Using MCP with AI Agents](#using-mcp-with-ai-agents)
- [MCP Server Implementation](#mcp-server-implementation)
- [Testing MCP Integration](#testing-mcp-integration)
- [Best Practices](#best-practices)
- [Additional Resources](#additional-resources)

---

## What is Model Context Protocol?

The **Model Context Protocol (MCP)** is an open standard for context sharing between large language models (LLMs) and external systems. It provides:

- **Standardized Context Format** - Consistent way to pass context to models
- **Tool Integration** - Structured method for models to use external tools
- **Multi-Agent Communication** - Protocol for agents to share information
- **Context Management** - Efficient handling of context windows

### Key Benefits

✅ **Interoperability** - Works across different AI models and frameworks  
✅ **Extensibility** - Easy to add new tools and context sources  
✅ **Efficiency** - Optimized context usage reduces token consumption  
✅ **Standardization** - Industry-standard protocol for AI applications  

---

## MCP in DocAI Architecture

The DocAI solution uses MCP in two primary ways:

### 1. Validator Agent Communication

```
┌─────────────────────┐      MCP Protocol      ┌──────────────────┐
│  Analyzer Agent     │ ──────────────────────> │ Validator Agent  │
│  (Extracts Data)    │                         │  (Validates Data)│
└─────────────────────┘                         └──────────────────┘
         │                                               │
         │                                               │
         └──────────── Structured Context ──────────────┘
              (Persons, Emails, Tables, etc.)
```

### 2. Multi-Agent Pipeline Orchestration

```
┌──────────────────────────────────────────────────────┐
│              Orchestrator Agent                       │
│  (Coordinates agents using MCP context passing)      │
└───────────┬──────────────┬─────────────┬─────────────┘
            │              │             │
   ┌────────▼────────┐ ┌──▼──────────┐ ┌▼──────────────┐
   │  PDF Reader     │ │  Analyzer   │ │  Validator    │
   │  Agent          │ │  Agent      │ │  Agent (MCP)  │
   └─────────────────┘ └─────────────┘ └───────────────┘
```

---

## MCP Validator Service

The **DocAI.MCP.Validator** project implements an MCP server that validates extracted document data.

### Service Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/validate` | POST | Validates extracted data against business rules |
| `/health` | GET | Health check endpoint |
| `/swagger` | GET | OpenAPI documentation (Development only) |

### Validation Request Format

```json
{
  "persons": [
    {
      "name": "John Doe",
      "title": "CEO",
      "organization": "Contoso Ltd",
      "confidence": 95
    }
  ],
  "emails": [
    {
      "email": "john.doe@contoso.com",
      "context": "Contact Information"
    }
  ],
  "tables": [
    {
      "title": "Q1 Financial Summary",
      "headers": ["Category", "Amount"],
      "rows": [
        ["Revenue", "$1,000,000"],
        ["Expenses", "$750,000"]
      ]
    }
  ]
}
```

### Validation Response Format

```json
{
  "isValid": true,
  "validItems": [
    {
      "type": "Person",
      "value": "John Doe",
      "confidence": 95
    }
  ],
  "invalidItems": [],
  "unconfirmedItems": [],
  "summary": "3 items validated: 3 valid, 0 invalid, 0 unconfirmed"
}
```

---

## Using MCP with AI Agents

### Setting Up MCP Context

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Create kernel with MCP support
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: "gpt-4o",
        endpoint: azureEndpoint,
        apiKey: azureApiKey)
    .Build();

// Create MCP context for validation
var mcpContext = new
{
    persons = extractedPersons,
    emails = extractedEmails,
    tables = extractedTables,
    metadata = new
    {
        documentId = documentId,
        processedAt = DateTime.UtcNow
    }
};

// Call MCP validator service
var validationResult = await httpClient.PostAsJsonAsync(
    "http://localhost:8080/validate",
    mcpContext);
```

### Agent Prompt with MCP Context

```csharp
var prompt = @$"
You are a data validation agent. Review the following extracted data
and determine if it meets quality standards.

Context (MCP Format):
{{
  ""persons"": {JsonSerializer.Serialize(mcpContext.persons)},
  ""emails"": {JsonSerializer.Serialize(mcpContext.emails)},
  ""tables"": {JsonSerializer.Serialize(mcpContext.tables)}
}}

Task: Validate each item and flag any concerns.
";

var result = await kernel.InvokePromptAsync(prompt);
```

---

## MCP Server Implementation

### Minimal API with MCP Support

The validator service uses ASP.NET Core Minimal APIs:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MCP validation endpoint
app.MapPost("/validate", async (ValidationRequest request) =>
{
    var result = new ValidationResult
    {
        IsValid = true,
        ValidItems = new List<ValidationItem>(),
        InvalidItems = new List<ValidationItem>(),
        UnconfirmedItems = new List<ValidationItem>()
    };

    // Validate persons
    foreach (var person in request.Persons)
    {
        if (person.Confidence >= 80)
        {
            result.ValidItems.Add(new ValidationItem
            {
                Type = "Person",
                Value = person.Name,
                Confidence = person.Confidence
            });
        }
        else
        {
            result.UnconfirmedItems.Add(new ValidationItem
            {
                Type = "Person",
                Value = person.Name,
                Confidence = person.Confidence
            });
        }
    }

    // Similar validation for emails and tables...

    result.Summary = $"{result.ValidItems.Count + result.InvalidItems.Count + result.UnconfirmedItems.Count} items validated";
    
    return Results.Ok(result);
})
.WithName("ValidateData")
.WithOpenApi();

app.Run();
```

### Running the MCP Server

```bash
# Start the MCP Validator service
cd src/DocAISLN
dotnet run --project DocAI.MCP.Validator

# The service will be available at:
# http://localhost:8080
```

### Docker Deployment

```bash
# Build the Docker image
docker build -f containers/MCP-Validator -t docai-mcp-validator ./src/DocAISLN

# Run the container
docker run -p 8080:8080 docai-mcp-validator

# Test the service
curl http://localhost:8080/health
```

---

## Testing MCP Integration

### Unit Tests

The `DocAI.MCP.Validator.Tests` project includes integration tests:

```csharp
[Fact]
public async Task Validate_ReturnsSuccessForValidData()
{
    // Arrange
    var request = new ValidationRequest
    {
        Persons = new List<PersonData>
        {
            new() { Name = "John Doe", Confidence = 95 }
        },
        Emails = new List<EmailData>
        {
            new() { Email = "john@contoso.com" }
        },
        Tables = new List<TableData>()
    };

    // Act
    var response = await _client.PostAsJsonAsync("/validate", request);

    // Assert
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<ValidationResult>();
    
    Assert.True(result.IsValid);
    Assert.NotEmpty(result.ValidItems);
}
```

### Manual Testing with curl

```bash
# Test validation endpoint
curl -X POST http://localhost:8080/validate \
  -H "Content-Type: application/json" \
  -d '{
    "persons": [
      {
        "name": "Jane Smith",
        "title": "Manager",
        "organization": "Contoso",
        "confidence": 92
      }
    ],
    "emails": [
      {
        "email": "jane.smith@contoso.com",
        "context": "Business Contact"
      }
    ],
    "tables": []
  }'
```

### Playwright E2E Tests

```csharp
[Test]
public async Task ProcessDocument_UsesValidatorAgent()
{
    await Page.GotoAsync($"{BaseUrl}/Docs/UploadAndProcess");
    
    // Upload a test document
    await Page.Locator("#FormFile").SetInputFilesAsync("testdata/sample.pdf");
    await Page.Locator("#uploadBtn").ClickAsync();
    
    // Wait for processing to complete
    await Page.WaitForSelectorAsync(".validation-badge", new() { Timeout = 30000 });
    
    // Verify validation results are displayed
    var validBadge = Page.Locator(".validation-valid");
    await Expect(validBadge).ToBeVisibleAsync();
}
```

---

## Best Practices

### 1. Context Size Management

Keep MCP context payloads efficient:

```csharp
// ❌ Avoid sending entire document text
var mcpContext = new
{
    fullText = documentText,  // Can be very large
    extractedData = data
};

// ✅ Send only necessary structured data
var mcpContext = new
{
    summary = documentSummary,
    extractedData = data,
    metadata = new { pageCount, fileSize }
};
```

### 2. Error Handling

Implement robust error handling for MCP calls:

```csharp
try
{
    var response = await mcpClient.PostAsJsonAsync("/validate", context);
    
    if (!response.IsSuccessStatusCode)
    {
        _logger.LogWarning("MCP validation failed: {Status}", response.StatusCode);
        return new ValidationResult { IsValid = false };
    }
    
    return await response.Content.ReadFromJsonAsync<ValidationResult>();
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "Failed to connect to MCP validator service");
    return new ValidationResult { IsValid = false };
}
```

### 3. Versioning

Include version information in MCP requests:

```csharp
var mcpRequest = new
{
    version = "1.0",
    data = extractedData,
    timestamp = DateTime.UtcNow
};
```

### 4. Monitoring

Log MCP interactions for debugging:

```csharp
_logger.LogInformation(
    "Sending MCP validation request: {PersonCount} persons, {EmailCount} emails",
    request.Persons.Count,
    request.Emails.Count);
```

---

## Additional Resources

### Official Documentation

- 🌐 [Model Context Protocol Website](https://modelcontextprotocol.io/)
- 📘 [MCP Specification](https://spec.modelcontextprotocol.io/)
- 🔧 [MCP SDKs and Tools](https://github.com/modelcontextprotocol)

### Azure AI Services

- 🤖 [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/)
- 🏭 [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/)
- 💬 [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)

### Related Documentation

- [Environment Setup Guide](./environment-setup.md)
- [Architecture Overview](./README.md)
- [Main Project README](../README.md)

---

## Support and Contributions

For questions or issues related to MCP integration:

1. Check the [GitHub Issues](https://github.com/bovrhovn/azure-demos-content-understanding-multiple-agents/issues)
2. Review the [MCP Specification](https://spec.modelcontextprotocol.io/)
3. Join the discussion in the project's community forums

Contributions to improve MCP integration are welcome! Please see the [Contributing Guide](../README.md#contributing).
