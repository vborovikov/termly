# Termly
Terminal colorizer for .NET console apps

[![NuGet](https://img.shields.io/nuget/dt/Termly.svg)](https://www.nuget.org/packages/Termly)
[![NuGet](https://img.shields.io/nuget/v/Termly.svg)](https://www.nuget.org/packages/Termly)

Termly provides an easy to use set of extension methods to display colorized text in the terminal. It doesn't support arbitrary colors, instead it reuses the same old [ConsoleColor](https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor) constants. The idea here is that you don't need to come up with your own unique color pallete to display textual data, you just continue using the colors the user already chose for their terminal. The colorization is achieved with the use of a small subset of ANSI escape codes.

Apps with Termly look great in any terminal, using the user's favorite color scheme.

# Usage

You can start adding color to the console output with `Print`, `PrintLine`, and `InColor` extension methods.

```csharp
private static void HandleContextLog(object sender, LoggingEventArgs e)
{
    if (e.Kind > ChannelMessageKind.Trace)
    {
        // using PrintLine method
        Console.Error.PrintLine($"{e.Source:gray}: {e.RawMessage:darkBlue}");

        // using InColor method
        Console.Error.WriteLine($"{e.Source.InColor(ConsoleColor.Gray)}: {e.RawMessage.InColor(ConsoleColor.DarkBlue)}");
    }
}
```

The overloaded extension methods `Print` and `PrintLine` are used to colorize interpolated string parameters with the same or different colors.

```csharp
Console.Out.PrintLine(ConsoleColor.DarkYellow, $"Parameters Count: {parameters.Statistics.ParametersCount}");

Console.Out.PrintLine($"Count: {count:blue} Total: {total:white|green}");
```

Background colors are supported too.

```csharp
Console.Error.WriteLine("ALERT".InColor(foreground: ConsoleColor.Black, background: ConsoleColor.Red));
Console.Error.PrintLine($"{"ALERT":black|red}");
```
