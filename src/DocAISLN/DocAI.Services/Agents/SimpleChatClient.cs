using Azure.AI.Inference;

namespace DocAI.Services.Agents;

public interface IChatClient
{
    Task<ChatResponse> CompleteAsync(ChatMessage[] messages);
}

public class SimpleChatClient(ChatCompletionsClient client, string modelId) : IChatClient
{
    public async Task<ChatResponse> CompleteAsync(ChatMessage[] messages)
    {
        var requestMessages = messages.Select(m => 
            new ChatRequestUserMessage(m.Content)).Cast<ChatRequestMessage>().ToList();
        
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Model = modelId,
            Messages = requestMessages
        };

        var response = await client.CompleteAsync(chatCompletionsOptions);
        var content = response.Value.Content;
        
        return new ChatResponse
        {
            Message = new ChatMessage(ChatRole.Assistant, content)
        };
    }
}

public class ChatMessage(ChatRole role, string content)
{
    public ChatRole Role { get; set; } = role;
    public string Content { get; set; } = content;
}

public class ChatResponse
{
    public ChatMessage Message { get; set; } = new ChatMessage(ChatRole.Assistant, "");
}

public enum ChatRole
{
    User,
    Assistant
}
