# Containers (`containers`)

This folder contains all Dockerfiles for the **Azure Content Understanding – Multiple Agents** demo.

## Contents

| File | Description |
|---|---|
| [`MCP-Validator`](./MCP-Validator) | Dockerfile for the `DocAI.MCP.Validator` API service |
| [`DocAI-Web`](./DocAI-Web) | Dockerfile for the `DocAI.Web` Razor Pages frontend |

---

## Building Images

All Docker builds use `src/DocAISLN/` as the build context. Run the following commands from the **repository root**:

### MCP Validator

```bash
docker build -f containers/MCP-Validator -t docai-mcp-validator ./src/DocAISLN
```

### Web Frontend

```bash
docker build -f containers/DocAI-Web -t docai-web ./src/DocAISLN
```

---

## Running Locally with Docker

### MCP Validator

```bash
docker run -p 8080:8080 docai-mcp-validator
# API available at http://localhost:8080
```

### Web Frontend

```bash
docker run -p 8081:8080 docai-web
# Web app available at http://localhost:8081
```

### Running Both Services Together

```bash
docker run -d -p 8080:8080 --name mcp-validator docai-mcp-validator
docker run -d -p 8081:8080 --name docai-web docai-web
```

---

## Deploying to Azure

The containers can be deployed to:

- **[Azure Container Apps](https://learn.microsoft.com/azure/container-apps/overview)** – serverless containers with built-in scaling and ingress.
- **[Azure Kubernetes Service (AKS)](https://learn.microsoft.com/azure/aks/intro-kubernetes)** – managed Kubernetes for production workloads.
- **[Azure Container Registry (ACR)](https://learn.microsoft.com/azure/container-registry/container-registry-intro)** – private registry for storing and managing container images.

### Push to Azure Container Registry

```bash
# Login to ACR
az acr login --name <your-acr-name>

# Tag and push MCP Validator
docker tag docai-mcp-validator <your-acr-name>.azurecr.io/docai-mcp-validator:latest
docker push <your-acr-name>.azurecr.io/docai-mcp-validator:latest

# Tag and push Web Frontend
docker tag docai-web <your-acr-name>.azurecr.io/docai-web:latest
docker push <your-acr-name>.azurecr.io/docai-web:latest
```

### Deploy to Azure Container Apps

```bash
# Deploy MCP Validator
az containerapp create \
  --name docai-mcp-validator \
  --resource-group <your-rg> \
  --environment <your-env> \
  --image <your-acr-name>.azurecr.io/docai-mcp-validator:latest \
  --target-port 8080 \
  --ingress external

# Deploy Web Frontend
az containerapp create \
  --name docai-web \
  --resource-group <your-rg> \
  --environment <your-env> \
  --image <your-acr-name>.azurecr.io/docai-web:latest \
  --target-port 8080 \
  --ingress external
```

---

## Related Resources

- [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry)
- [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview)
- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Azure Container Registry Documentation](https://learn.microsoft.com/azure/container-registry/)
- [C# / .NET Documentation](https://learn.microsoft.com/dotnet/csharp/)
