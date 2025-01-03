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

IChatClient client =
    new AzureOpenAIClient(
        new Uri(endpoint),
        apiKeyCredential) //
            .AsChatClient(modelId);

var response = await client.CompleteAsync("What is a key credential?");

Console.WriteLine(response.Message);