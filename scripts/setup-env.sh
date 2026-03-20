#!/bin/bash

##############################################################################
# Azure DocAI - Environment Variable Setup (Bash)
# 
# This script configures environment variables for the Azure Content
# Understanding Multiple Agents demo on Linux/macOS.
#
# Usage:
#   ./setup-env.sh
#   ./setup-env.sh --reset
##############################################################################

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Banner
echo -e "${CYAN}================================================${NC}"
echo -e "${CYAN}  Azure DocAI - Environment Variable Setup${NC}"
echo -e "${CYAN}================================================${NC}"
echo ""

# Function to set environment variable
set_env_var() {
    local name=$1
    local value=$2
    local description=$3
    
    if [ -z "$value" ]; then
        echo -e "${YELLOW}⚠️  Skipping $name (no value provided)${NC}"
        return
    fi
    
    # Add to ~/.bashrc or ~/.zshrc depending on shell
    if [ -n "$ZSH_VERSION" ]; then
        SHELL_RC="$HOME/.zshrc"
    else
        SHELL_RC="$HOME/.bashrc"
    fi
    
    # Remove existing entry if present
    sed -i.bak "/export $name=/d" "$SHELL_RC" 2>/dev/null || true
    
    # Add new entry
    echo "export $name=\"$value\"" >> "$SHELL_RC"
    
    # Also set for current session
    export "$name=$value"
    
    echo -e "${GREEN}✅ Set $name${NC}"
    if [ -n "$description" ]; then
        echo -e "${GRAY}   $description${NC}"
    fi
}

# Function to clear DocAI environment variables
clear_docai_env_vars() {
    echo -e "${YELLOW}🧹 Clearing existing DocAI environment variables...${NC}"
    
    local env_vars=(
        "FOUNDRYENDPOINTMAIN"
        "FOUNDRYENDPOINTMINI"
        "ORCHESTRATORMODEL"
        "MINIMODEL"
        "DOCENDPOINT"
        "DOCMODELID"
    )
    
    if [ -n "$ZSH_VERSION" ]; then
        SHELL_RC="$HOME/.zshrc"
    else
        SHELL_RC="$HOME/.bashrc"
    fi
    
    for var in "${env_vars[@]}"; do
        sed -i.bak "/export $var=/d" "$SHELL_RC" 2>/dev/null || true
        unset "$var"
    done
    
    echo -e "${GREEN}✅ Cleared environment variables${NC}"
    echo ""
}

# Check for reset flag
if [ "$1" = "--reset" ] || [ "$1" = "-r" ]; then
    clear_docai_env_vars
fi

# Prompt for values
echo -e "${YELLOW}Please provide the following Azure resource endpoints:${NC}"
echo -e "${GRAY}(Press Enter to skip optional values)${NC}"
echo ""

# Azure AI Foundry / OpenAI Endpoints
read -p "Azure AI Foundry Endpoint (Main): " FOUNDRY_ENDPOINT_MAIN
read -p "Azure AI Foundry Endpoint (Mini/Fallback) [Optional]: " FOUNDRY_ENDPOINT_MINI

if [ -z "$FOUNDRY_ENDPOINT_MINI" ]; then
    FOUNDRY_ENDPOINT_MINI="$FOUNDRY_ENDPOINT_MAIN"
fi

# Model Names
read -p "Main Model Name (default: gpt-4o): " MAIN_MODEL
if [ -z "$MAIN_MODEL" ]; then
    MAIN_MODEL="gpt-4o"
fi

read -p "Mini Model Name (default: gpt-4o-mini): " MINI_MODEL
if [ -z "$MINI_MODEL" ]; then
    MINI_MODEL="gpt-4o-mini"
fi

# Azure Document Intelligence
read -p "Azure Document Intelligence Endpoint: " DOC_ENDPOINT
read -p "Document Intelligence Model ID (default: prebuilt-layout): " DOC_MODEL_ID
if [ -z "$DOC_MODEL_ID" ]; then
    DOC_MODEL_ID="prebuilt-layout"
fi

echo ""
echo -e "${CYAN}🔧 Setting environment variables...${NC}"
echo ""

# Set variables
set_env_var "FOUNDRYENDPOINTMAIN" "$FOUNDRY_ENDPOINT_MAIN" "Primary Azure AI endpoint"
set_env_var "FOUNDRYENDPOINTMINI" "$FOUNDRY_ENDPOINT_MINI" "Fallback Azure AI endpoint"
set_env_var "ORCHESTRATORMODEL" "$MAIN_MODEL" "Model deployment name for orchestrator"
set_env_var "MINIMODEL" "$MINI_MODEL" "Model deployment name for auxiliary agents"
set_env_var "DOCENDPOINT" "$DOC_ENDPOINT" "Azure Document Intelligence endpoint"
set_env_var "DOCMODELID" "$DOC_MODEL_ID" "Document Intelligence model ID"

echo ""
echo -e "${CYAN}================================================${NC}"
echo -e "${GREEN}✅ Environment setup complete!${NC}"
echo -e "${CYAN}================================================${NC}"
echo ""
echo -e "${YELLOW}📝 Next Steps:${NC}"
echo "   1. Reload your shell: source ~/.bashrc (or ~/.zshrc)"
echo "   2. Verify: echo \$FOUNDRYENDPOINTMAIN"
echo "   3. Run: dotnet run --project src/DocAISLN/DocAI.Web/"
echo ""
echo -e "${YELLOW}📖 Documentation:${NC}"
echo "   • Environment Setup: docs/environment-setup.md"
echo "   • MCP Integration: docs/model-context-protocol.md"
echo "   • Main README: README.md"
echo ""
