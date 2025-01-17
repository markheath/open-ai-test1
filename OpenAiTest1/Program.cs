// Program.cs
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.Reflection;

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

var innerClient = new AzureOpenAIClient(
            new Uri(endpoint),
            apiKeyCredential)
                .AsChatClient(modelId);

IChatClient chatClient =
    new ChatClientBuilder(innerClient)
        .UseFunctionInvocation()
        .Build();

List<ChatMessage> chatHistory = new()
{
    new ChatMessage(ChatRole.System, """
        You are a helpful assistant, who can provide descriptions of images.
    """)
};
await MainMenu();

async Task MainMenu()
{
    var examples = LoadExamples();

    while (true)
    {
        DisplayMenu(examples);
        var choice = Console.ReadLine();

        if (int.TryParse(choice, out int index) && index >= 1 && index <= examples.Count)
        {
            var example = examples[index - 1];
            await example.RunAsync(chatClient, chatHistory);
        }
        else
        {
            Console.WriteLine("Invalid choice. Please try again.");
        }
    }
}

List<IExample> LoadExamples()
{
    var examples = new List<IExample>();
    var types = Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => typeof(IExample).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

    foreach (var type in types)
    {
        if (Activator.CreateInstance(type) is IExample example)
        {
            examples.Add(example);
        }
    }

    return examples;
}

void DisplayMenu(List<IExample> examples)
{
    Console.WriteLine("Select an example to run:");
    for (int i = 0; i < examples.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {examples[i].Name}");
    }
    Console.WriteLine("Enter the number of your choice:");
}
