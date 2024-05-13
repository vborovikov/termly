namespace Termly;

using System;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// Provides utilities for colorizing console output.
/// </summary>
public static partial class Colorizer
{
    /// <summary>
    /// The ANSI code used to reset terminal color.
    /// </summary>
    private const string ResetCode = "\x1b[0m";

    /// <summary>
    /// The ANSI codes used to set foreground color for each <see cref="ConsoleColor"/>.
    /// </summary>
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

    /// <summary>
    /// The ANSI codes used to set background color for each <see cref="ConsoleColor"/>.
    /// </summary>
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

    // The handle for standard output
    private const int STD_OUTPUT_HANDLE = -11;
    // The handle for standard error
    private const int STD_ERROR_HANDLE = -12;
    // The flag used to enable virtual terminal processing on Windows
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    // A native method from kernel32.dll used to get the console mode
    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
    // A native method from kernel32.dll used to set the console mode
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    // A native method from kernel32.dll used to get the handle for a standard device
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    /// <summary>
    /// <c>true</c> if colorization is enabled, <c>false</c> otherwise.
    /// </summary>
    private static readonly bool isEnabled;

    /// <summary>
    /// <c>true</c> if colorization is enabled for standard output, <c>false</c> otherwise.
    /// </summary>
    private static readonly bool isEnabledOut;

    /// <summary>
    /// <c>true</c> if colorization is enabled for standard error, <c>false</c> otherwise.
    /// </summary>
    private static readonly bool isEnabledErr;


    /// <summary>
    /// Initializes static members of the <see cref="Colorizer"/> class.
    /// </summary>
    static Colorizer()
    {
        isEnabled = Environment.GetEnvironmentVariable("NO_COLOR") is null;

        if (isEnabled)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                isEnabledOut = TryEnableForDevice(STD_OUTPUT_HANDLE);
                isEnabledErr = TryEnableForDevice(STD_ERROR_HANDLE);
            }
            else
            {
                isEnabledOut = !Console.IsOutputRedirected;
                isEnabledErr = !Console.IsErrorRedirected;
            }

            isEnabled = isEnabledOut || isEnabledErr;
        }
    }

    /// <summary>
    /// Returns a string representation of the given object with the specified foreground and background colors applied.
    /// If the colors can't be applied (e.g. the console doesn't support ANSI colors or the NO_COLOR environment variable is set),
    /// the method returns the original string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to generate the colored string for.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns>A string representation of the given object with the specified foreground and background colors applied.</returns>
    public static string? InColor<T>(this T obj, ConsoleColor foreground, ConsoleColor? background = default)
    {
        var text = obj?.ToString();
        if (!isEnabled || text is null || text.Length == 0)
            return text;

        return SpanInColor(text, foreground, background);
    }

    /// <summary>
    /// Returns a string representation of the given object with the specified foreground and background colors applied
    /// and formatted according to the specified format string. If the colors can't be applied (e.g. the console
    /// doesn't support ANSI colors or the NO_COLOR environment variable is set), the method returns the original string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to generate the colored string for.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns>A string representation of the given object with the specified foreground and background colors applied
    /// and formatted according to the specified format string.</returns>
    public static string? InColor<T>(this T obj, string? format, ConsoleColor foreground, ConsoleColor? background = default)
        where T : struct, IFormattable => InColor(obj, format, null, foreground, background);

    /// <summary>
    /// Returns a string representation of the given object with the specified foreground and background colors applied and formatted according to
    /// the specified format string and format provider. If the colors can't be applied (e.g. the console doesn't support ANSI colors or the NO_COLOR
    /// environment variable is set), the method returns the original string.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to generate the colored and formatted string for.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <param name="foreground">The foreground color.</param> /// <param name="background">The background color.</param>
    /// <returns>A string representation of the given object with the specified foreground and background colors applied and formatted according to
    /// the specified format string and format provider.</returns>
    public static string? InColor<T>(this T obj, string? format, IFormatProvider? formatProvider, ConsoleColor foreground, ConsoleColor? background = default)
        where T : struct, IFormattable
    {
        var text = obj.ToString(format, formatProvider);
        if (!isEnabled || text is null || text.Length == 0)
            return text;

        return SpanInColor(text, foreground, background);
    }

    /// <summary>
    /// Writes a string with the specified foreground color to the given <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="value">The string to write to the <see cref="TextWriter"/>.</param>
    public static void Write(this TextWriter writer, ConsoleColor foreground, string value)
    {
        writer.Write(value.InColor(foreground));
    }

    /// <summary>
    /// Writes a string with the specified foreground and background colors to the given <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="background">The background color to use for the string.</param>
    /// <param name="value">The string to write to the <see cref="TextWriter"/>.</param>
    public static void Write(this TextWriter writer, ConsoleColor foreground, ConsoleColor background, string value)
    {
        writer.Write(value.InColor(foreground, background));
    }

    /// <summary>
    /// Writes a string with the specified foreground color to the given <see cref="TextWriter"/>
    /// followed by a line terminator.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="value">The string to write to the <see cref="TextWriter"/>.</param>
    public static void WriteLine(this TextWriter writer, ConsoleColor foreground, string value)
    {
        writer.WriteLine(value.InColor(foreground));
    }

    /// <summary>
    /// Writes a string with the specified foreground color to the given <see cref="TextWriter"/>
    /// followed by a line terminator.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="background">The background color to use for the string.</param>
    /// <param name="value">The string to write to the <see cref="TextWriter"/>.</param>
    public static void WriteLine(this TextWriter writer, ConsoleColor foreground, ConsoleColor background, string value)
    {
        writer.WriteLine(value.InColor(foreground, background));
    }

    /// <summary>
    /// Determines if colorization is enabled for the given <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to check. If <c>null</c>, uses the default behavior.</param>
    /// <returns><c>true</c> if colorization is enabled for the given <see cref="TextWriter"/>; otherwise, <c>false</c>.</returns>
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
