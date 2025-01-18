using Microsoft.Extensions.AI;

namespace OpenAiTest1;

public class ToolExample : IExample
{
    public string Name => "Order Management Example";

    public async Task RunAsync(IChatClient chatClient, List<ChatMessage> chatHistory)
    {
        var chatOptions = new ChatOptions
        {
            Tools = new List<AITool>
            {
                AIFunctionFactory.Create(
                    (string userId) => GetRecentOrders(userId),
                    "get_recent_orders",
                    "Retrieve the recent orders of a user by their user ID."
                ),
                AIFunctionFactory.Create(
                    (string orderNumber) => RefundOrder(orderNumber),
                    "refund_order",
                    "Refund a specific order by its order number."
                )
            }
        };

        // Update system prompt to provide context about available tools
        chatHistory[0] = new ChatMessage(ChatRole.System, """
            You are a customer support assistant capable of handling user orders. You can retrieve a user's recent orders and process refunds when requested. Use the provided tools to assist the user effectively. 
            
            You are chatting with user '12345'. How can I help you today?
            """);

        Console.WriteLine("How can I help you today?");
        while (true)
        {
            Console.Write("Your prompt:");
            var userPrompt = Console.ReadLine();
            if (string.IsNullOrEmpty(userPrompt)) break;

            chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

            await ResponseHelper.GetResponse(chatClient, chatHistory, chatOptions);
            Console.WriteLine();
        }
    }

    private List<Order> GetRecentOrders(string userId)
    {
        Console.WriteLine($"\nTOOL FETCHED ORDERS FOR {userId}");
        // Placeholder for fetching orders from a database or service
        if (userId == "12345")
        {
            return new List<Order>
            {
                new Order { OrderNumber = "A123", Items = new List<string> { "Laptop", "Mouse" } },
                new Order { OrderNumber = "B456", Items = new List<string> { "Smartphone" } }
            };
        }
        else if (userId == "23456")
        {
            return new List<Order>
            {
                new Order { OrderNumber = "C234", Items = new List<string> { "Kettle", "Toaster" } },
                new Order { OrderNumber = "D567", Items = new List<string> { "Dishwasher" } }
            };
        }
        else
        {
            return new List<Order>();
        }

    }

    private string RefundOrder(string orderNumber)
    {
        Console.WriteLine($"\nTOOL REFUNDED ORDER FOR {orderNumber}");
        // Placeholder for refunding an order
        return $"Order {orderNumber} has been successfully refunded.";
    }

    private class Order
    {
        public string OrderNumber { get; set; }
        public List<string> Items { get; set; }
    }
}
