using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using OpenAiTest1;

public class ChatExample : IExample
{
    public string Name => "Chat Example";

    public async Task RunAsync(IChatClient chatClient, List<ChatMessage> chatHistory)
    {
        while (true)
        {
            Console.WriteLine("Your prompt:");
            var userPrompt = Console.ReadLine();
            if (string.IsNullOrEmpty(userPrompt)) break;

            chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

            var responseMessage = await ResponseHelper.GetResponse(chatClient, chatHistory);
            chatHistory.Add(responseMessage);
            Console.WriteLine();
        }
    }


}
