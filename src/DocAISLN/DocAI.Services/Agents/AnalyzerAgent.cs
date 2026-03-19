using System.Text.Json;
using DocAI.Models;
using Microsoft.Extensions.Logging;

namespace DocAI.Services.Agents;

public class AnalyzerAgent(IChatClient chatClient, ILogger<AnalyzerAgent> logger)
{
    public async Task<ExtractedData> ExtractStructuredData(ExtractedData data)
    {
        data.Persons = await ExtractPersons(data.RawText);
        data.Emails = await ExtractEmails(data.RawText);
        data.Tables = await ExtractTables(data.RawText);
        return data;
    }

    private async Task<List<Person>> ExtractPersons(string text)
    {
        var textSubstring = text.Substring(0, Math.Min(text.Length, 8000));
        var prompt = @"Extract all person names from the following text. 
For each person, identify their name, title (if mentioned), and organization (if mentioned).

Return the result as a JSON array with this structure:
[
  {""name"": ""Full Name"", ""title"": ""Job Title or null"", ""organization"": ""Company/Organization or null"", ""confidence"": 80}
]

TEXT:
" + textSubstring + @"

Return ONLY the JSON array, no additional text.";

        var response = await chatClient.CompleteAsync([new ChatMessage(ChatRole.User, prompt)]);
        var jsonText = ExtractJsonFromResponse(response.Message.Content ?? "[]");
        
        try
        {
            return JsonSerializer.Deserialize<List<Person>>(jsonText) ?? [];
        }
        catch(Exception err)
        {
            logger.LogError(err, "Failed to parse persons JSON");
            return [];
        }
    }

    private async Task<List<EmailAddress>> ExtractEmails(string text)
    {
        var textSubstring = text.Substring(0, Math.Min(text.Length, 8000));
        var prompt = @"Extract all email addresses from the following text.
For each email, provide context about who or what it belongs to.

Return the result as a JSON array with this structure:
[
  {""email"": ""email@example.com"", ""context"": ""Belongs to John Doe, Sales Manager""}
]

TEXT:
" + textSubstring + @"

Return ONLY the JSON array, no additional text.";

        var response = await chatClient.CompleteAsync([new ChatMessage(ChatRole.User, prompt)]);
        var jsonText = ExtractJsonFromResponse(response.Message.Content ?? "[]");
        
        try
        {
            return JsonSerializer.Deserialize<List<EmailAddress>>(jsonText) ?? [];
        }
        catch(Exception err)
        {
            logger.LogError(err, "Failed to parse emails JSON");
            return [];
        }
    }

    private async Task<List<TableData>> ExtractTables(string text)
    {
        var textSubstring = text.Substring(0, Math.Min(text.Length, 8000));
        var prompt = @"Extract all tables from the following text.
For each table, identify the title (if any), headers, and rows.

Return the result as a JSON array with this structure:
[
  {
    ""title"": ""Table Title or empty string"",
    ""headers"": [""Column1"", ""Column2"", ""Column3""],
    ""rows"": [
      [""Value1"", ""Value2"", ""Value3""],
      [""Value4"", ""Value5"", ""Value6""]
    ]
  }
]

TEXT:
" + textSubstring + @"

If no tables are found, return an empty array [].
Return ONLY the JSON array, no additional text.";

        var response = await chatClient.CompleteAsync([new ChatMessage(ChatRole.User, prompt)]);
        var jsonText = ExtractJsonFromResponse(response.Message.Content ?? "[]");
        
        try
        {
            return JsonSerializer.Deserialize<List<TableData>>(jsonText) ?? [];
        }
        catch(Exception err)
        {
            logger.LogError(err, "Failed to parse tables JSON");
            return [];
        }
    }

    private static string ExtractJsonFromResponse(string response)
    {
        var trimmed = response.Trim();
        
        if (trimmed.StartsWith("```json"))
        {
            trimmed = trimmed.Substring(7);
        }
        else if (trimmed.StartsWith("```"))
        {
            trimmed = trimmed.Substring(3);
        }
        
        if (trimmed.EndsWith("```"))
        {
            trimmed = trimmed.Substring(0, trimmed.Length - 3);
        }
        
        return trimmed.Trim();
    }
}
