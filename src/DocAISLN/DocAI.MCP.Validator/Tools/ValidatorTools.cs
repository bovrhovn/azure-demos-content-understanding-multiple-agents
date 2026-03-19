using System.ComponentModel;
using ModelContextProtocol.Server;

namespace DocAI.MCP.Validator.Tools;

[McpServerToolType]
public class ValidatorTools
{
    [McpServerTool, Description("Echo a message back.")]
    public string Echo([Description("Message to echo")] string message)
        => $"Hello from MCP: {message}";
}