using Termly;

Console.Out.PrintLine($"{"Hello":blue}, {"World":white|green}!");
Console.Error.PrintLine(ConsoleColor.Red, $"{"Hello":x}, {"World":x}!");
Console.Out.PrintLine($"{12345.67:N2:cyan}");
Console.Out.PrintLine($"{DateTimeOffset.Now:HH:mm:yellow}");