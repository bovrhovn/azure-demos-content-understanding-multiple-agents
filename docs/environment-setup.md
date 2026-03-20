# Environment Setup Guide

This guide provides comprehensive instructions for configuring environment variables required to run the Azure Content Understanding Multiple Agents demo.

---

## Table of Contents

- [Overview](#overview)
- [Quick Setup](#quick-setup)
- [Environment Variables Reference](#environment-variables-reference)
- [Configuration Scenarios](#configuration-scenarios)
- [Authentication](#authentication)
- [Troubleshooting](#troubleshooting)
- [Additional Resources](#additional-resources)

---

## Overview

The DocAI application requires several environment variables to connect to Azure services:

- **Azure AI Foundry / OpenAI** - For AI agent orchestration and model deployment
- **Azure Document Intelligence** - For document layout and content extraction
- **Model Configuration** - Deployment names and model IDs

---

## Quick Setup

### Automated Configuration (Recommended)

#### Windows (PowerShell)

```powershell
# Navigate to project root
cd azure-demos-content-understanding-multiple-agents

# Run setup script
.\scripts\setup-env.ps1

# To reset and reconfigure
.\scripts\setup-env.ps1 -Reset
```

#### Linux / macOS (Bash)

```bash
# Navigate to project root
cd azure-demos-content-understanding-multiple-agents

# Make script executable
chmod +x scripts/setup-env.sh

# Run setup script
./scripts/setup-env.sh

# To reset and reconfigure
./scripts/setup-env.sh --reset
```

### Manual Configuration

#### Windows

```powershell
# Set environment variables for current user
[Environment]::SetEnvironmentVariable("FOUNDRYENDPOINTMAIN", "https://your-foundry.openai.azure.com/", "User")
[Environment]::SetEnvironmentVariable("ORCHESTRATORMODEL", "gpt-4o", "User")
[Environment]::SetEnvironmentVariable("DOCENDPOINT", "https://your-doc-intel.cognitiveservices.azure.com/", "User")

# Restart terminal to load new variables
```

#### Linux / macOS

```bash
# Add to ~/.bashrc or ~/.zshrc
echo 'export FOUNDRYENDPOINTMAIN="https://your-foundry.openai.azure.com/"' >> ~/.bashrc
echo 'export ORCHESTRATORMODEL="gpt-4o"' >> ~/.bashrc
echo 'export DOCENDPOINT="https://your-doc-intel.cognitiveservices.azure.com/"' >> ~/.bashrc

# Reload shell configuration
source ~/.bashrc
```

---

## Environment Variables Reference

### Required Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `FOUNDRYENDPOINTMAIN` | Primary Azure AI Foundry or OpenAI endpoint | `https://contoso.openai.azure.com/` |
| `ORCHESTRATORMODEL` | Main model deployment name | `gpt-4o` |
| `DOCENDPOINT` | Azure Document Intelligence service endpoint | `https://contoso-docai.cognitiveservices.azure.com/` |

### Optional Variables

| Variable | Description | Default | Example |
|----------|-------------|---------|---------|
| `FOUNDRYENDPOINTMINI` | Fallback AI endpoint for smaller models | Same as `FOUNDRYENDPOINTMAIN` | `https://contoso-mini.openai.azure.com/` |
| `MINIMODEL` | Secondary model deployment name | `gpt-4o-mini` | `gpt-4o-mini` |
| `DOCMODELID` | Document Intelligence model ID | `prebuilt-layout` | `prebuilt-read`, `prebuilt-invoice` |

### Configuration File Alternative

Instead of environment variables, you can configure settings in `appsettings.json`:

```json
{
  "AzureAI": {
    "FoundryEndpointMain": "https://your-foundry.openai.azure.com/",
    "FoundryEndpointMini": "https://your-foundry.openai.azure.com/",
    "MainModelName": "gpt-4o",
    "MiniModelName": "gpt-4o-mini",
    "DocumentEndpoint": "https://your-doc-intel.cognitiveservices.azure.com/",
    "DocumentModelId": "prebuilt-layout"
  }
}
```

⚠️ **Security Note**: Never commit `appsettings.json` with real credentials to source control. Use `appsettings.Development.json` (gitignored) or environment variables for sensitive data.

---

## Configuration Scenarios

### Local Development

For local development with Azure services:

```bash
# Azure AI Foundry endpoint (includes /openai suffix)
FOUNDRYENDPOINTMAIN=https://your-resource.openai.azure.com/openai

# Use gpt-4o for primary processing
ORCHESTRATORMODEL=gpt-4o

# Azure Document Intelligence endpoint
DOCENDPOINT=https://your-docai.cognitiveservices.azure.com/

# Use prebuilt-layout for general documents
DOCMODELID=prebuilt-layout
```

### Docker / Container Apps

For containerized deployment, pass environment variables at runtime:

```bash
# Docker
docker run \
  -e FOUNDRYENDPOINTMAIN=https://your-foundry.openai.azure.com/ \
  -e ORCHESTRATORMODEL=gpt-4o \
  -e DOCENDPOINT=https://your-docai.cognitiveservices.azure.com/ \
  -e DOCMODELID=prebuilt-layout \
  -p 8080:8080 \
  docai-web

# Azure Container Apps
az containerapp create \
  --name docai-web \
  --resource-group docai-rg \
  --environment docai-env \
  --image docai-web:latest \
  --env-vars \
    FOUNDRYENDPOINTMAIN=https://your-foundry.openai.azure.com/ \
    ORCHESTRATORMODEL=gpt-4o \
    DOCENDPOINT=https://your-docai.cognitiveservices.azure.com/ \
    DOCMODELID=prebuilt-layout
```

### CI/CD Pipelines

For GitHub Actions or Azure DevOps:

#### GitHub Actions

```yaml
name: Build and Deploy

on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Build
        run: dotnet build src/DocAISLN/
        env:
          FOUNDRYENDPOINTMAIN: ${{ secrets.FOUNDRY_ENDPOINT }}
          ORCHESTRATORMODEL: ${{ secrets.ORCHESTRATOR_MODEL }}
          DOCENDPOINT: ${{ secrets.DOC_ENDPOINT }}
```

#### Azure DevOps

```yaml
variables:
  FOUNDRYENDPOINTMAIN: $(FoundryEndpoint)
  ORCHESTRATORMODEL: $(OrchestratorModel)
  DOCENDPOINT: $(DocEndpoint)

steps:
- task: DotNetCoreCLI@2
  inputs:
    command: 'build'
    projects: 'src/DocAISLN/**/*.csproj'
```

---

## Authentication

### Azure CLI Authentication (Recommended for Local Dev)

```bash
# Login to Azure
az login

# Set subscription
az account set --subscription "your-subscription-id"

# The application will use DefaultAzureCredential
# which automatically uses Azure CLI credentials
```

### Managed Identity (Recommended for Production)

For Azure-hosted applications (App Service, Container Apps, etc.):

1. Enable Managed Identity on your Azure resource
2. Grant the identity appropriate RBAC roles:
   - **Cognitive Services OpenAI User** - For Azure OpenAI
   - **Cognitive Services User** - For Document Intelligence

```bash
# Get managed identity principal ID
PRINCIPAL_ID=$(az webapp identity show \
  --name your-app-name \
  --resource-group your-rg \
  --query principalId -o tsv)

# Assign role for Azure OpenAI
az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role "Cognitive Services OpenAI User" \
  --scope /subscriptions/{subscription-id}/resourceGroups/{rg}/providers/Microsoft.CognitiveServices/accounts/{account-name}

# Assign role for Document Intelligence
az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role "Cognitive Services User" \
  --scope /subscriptions/{subscription-id}/resourceGroups/{rg}/providers/Microsoft.CognitiveServices/accounts/{doc-intel-account}
```

### Environment Variables for Keys (Not Recommended)

If you must use API keys:

```bash
# Add API key variables
export AZURE_OPENAI_API_KEY="your-openai-key"
export AZURE_DOCUMENT_INTELLIGENCE_KEY="your-doc-intel-key"
```

⚠️ **Security Warning**: API keys should only be used for development. Use Managed Identity or Azure CLI authentication for production.

---

## Troubleshooting

### Verify Environment Variables

#### Windows

```powershell
# List all DocAI-related variables
Get-ChildItem Env: | Where-Object { $_.Name -like "*FOUNDRY*" -or $_.Name -like "*DOC*" }

# Check specific variable
$env:FOUNDRYENDPOINTMAIN
```

#### Linux / macOS

```bash
# List all DocAI-related variables
env | grep -E "FOUNDRY|DOC|MODEL"

# Check specific variable
echo $FOUNDRYENDPOINTMAIN
```

### Common Issues

#### 1. "Endpoint not configured" Error

**Cause**: Environment variables not set or not loaded.

**Solution**:
```bash
# Verify variable is set
echo $FOUNDRYENDPOINTMAIN

# If empty, run setup script again or set manually
# Then restart your terminal/IDE
```

#### 2. "Authentication failed" Error

**Cause**: Invalid credentials or insufficient permissions.

**Solution**:
```bash
# For Azure CLI auth, re-login
az login
az account set --subscription "your-subscription"

# For Managed Identity, verify RBAC roles are assigned
az role assignment list --assignee {principal-id}
```

#### 3. "Model not found" Error

**Cause**: Model deployment name doesn't match Azure deployment.

**Solution**:
```bash
# List available models
az cognitiveservices account deployment list \
  --name your-openai-resource \
  --resource-group your-rg

# Update environment variable to match deployment name
export ORCHESTRATORMODEL="your-actual-deployment-name"
```

#### 4. Variables Not Persisting

**Windows**: Ensure you're setting at "User" scope, not "Process"
```powershell
[Environment]::SetEnvironmentVariable("VAR", "value", "User")
# NOT "Process"
```

**Linux/macOS**: Ensure you've added to shell RC file and reloaded
```bash
source ~/.bashrc  # or ~/.zshrc
```

### Debug Mode

Enable verbose logging to diagnose issues:

```bash
# Set logging level
export Logging__LogLevel__Default=Debug

# Run application
dotnet run --project src/DocAISLN/DocAI.Web/
```

---

## Additional Resources

### Azure Documentation

- 📘 [Azure AI Foundry Documentation](https://learn.microsoft.com/azure/ai-foundry/)
- 🔑 [Azure Authentication Best Practices](https://learn.microsoft.com/azure/developer/intro/passwordless-overview)
- 🔒 [Azure RBAC Documentation](https://learn.microsoft.com/azure/role-based-access-control/)

### Project Documentation

- [Model Context Protocol Integration](./model-context-protocol.md)
- [Architecture Overview](./README.md)
- [Main Project README](../README.md)

### Tools

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Visual Studio Code Azure Extensions](https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-node-azure-pack)
- [Azure PowerShell](https://learn.microsoft.com/powershell/azure/install-azure-powershell)

---

## Support

For issues or questions:

1. Check [GitHub Issues](https://github.com/bovrhovn/azure-demos-content-understanding-multiple-agents/issues)
2. Review [Azure AI Troubleshooting](https://learn.microsoft.com/azure/ai-services/openai/troubleshooting)
3. Open a new issue with environment details and error messages

---

**Last Updated**: 2026-03-20  
**Maintained By**: Azure DocAI Team
