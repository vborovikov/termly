namespace Termly;

using System;
using System.Runtime.CompilerServices;

public static partial class Colorizer
{
    /// <summary>
    /// Writes an interpolated string with the specified foreground color to the given <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="colorHandler">The interpolated string handler that contains the string to write.</param>
    public static void Write(this TextWriter writer, ConsoleColor foreground,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground))] ref InterpolatedStringHandler colorHandler)
    {
        writer.Write(colorHandler.ToStringAndClear());
    }

    /// <summary>
    /// Writes an interpolated string with the specified foreground and background colors to the given <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="background">The background color to use for the string.</param>
    /// <param name="colorHandler">The interpolated string handler that contains the string to write.</param>
    public static void Write(this TextWriter writer, ConsoleColor foreground, ConsoleColor background,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground), nameof(background))] ref InterpolatedStringHandler colorHandler)
    {
        writer.Write(colorHandler.ToStringAndClear());
    }

    /// <summary>
    /// Writes an interpolated string with the specified foreground color followed by a line terminator to the given <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="colorHandler">The interpolated string handler that contains the string to write.</param>
    public static void WriteLine(this TextWriter writer, ConsoleColor foreground,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground))] ref InterpolatedStringHandler colorHandler)
    {
        writer.WriteLine(colorHandler.ToStringAndClear());
    }

    /// <summary>
    /// Writes an interpolated string with the specified foreground and background colors followed by a line terminator to the given <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write the colored string to.</param>
    /// <param name="foreground">The foreground color to use for the string.</param>
    /// <param name="background">The background color to use for the string.</param>
    /// <param name="colorHandler">The interpolated string handler that contains the string to write.</param>
    public static void WriteLine(this TextWriter writer, ConsoleColor foreground, ConsoleColor background,
        [InterpolatedStringHandlerArgument(nameof(writer), nameof(foreground), nameof(background))] ref InterpolatedStringHandler colorHandler)
    {
        writer.WriteLine(colorHandler.ToStringAndClear());
    }

    /// <summary>
    /// Provides an interpolated string handler for colorizing output to a <see cref="TextWriter"/>.
    /// </summary>
    [InterpolatedStringHandler]
    public ref struct InterpolatedStringHandler
    {
        private readonly bool isEnabled;
        private DefaultInterpolatedStringHandler handler;
        private readonly string colorCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringHandler"/> struct with the given parameters.
        /// </summary>
        /// <param name="literalLength">The length of the literal portion of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted expressions in the interpolated string.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
        /// <param name="foreground">The foreground color to use for the colorized output.</param>
        public InterpolatedStringHandler(int literalLength, int formattedCount,
            TextWriter writer, ConsoleColor foreground)
            : this(literalLength, formattedCount, null, writer, foreground)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringHandler"/> struct with the given parameters.
        /// </summary>
        /// <param name="literalLength">The length of the literal portion of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted expressions in the interpolated string.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
        /// <param name="foreground">The foreground color to use for the colorized output.</param>
        /// <param name="background">The background color to use for the colorized output.</param>
        public InterpolatedStringHandler(int literalLength, int formattedCount,
            TextWriter writer, ConsoleColor foreground, ConsoleColor background)
            : this(literalLength, formattedCount, null, writer, foreground, background)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringHandler"/> struct with the given parameters.
        /// </summary>
        /// <param name="literalLength">The length of the literal portion of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted expressions in the interpolated string.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> to use for formatting.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
        /// <param name="foreground">The foreground color to use for the colorized output.</param>
        public InterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider,
            TextWriter writer, ConsoleColor foreground)
        {
            this.isEnabled = IsEnabledFor(writer);
            if (this.isEnabled)
                literalLength += formattedCount * 18;
            this.handler = new(literalLength, formattedCount, provider);
            this.colorCode = ForegroundCodes[(int)foreground];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringHandler"/> struct with the given parameters.
        /// </summary>
        /// <param name="literalLength">The length of the literal portion of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted expressions in the interpolated string.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> to use for formatting.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
        /// <param name="foreground">The foreground color to use for the colorized output.</param>
        /// <param name="background">The background color to use for the colorized output.</param>
        public InterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider,
            TextWriter writer, ConsoleColor foreground, ConsoleColor background)
            : this(literalLength, formattedCount, provider, writer, foreground)
        {
            this.colorCode += BackgroundCodes[(int)background];
        }

        /// <inheritdoc/>
        public override string ToString() => this.handler.ToString();

        /// <summary>Gets the built <see cref="string"/> and clears the handler.</summary>
        /// <returns>The built string.</returns>
        internal ReadOnlySpan<char> ToStringAndClear() => this.handler.ToStringAndClear();

        /// <summary>Writes the specified string to the handler.</summary>
        /// <param name="value">The string to write.</param>
        public void AppendLiteral(string value)
        {
            this.handler.AppendLiteral(value);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, string? format)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment, string? format)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified character span to the handler.</summary>
        /// <param name="value">The span to write.</param>
        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified string of chars to the handler.</summary>
        /// <param name="value">The span to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        public void AppendFormatted(string? value)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(string? value, int alignment = 0, string? format = null)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(object? value, int alignment = 0, string? format = null)
        {
            if (this.isEnabled) this.handler.AppendLiteral(this.colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            if (this.isEnabled) this.handler.AppendLiteral(ResetCode);
        }
    }
}