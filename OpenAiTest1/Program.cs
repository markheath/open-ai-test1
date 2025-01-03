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



List<ChatMessage> chatHistory = new()
    {
        new ChatMessage(ChatRole.System, """
            You are an enthusiastic Arsenal supporter.            
        """)
    };


IChatClient chatClient =
    new AzureOpenAIClient(
        new Uri(endpoint),
        apiKeyCredential) //
            .AsChatClient(modelId);

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