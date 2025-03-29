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

        Utils.ColorConsoleWriteLine(ConsoleColor.DarkGray, "How can I help you today?");
        while (true)
        {
            var userPrompt = Utils.GetUserPrompt();
            if (string.IsNullOrEmpty(userPrompt)) break;

            chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

            await ResponseHelper.GetResponse(chatClient, chatHistory, chatOptions);
            Console.WriteLine();
        }
    }

    private List<Order> GetRecentOrders(string userId)
    {
        Utils.ColorConsoleWriteLine(ConsoleColor.Blue, $"\nTOOL FETCHED ORDERS FOR {userId}");
        // Placeholder for fetching orders from a database or service
        if (userId == "12345")
        {
            return
            [
                new Order { OrderNumber = "A123", Items = ["Laptop", "Mouse"] },
                new Order { OrderNumber = "B456", Items = ["Smartphone"] }
            ];
        }
        else if (userId == "23456")
        {
            return
            [
                new Order { OrderNumber = "C234", Items = ["Kettle", "Toaster"] },
                new Order { OrderNumber = "D567", Items = ["Dishwasher"] }
            ];
        }
        else
        {
            return [];
        }

    }

    private string RefundOrder(string orderNumber)
    {
        Utils.ColorConsoleWriteLine(ConsoleColor.Blue, $"\nTOOL REFUNDED ORDER FOR {orderNumber}");
        // Placeholder for refunding an order
        return $"Order {orderNumber} has been successfully refunded.";
    }

    private class Order
    {
        public required string OrderNumber { get; set; }
        public required List<string> Items { get; set; }
    }
}
