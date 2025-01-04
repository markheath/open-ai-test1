using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var configuration = builder.Build();

var endpoint = "https://mark-openai.openai.azure.com/"; // Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var modelId = "gpt-4o-mini-mark1"; // this is the deployment name
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() {  TenantId = "f476856c-e3be-4738-aea7-e431d6bfc6e8" });

var apiKey = configuration["AzureOpenAI:ApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new InvalidOperationException("API key is not set in the secrets.");
}

var apiKeyCredential = new ApiKeyCredential(apiKey);

IChatClient chatClient =
    new AzureOpenAIClient(
        new Uri(endpoint),
        apiKeyCredential) //            
            .AsChatClient(modelId);

List<ChatMessage> chatHistory = new()
    {
        new ChatMessage(ChatRole.System, """
            You are a helpful assistant, who can provide descriptions of images.
        """)
    };





while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var response = "";
    await foreach (var item in
        chatClient.CompleteStreamingAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}

async static Task ImageExample(IChatClient chatClient, List<ChatMessage> chatHistory)
{
    var message = new ChatMessage(ChatRole.User, "Can you provide a brief, one-sentence description of this image.");
    //var data = File.ReadAllBytes(@"C:\Users\markh\Downloads\KCC Youth Band Harun Chris Neil Will.jpeg");
    //message.Contents.Add(new ImageContent(data, "image/jpeg"));

    message.Contents.Add(new ImageContent(new Uri("https://markheath.net/posts/2022/running-microservices-aca-dapr-2.jpg")));
    chatHistory.Add(message);
    //var resp = await chatClient.CompleteAsync(chatHistory);
    //Console.WriteLine(resp.Message.Text);

    Console.WriteLine($"AI Response started at {DateTime.Now}:");
    await foreach (var item in
        chatClient.CompleteStreamingAsync(chatHistory))
    {
        Console.Write(item.Text);
    }
    Console.WriteLine($"\nAI Response completed at {DateTime.Now}:");
}