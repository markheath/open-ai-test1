namespace OpenAiTest1;

internal class Utils
{
    static public void ColorConsoleWriteLine(ConsoleColor color, string message)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        //Console.ResetColor();
    }
    static public void ColorConsoleWrite(ConsoleColor color, string message)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        //Console.ResetColor();
    }
    static public string? GetUserPrompt()
    {
        ColorConsoleWriteLine(ConsoleColor.DarkGray, "Your prompt:");
        Console.ForegroundColor = ConsoleColor.Yellow;
        var userPrompt = Console.ReadLine();
        return userPrompt;
    }

}
