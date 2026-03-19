# Containers (`containers`)

This folder contains all container-related definitions for the **Azure Content Understanding – Multiple Agents** demo.

## Contents

| File / Folder | Description |
|---|---|
| *(to be added)* | `Dockerfile` for the main application |
| *(to be added)* | `docker-compose.yml` for local multi-service orchestration |
| *(to be added)* | Kubernetes manifests for Azure Kubernetes Service (AKS) deployment |

## Running Locally with Docker

1. Build the image:

   ```bash
   docker build -t content-understanding-agents .
   ```

2. Run the container:

   ```bash
   docker run --env-file .env content-understanding-agents
   ```

3. (Optional) Use Docker Compose to start all services together:

   ```bash
   docker compose up --build
   ```

## Deploying to Azure

The containers can be deployed to:

- **[Azure Container Apps](https://learn.microsoft.com/azure/container-apps/overview)** – serverless containers with built-in scaling.
- **[Azure Kubernetes Service (AKS)](https://learn.microsoft.com/azure/aks/intro-kubernetes)** – managed Kubernetes for production workloads.
- **[Azure Container Registry (ACR)](https://learn.microsoft.com/azure/container-registry/container-registry-intro)** – private registry for storing and managing container images.

## Related Resources

- [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-ai-foundry)
- [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/overview)
- [Azure AI Agent Patterns](https://learn.microsoft.com/azure/ai-services/agents/concepts/agents)
- [C# / .NET Documentation](https://learn.microsoft.com/dotnet/csharp/)
