# Termly
Terminal colorizer for .NET console apps

Termly provides an easy to use set of extension methods to display colorized text in the terminal. It doesn't support arbitrary colors, instead it reuses the same old [ConsoleColor](https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor) constants. The idea here is that you don't need to come up with your own unique color pallete to display textual data, you just continue using the colors the user already chose for their terminal. The colorization is achieved with the use of a small subset of ANSI escape codes.

Apps with Termly look great in Windows Terminal with your favorite color scheme.

# Usage

You can start adding color to the console output with `InColor` method extension.

```csharp
private static void HandleContextLog(object sender, LoggingEventArgs e)
{
    if (e.Kind > ChannelMessageKind.Trace)
    {
        Console.Error.WriteLine($"{e.Source.InColor(ConsoleColor.Gray)}: {e.RawMessage.InColor(ConsoleColor.DarkBlue)}");
    }
}
```

The extension methods `Write` and `WriteLine` are used to colorize interpolated strings.

```csharp
console.WriteLine(ConsoleColor.DarkYellow, $"Parameters Count: {parameters.Statistics.ParametersCount}");
```

Background colors are supported too.

```csharp
Console.Error.WriteLine("ALERT".InColor(foreground: ConsoleColor.Black, background: ConsoleColor.Red));
```
