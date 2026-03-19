namespace DocAI.Console.Agents;

public class PdfReaderAgent(IChatClient chatClient)
{
    public async Task<string> AnalyzeTextStructure(string rawText)
    {
        var prompt = $"""
            Analyze the following text extracted from a PDF document.
            Provide a brief summary of its structure and content:
            - Document type (invoice, report, letter, etc.)
            - Main sections identified
            - Quality of text extraction
            - Any issues or anomalies
            
            TEXT:
            {rawText.Substring(0, Math.Min(rawText.Length, 4000))}
            
            Respond in a concise format.
            """;

        var response = await chatClient.CompleteAsync([new ChatMessage(ChatRole.User, prompt)]);
        return response.Message.Content ?? "Unable to analyze structure";
    }
}
