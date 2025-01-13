// IExample.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;

public interface IExample
{
    string Name { get; }
    Task RunAsync(IChatClient chatClient, List<ChatMessage> chatHistory);
}
