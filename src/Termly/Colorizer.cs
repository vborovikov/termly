namespace Termly;

using System;
using System.IO;
using System.Runtime.InteropServices;

public static partial class Colorizer
{
    private const string ResetCode = "\x1b[0m";

    private static readonly string[] ForegroundCodes =
    {
            "\x1b[30m",     /// <see cref="ConsoleColor.Black"/>
            "\x1b[34m",     /// <see cref="ConsoleColor.DarkBlue"/>
            "\x1b[32m",     /// <see cref="ConsoleColor.DarkGreen"/>
            "\x1b[36m",     /// <see cref="ConsoleColor.DarkCyan"/>
            "\x1b[31m",     /// <see cref="ConsoleColor.DarkRed"/>
            "\x1b[35m",     /// <see cref="ConsoleColor.DarkMagenta"/>
            "\x1b[33m",     /// <see cref="ConsoleColor.DarkYellow"/>
            "\x1b[37m",     /// <see cref="ConsoleColor.Gray"/>
            "\x1b[30;1m",   /// <see cref="ConsoleColor.DarkGray"/>
            "\x1b[34;1m",   /// <see cref="ConsoleColor.Blue"/>
            "\x1b[32;1m",   /// <see cref="ConsoleColor.Green"/>
            "\x1b[36;1m",   /// <see cref="ConsoleColor.Cyan"/>
            "\x1b[31;1m",   /// <see cref="ConsoleColor.Red"/>
            "\x1b[35;1m",   /// <see cref="ConsoleColor.Magenta"/>
            "\x1b[33;1m",   /// <see cref="ConsoleColor.Yellow"/>
            "\x1b[37;1m",   /// <see cref="ConsoleColor.White"/>
    };

    private static readonly string[] BackgroundCodes =
    {
            "\x1b[40m",     /// <see cref="ConsoleColor.Black"/>
            "\x1b[44m",     /// <see cref="ConsoleColor.DarkBlue"/>
            "\x1b[42m",     /// <see cref="ConsoleColor.DarkGreen"/>
            "\x1b[46m",     /// <see cref="ConsoleColor.DarkCyan"/>
            "\x1b[41m",     /// <see cref="ConsoleColor.DarkRed"/>
            "\x1b[45m",     /// <see cref="ConsoleColor.DarkMagenta"/>
            "\x1b[43m",     /// <see cref="ConsoleColor.DarkYellow"/>
            "\x1b[47m",     /// <see cref="ConsoleColor.Gray"/>
            "\x1b[40;1m",   /// <see cref="ConsoleColor.DarkGray"/>
            "\x1b[44;1m",   /// <see cref="ConsoleColor.Blue"/>
            "\x1b[42;1m",   /// <see cref="ConsoleColor.Green"/>
            "\x1b[46;1m",   /// <see cref="ConsoleColor.Cyan"/>
            "\x1b[41;1m",   /// <see cref="ConsoleColor.Red"/>
            "\x1b[45;1m",   /// <see cref="ConsoleColor.Magenta"/>
            "\x1b[43;1m",   /// <see cref="ConsoleColor.Yellow"/>
            "\x1b[47;1m",   /// <see cref="ConsoleColor.White"/>
    };

    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_ERROR_HANDLE = -12;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    private static readonly bool isEnabled;
    private static readonly bool isEnabledOut;
    private static readonly bool isEnabledErr;


    static Colorizer()
    {
        isEnabled = Environment.GetEnvironmentVariable("NO_COLOR") is null;

        if (isEnabled && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            isEnabledOut = TryEnableForDevice(STD_OUTPUT_HANDLE);
            isEnabledErr = TryEnableForDevice(STD_ERROR_HANDLE);
            isEnabled = isEnabledOut || isEnabledErr;
        }
    }

    public static string? InColor<T>(this T obj, ConsoleColor foreground, ConsoleColor? background = default)
    {
        var text = obj?.ToString();
        if (!isEnabled || text is null || text.Length == 0)
            return text;

        return SpanInColor(text, foreground, background);
    }

    public static string? InColor<T>(this T obj, string? format, ConsoleColor foreground, ConsoleColor? background = default)
        where T : struct, IFormattable => InColor(obj, format, null, foreground, background);

    public static string? InColor<T>(this T obj, string? format, IFormatProvider? formatProvider, ConsoleColor foreground, ConsoleColor? background = default)
        where T : struct, IFormattable
    {
        var text = obj.ToString(format, formatProvider);
        if (!isEnabled || text is null || text.Length == 0)
            return text;

        return SpanInColor(text, foreground, background);
    }

    public static void Write(this TextWriter writer, ConsoleColor foreground, string value)
    {
        writer.Write(value.InColor(foreground));
    }

    public static void Write(this TextWriter writer, ConsoleColor foreground, ConsoleColor background, string value)
    {
        writer.Write(value.InColor(foreground, background));
    }

    public static void WriteLine(this TextWriter writer, ConsoleColor foreground, string value)
    {
        writer.WriteLine(value.InColor(foreground));
    }

    public static void WriteLine(this TextWriter writer, ConsoleColor foreground, ConsoleColor background, string value)
    {
        writer.WriteLine(value.InColor(foreground, background));
    }

    private static bool IsEnabledFor(TextWriter? writer = default)
    {
        return writer switch
        {
            var x when x == Console.Out => isEnabledOut,
            var x when x == Console.Error => isEnabledErr,
            _ => false
        };
    }

    private static string SpanInColor(ReadOnlySpan<char> text, ConsoleColor foreground, ConsoleColor? background)
    {
        if (background is not null)
            return String.Concat(ForegroundCodes[(int)foreground], BackgroundCodes[(int)background.Value], text, ResetCode);

        return String.Concat(ForegroundCodes[(int)foreground], text, ResetCode);
    }

    private static bool TryEnableForDevice(int nStdHandle)
    {
        var deviceHandle = GetStdHandle(nStdHandle);

        return
            GetConsoleMode(deviceHandle, out var deviceMode) &&
            SetConsoleMode(deviceHandle, deviceMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
    }
}
