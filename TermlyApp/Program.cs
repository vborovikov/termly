using Termly;

Console.Out.WriteLineInColor($"{"Hello":blue}, {"World":white|green}!");
Console.Error.WriteLine(ConsoleColor.Red, $"{"Hello":x}, {"World":x}!");
Console.Out.WriteLineInColor($"{12345.67:N2:cyan}");
Console.Out.WriteLineInColor($"{DateTimeOffset.Now:HH:mm:yellow}");