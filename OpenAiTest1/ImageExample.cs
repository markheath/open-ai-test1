using Microsoft.Extensions.AI;

namespace OpenAiTest1;
public class ImageExample : IExample
{
    public string Name => "Image Example";

    public async Task RunAsync(IChatClient chatClient, List<ChatMessage> chatHistory)
    {
        var message = new ChatMessage(ChatRole.User, "Can you provide a brief, one-sentence description of this image.");
        // note: ImageContent replaced with DataContent: https://github.com/dotnet/extensions/blob/main/src/Libraries/Microsoft.Extensions.AI.Abstractions/CHANGELOG.md
        
        /* use DataContent for if you have the image bytes (e.g. a local file)
        using var httpClient = new HttpClient();
        var imageUri = new Uri("https://markheath.net/posts/2022/running-microservices-aca-dapr-2.jpg");
        var imageBytes = await httpClient.GetByteArrayAsync(imageUri);
        var mediaType = "image/jpeg";
        var content = new DataContent(imageBytes, mediaType);*/

        // you can use image content for online
        var imageContent = new UriContent("https://markheath.net/posts/2022/running-microservices-aca-dapr-2.jpg", "image/jpeg");

        message.Contents.Add(imageContent);
        chatHistory.Add(message);
        await ResponseHelper.GetResponse(chatClient, chatHistory);
    }
}
