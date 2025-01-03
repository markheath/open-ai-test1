using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;

var endpoint = "https://mark-openai.openai.azure.com/"; // Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var modelId = "gpt-4o-mini-mark1"; // this is the deployment name
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions() {  TenantId = "f476856c-e3be-4738-aea7-e431d6bfc6e8" });

IChatClient client =
    new AzureOpenAIClient(
        new Uri(endpoint),
        credential)
            .AsChatClient(modelId);

var response = await client.CompleteAsync("What is AI?");

Console.WriteLine(response.Message);