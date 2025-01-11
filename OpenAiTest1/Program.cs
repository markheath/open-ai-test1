using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var configuration = builder.Build();

var endpoint = "https://mark-openai.openai.azure.com/"; // Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var modelId = "gpt-4o-mini-mark1"; // this is the deployment name
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() { TenantId = "f476856c-e3be-4738-aea7-e431d6bfc6e8" });

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
await MainMenu();

async Task ChatExample(IChatClient chatClient, List<ChatMessage> chatHistory)
{
    while (true)
    {
        // Get user prompt and add to chat history
        Console.WriteLine("Your prompt:");
        var userPrompt = Console.ReadLine();
        chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

        var responseMessage = await GetResponse(chatClient, chatHistory);
        chatHistory.Add(responseMessage);
        Console.WriteLine();
    }
}

async Task<ChatMessage> GetResponse(IChatClient chatClient, List<ChatMessage> chatHistory)
{
    // Stream the AI response and add to chat history
    Console.WriteLine($"AI Response started at {DateTime.Now}:");
    var response = "";
    //var x = await chatClient.CompleteAsync(chatHistory);
    //a ChatCompletion has Usage
    UsageDetails? usageDetails = null;
    await foreach (var item in
        chatClient.CompleteStreamingAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;

        var usage = item.Contents.OfType<UsageContent>().FirstOrDefault()?.Details;
        if (usage != null) usageDetails = usage;
    }
    Console.WriteLine($"\nAI Response completed at {DateTime.Now}:");

    ShowUsageDetails(usageDetails);
    return new ChatMessage(ChatRole.Assistant, response);
}

void ShowUsageDetails(UsageDetails? usage)
{
    if (usage != null)
    {
        Console.WriteLine($"  InputTokenCount: {usage.InputTokenCount}");
        Console.WriteLine($"  OutputTokenCount: {usage.OutputTokenCount}");
        Console.WriteLine($"  TotalTokenCount: {usage.TotalTokenCount}");
        if (usage.AdditionalProperties != null)
        {
            ShowNestedDictionary(usage.AdditionalProperties!, "    ");
        }
    }
}

void ShowNestedDictionary(IDictionary<string, object> dictionary, string indent)
{
    foreach (var (key, value) in dictionary)
    {
        if (value is IDictionary<string, object> nestedDictionary)
        {
            Console.WriteLine($"{indent}{key}:");
            ShowNestedDictionary(nestedDictionary, indent + "    ");
        }
        else
        {
            Console.WriteLine($"{indent}{key}: {value}");
        }
    }
}

async Task ImageExample(IChatClient chatClient, List<ChatMessage> chatHistory)
{
    var message = new ChatMessage(ChatRole.User, "Can you provide a brief, one-sentence description of this image.");
    message.Contents.Add(new ImageContent(new Uri("https://markheath.net/posts/2022/running-microservices-aca-dapr-2.jpg")));
    chatHistory.Add(message);

    Console.WriteLine($"AI Response started at {DateTime.Now}:");
    await foreach (var item in
        chatClient.CompleteStreamingAsync(chatHistory))
    {
        Console.Write(item.Text);
    }
    Console.WriteLine($"\nAI Response completed at {DateTime.Now}:");
}

void DisplayMenu()
{
    Console.WriteLine("Select an example to run:");
    Console.WriteLine("1. Chat Example");
    Console.WriteLine("2. Image Example");
    Console.WriteLine("Enter the number of your choice:");
}

async Task MainMenu()
{
    while (true)
    {
        DisplayMenu();
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ChatExample(chatClient, chatHistory);
                break;
            case "2":
                await ImageExample(chatClient, chatHistory);
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}

