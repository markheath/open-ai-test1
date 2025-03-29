using Microsoft.Extensions.AI;

namespace OpenAiTest1;
public class ImageExample : IExample
{
    public string Name => "Image Example";

    public async Task RunAsync(IChatClient chatClient, List<ChatMessage> chatHistory)
    {
        var message = new ChatMessage(ChatRole.User, "Can you provide a brief, one-sentence description of this image.");
        // note: ImageContent replaced with DataContent: https://github.com/dotnet/extensions/blob/main/src/Libraries/Microsoft.Extensions.AI.Abstractions/CHANGELOG.md
        using var httpClient = new HttpClient();
        var imageUri = new Uri("https://markheath.net/posts/2022/running-microservices-aca-dapr-2.jpg");
        var imageBytes = await httpClient.GetByteArrayAsync(imageUri);
        var mediaType = "image/jpeg";

        message.Contents.Add(new DataContent(imageBytes, mediaType));
        chatHistory.Add(message);
        await ResponseHelper.GetResponse(chatClient, chatHistory);
    }
}
