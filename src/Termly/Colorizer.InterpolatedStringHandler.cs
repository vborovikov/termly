namespace Termly;

using System;
using System.Runtime.CompilerServices;

public static partial class Colorizer
{
    public static void Write(this TextWriter writer, ConsoleColor foreground,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground))] ref InterpolatedStringHandler colorHandler)
    {
        writer.Write(colorHandler.ToStringAndClear());
    }

    public static void Write(this TextWriter writer, ConsoleColor foreground, ConsoleColor background,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground), nameof(background))] ref InterpolatedStringHandler colorHandler)
    {
        writer.Write(colorHandler.ToStringAndClear());
    }

    public static void WriteLine(this TextWriter writer, ConsoleColor foreground,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground))] ref InterpolatedStringHandler colorHandler)
    {
        writer.WriteLine(colorHandler.ToStringAndClear());
    }

    public static void WriteLine(this TextWriter writer, ConsoleColor foreground, ConsoleColor background,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground), nameof(background))] ref InterpolatedStringHandler colorHandler)
    {
        writer.WriteLine(colorHandler.ToStringAndClear());
    }

    [InterpolatedStringHandler]
    public ref struct InterpolatedStringHandler
    {
        private readonly bool isEnabled;
        private DefaultInterpolatedStringHandler handler;
        private readonly string colorCode;

        public InterpolatedStringHandler(int literalLength, int formattedCount,
            TextWriter writer, ConsoleColor foreground)
            : this(literalLength, formattedCount, null, writer, foreground)
        {
        }

        public InterpolatedStringHandler(int literalLength, int formattedCount,
            TextWriter writer, ConsoleColor foreground, ConsoleColor background)
            : this(literalLength, formattedCount, null, writer, foreground, background)
        {
        }

        public InterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider,
            TextWriter writer, ConsoleColor foreground)
        {
            this.isEnabled = IsEnabledFor(writer);
            if (this.isEnabled)
                literalLength += formattedCount * 18;
            this.handler = new(literalLength, formattedCount, provider);
            this.colorCode = ForegroundCodes[(int)foreground];
        }

        public InterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider,
            TextWriter writer, ConsoleColor foreground, ConsoleColor background)
            : this(literalLength, formattedCount, provider, writer, foreground)
        {
            this.colorCode += BackgroundCodes[(int)background];
        }

        public override string ToString() => this.handler.ToString();

        internal ReadOnlySpan<char> ToStringAndClear() => this.handler.ToStringAndClear();

        public void AppendLiteral(string value)
        {
            this.handler.AppendLiteral(value);
        }

        public void AppendFormatted<T>(T value)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted<T>(T value, string? format)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted<T>(T value, int alignment)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted<T>(T value, int alignment, string? format)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted(string? value)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted(string? value, int alignment = 0, string? format = null)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        public void AppendFormatted(object? value, int alignment = 0, string? format = null)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }
    }
}