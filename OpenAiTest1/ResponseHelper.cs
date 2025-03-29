using Microsoft.Extensions.AI;

namespace OpenAiTest1;

internal class ResponseHelper
{
    public static async Task<ChatMessage> GetResponse(IChatClient chatClient, List<ChatMessage> chatHistory, ChatOptions? options = null)
    {
        Console.WriteLine($"AI Response started at {DateTime.Now}:");
        var response = "";
        UsageDetails? usageDetails = null;

        await foreach (var item in chatClient.GetStreamingResponseAsync(chatHistory, options))
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
            
            if (usage.AdditionalCounts != null)
            {
                foreach(var additionalCount in usage.AdditionalCounts)
                {
                    Console.WriteLine($"  {additionalCount.Key}: {additionalCount.Value}");
                }

            }
        }
    }

}
