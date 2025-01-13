using Microsoft.Extensions.AI;

namespace OpenAiTest1;

internal class ResponseHelper
{
    public static async Task<ChatMessage> GetResponse(IChatClient chatClient, List<ChatMessage> chatHistory)
    {
        Console.WriteLine($"AI Response started at {DateTime.Now}:");
        var response = "";
        UsageDetails? usageDetails = null;

        await foreach (var item in chatClient.CompleteStreamingAsync(chatHistory))
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

    private static void ShowUsageDetails(UsageDetails? usage)
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

    private static void ShowNestedDictionary(IDictionary<string, object> dictionary, string indent)
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
}
