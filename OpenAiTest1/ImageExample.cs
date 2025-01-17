using Microsoft.Extensions.AI;
namespace OpenAiTest1;
public class ImageExample : IExample
{
    public string Name => "Image Example";

    public async Task RunAsync(IChatClient chatClient, List<ChatMessage> chatHistory)
    {
        var message = new ChatMessage(ChatRole.User, "Can you provide a brief, one-sentence description of this image.");
        message.Contents.Add(new ImageContent(new Uri("https://markheath.net/posts/2022/running-microservices-aca-dapr-2.jpg")));
        chatHistory.Add(message);
        await ResponseHelper.GetResponse(chatClient, chatHistory);
    }
}
