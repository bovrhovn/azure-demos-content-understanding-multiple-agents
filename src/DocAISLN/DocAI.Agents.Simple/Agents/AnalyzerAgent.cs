using System.Text.Json;
using DocAI.Models;
using Spectre.Console;

namespace DocAI.Console.Agents;

public class AnalyzerAgent(IChatClient chatClient)
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
            AnsiConsole.WriteException(err);
            return [];
        }
    }

    private async Task<List<EmailAddress>> ExtractEmails(string text)
    {
        var textSubstring = text.Substring(0, Math.Min(text.Length, 8000));
        var prompt = @"Extract all email addresses from the following text.
For each email, provide context about who it belongs to or its purpose.

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
            AnsiConsole.WriteException(err);
            return [];
        }
    }

    private async Task<List<TableData>> ExtractTables(string text)
    {
        var textSubstring = text.Substring(0, Math.Min(text.Length, 8000));
        var prompt = @"Identify and extract any tables from the following text.
For each table, extract:
- A descriptive title
- Column headers
- All rows of data

Return the result as a JSON array with this structure:
[
  {""title"": ""Table description"", ""headers"": [""Column1"", ""Column2""], ""rows"": [[""value1"", ""value2""], [""value3"", ""value4""]]}
]

TEXT:
" + textSubstring + @"

Return ONLY the JSON array, no additional text. If no tables found, return [].";

        var response = await chatClient.CompleteAsync([new ChatMessage(ChatRole.User, prompt)]);
        var jsonText = ExtractJsonFromResponse(response.Message.Content ?? "[]");
        
        try
        {
            return JsonSerializer.Deserialize<List<TableData>>(jsonText) ?? [];
        }
        catch (Exception err)
        {
            AnsiConsole.WriteException(err);
            return [];
        }
    }

    private string ExtractJsonFromResponse(string response)
    {
        response = response.Trim();
        if (response.StartsWith("```json"))
            response = response.Substring(7);
        else if (response.StartsWith("```")) 
            response = response.Substring(3);
        
        if (response.EndsWith("```")) 
            response = response.Substring(0, response.Length - 3);
        
        return response.Trim();
    }
}
