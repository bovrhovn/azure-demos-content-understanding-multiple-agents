using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using DocAI.Models;
using ModelContextProtocol.Server;
using ValidationResult = DocAI.Models.ValidationResult;

namespace DocAI.MCP.Validator.Tools;

[McpServerToolType]
public class ValidatorTools
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);
    
    [McpServerTool(Name = "Echo"), Description("Echo a message back.")]
    public string Echo([Description("Message to echo")] string message)
        => $"Hello from MCP: {message}";

    [McpServerTool(Name = "Validate"), Description("Validate provided extracted data")]
    public async Task<ValidationResult> ValidateExtractedData(ExtractedData data)
    {
        var result = new Models.ValidationResult { IsValid = true };

        // Validate emails
        foreach (var email in data.Emails)
        {
            var item = new ValidationItem
            {
                Type = "Email",
                Value = email.Email
            };

            if (EmailRegex.IsMatch(email.Email))
            {
                item.Reason = "Valid email format";
                result.ValidItems.Add(item);
            }
            else
            {
                item.Reason = "Invalid email format";
                result.InvalidItems.Add(item);
                result.IsValid = false;
            }
        }

        // Validate persons using AI
        foreach (var person in data.Persons)
        {
            var isValid = await ValidatePersonName(person.Name);
            var item = new ValidationItem
            {
                Type = "Person",
                Value = person.Name
            };

            if (isValid)
            {
                item.Reason = "Valid person name";
                result.ValidItems.Add(item);
            }
            else if (person.Confidence < 70)
            {
                item.Reason = $"Low confidence ({person.Confidence}%)";
                result.UnconfirmedItems.Add(item);
            }
            else
            {
                item.Reason = "Could not confirm as valid person name";
                result.UnconfirmedItems.Add(item);
            }
        }

        // Validate tables
        foreach (var table in data.Tables)
        {
            var item = new ValidationItem
            {
                Type = "Table",
                Value = $"{table.Title} ({table.RowCount} rows, {table.ColumnCount} columns)"
            };

            if (table.Headers.Count > 0 && table.Rows.Count > 0)
            {
                bool allRowsValid = table.Rows.All(row => row.Count == table.Headers.Count);
                if (allRowsValid)
                {
                    item.Reason = "Valid table structure";
                    result.ValidItems.Add(item);
                }
                else
                {
                    item.Reason = "Inconsistent column count across rows";
                    result.InvalidItems.Add(item);
                    result.IsValid = false;
                }
            }
            else
            {
                item.Reason = "Empty or incomplete table";
                result.UnconfirmedItems.Add(item);
            }
        }

        return result;
    }   
    
    private Task<bool> ValidatePersonName(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
                return Task.FromResult(false);

            // Basic heuristics
            return name.Any(char.IsDigit) ? 
                Task.FromResult(false) : Task.FromResult(name.Split(' ').Length >= 2);
        }
        catch (Exception exception)
        {
            return Task.FromException<bool>(exception);
        }
    }
}