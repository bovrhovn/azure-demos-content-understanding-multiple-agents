#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Configure environment variables for Azure DocAI Demo
.DESCRIPTION
    This script sets up all required environment variables for local development
    and testing of the Azure Content Understanding Multiple Agents demo.
.PARAMETER Profile
    The configuration profile to use: Local, Development, or Production
.PARAMETER Reset
    Clear all existing DocAI environment variables before setting new ones
.EXAMPLE
    .\setup-env.ps1 -Profile Local
.EXAMPLE
    .\setup-env.ps1 -Profile Development -Reset
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('Local', 'Development', 'Production')]
    [string]$Profile = 'Local',

    [Parameter(Mandatory=$false)]
    [switch]$Reset
)

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Azure DocAI - Environment Variable Setup" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Function to set environment variable
function Set-EnvVar {
    param(
        [string]$Name,
        [string]$Value,
        [string]$Description
    )
    
    if ([string]::IsNullOrWhiteSpace($Value)) {
        Write-Host "⚠️  Skipping $Name (no value provided)" -ForegroundColor Yellow
        return
    }
    
    [Environment]::SetEnvironmentVariable($Name, $Value, "User")
    Write-Host "✅ Set $Name" -ForegroundColor Green
    if ($Description) {
        Write-Host "   $Description" -ForegroundColor Gray
    }
}

# Function to clear DocAI environment variables
function Clear-DocAIEnvVars {
    Write-Host "🧹 Clearing existing DocAI environment variables..." -ForegroundColor Yellow
    
    $envVars = @(
        "FOUNDRYENDPOINTMAIN",
        "FOUNDRYENDPOINTMINI",
        "ORCHESTRATORMODEL",
        "MINIMODEL",
        "DOCENDPOINT",
        "DOCMODELID"
    )
    
    foreach ($var in $envVars) {
        [Environment]::SetEnvironmentVariable($var, $null, "User")
    }
    
    Write-Host "✅ Cleared environment variables" -ForegroundColor Green
    Write-Host ""
}

if ($Reset) {
    Clear-DocAIEnvVars
}

Write-Host "📋 Configuration Profile: $Profile" -ForegroundColor Cyan
Write-Host ""

# Prompt for required values
Write-Host "Please provide the following Azure resource endpoints:" -ForegroundColor Yellow
Write-Host "(Press Enter to skip optional values)" -ForegroundColor Gray
Write-Host ""

# Azure AI Foundry / OpenAI Endpoints
$foundryEndpointMain = Read-Host "Azure AI Foundry Endpoint (Main)"
$foundryEndpointMini = Read-Host "Azure AI Foundry Endpoint (Mini/Fallback) [Optional]"
if ([string]::IsNullOrWhiteSpace($foundryEndpointMini)) {
    $foundryEndpointMini = $foundryEndpointMain
}

# Model Names
$mainModel = Read-Host "Main Model Name (e.g., gpt-4o)"
if ([string]::IsNullOrWhiteSpace($mainModel)) {
    $mainModel = "gpt-4o"
}

$miniModel = Read-Host "Mini Model Name (e.g., gpt-4o-mini)"
if ([string]::IsNullOrWhiteSpace($miniModel)) {
    $miniModel = "gpt-4o-mini"
}

# Azure Document Intelligence
$docEndpoint = Read-Host "Azure Document Intelligence Endpoint"
$docModelId = Read-Host "Document Intelligence Model ID (default: prebuilt-layout)"
if ([string]::IsNullOrWhiteSpace($docModelId)) {
    $docModelId = "prebuilt-layout"
}

Write-Host ""
Write-Host "🔧 Setting environment variables..." -ForegroundColor Cyan
Write-Host ""

# Set variables
Set-EnvVar -Name "FOUNDRYENDPOINTMAIN" -Value $foundryEndpointMain `
    -Description "Primary Azure AI endpoint"

Set-EnvVar -Name "FOUNDRYENDPOINTMINI" -Value $foundryEndpointMini `
    -Description "Fallback Azure AI endpoint"

Set-EnvVar -Name "ORCHESTRATORMODEL" -Value $mainModel `
    -Description "Model deployment name for orchestrator"

Set-EnvVar -Name "MINIMODEL" -Value $miniModel `
    -Description "Model deployment name for auxiliary agents"

Set-EnvVar -Name "DOCENDPOINT" -Value $docEndpoint `
    -Description "Azure Document Intelligence endpoint"

Set-EnvVar -Name "DOCMODELID" -Value $docModelId `
    -Description "Document Intelligence model ID"

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "✅ Environment setup complete!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Restart your terminal to load new variables"
Write-Host "   2. Run: dotnet run --project src/DocAISLN/DocAI.Web/"
Write-Host ""
