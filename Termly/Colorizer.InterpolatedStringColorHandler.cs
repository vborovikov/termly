namespace Termly;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public static partial class Colorizer
{
    /// <summary>
    /// Writes an interpolated string with colorization to a <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
    /// <param name="colorHandler">The <see cref="InterpolatedStringColorHandler"/> to use for colorizing output.</param>
    public static void WriteInColor(this TextWriter writer,
        [InterpolatedStringHandlerArgument(nameof(writer))] ref InterpolatedStringColorHandler colorHandler)
    {
        writer.Write(colorHandler.ToStringAndClear());
    }

    /// <summary>
    /// Writes an interpolated string with colorization to a <see cref="TextWriter"/> followed by a line terminator.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
    /// <param name="colorHandler">The <see cref="InterpolatedStringColorHandler"/> to use for colorizing output.</param>
    public static void WriteLineInColor(this TextWriter writer,
        [InterpolatedStringHandlerArgument(nameof(writer))] ref InterpolatedStringColorHandler colorHandler)
    {
        writer.WriteLine(colorHandler.ToStringAndClear());
    }

    /// <summary>
    /// Provides an interpolated string handler for colorizing output to a <see cref="TextWriter"/>.
    /// </summary>
    [InterpolatedStringHandler]
    public ref struct InterpolatedStringColorHandler
    {
        private readonly bool isEnabled;
        private DefaultInterpolatedStringHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringColorHandler"/> struct with the given parameters.
        /// </summary>
        /// <param name="literalLength">The length of the literal portion of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted expressions in the interpolated string.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
        public InterpolatedStringColorHandler(int literalLength, int formattedCount, TextWriter writer)
            : this(literalLength, formattedCount, null, writer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedStringColorHandler"/> struct with the given parameters.
        /// </summary>
        /// <param name="literalLength">The length of the literal portion of the interpolated string.</param>
        /// <param name="formattedCount">The number of formatted expressions in the interpolated string.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> to use for formatting.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write colorized output to.</param>
        public InterpolatedStringColorHandler(int literalLength, int formattedCount, IFormatProvider? provider, TextWriter writer)
        {
            this.isEnabled = IsEnabledFor(writer);
            if (this.isEnabled)
                literalLength += formattedCount * 18;
            this.handler = new(literalLength, formattedCount, provider);
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
            this.handler.AppendFormatted(value);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, string? format)
        {
            if (HasColor(format, out var colorCode))
            {
                this.handler.AppendLiteral(colorCode);
                this.handler.AppendFormatted(value);
                this.handler.AppendLiteral(ResetCode);
            }
            else
            {
                this.handler.AppendFormatted(value, format);
            }
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment)
        {
            this.handler.AppendFormatted(value, alignment);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, int alignment, string? format)
        {
            if (HasColor(format, out var colorCode))
            {
                this.handler.AppendLiteral(colorCode);
                this.handler.AppendFormatted(value, alignment);
                this.handler.AppendLiteral(ResetCode);
            }
            else
            {
                this.handler.AppendFormatted(value, alignment, format);
            }
        }

        /// <summary>Writes the specified character span to the handler.</summary>
        /// <param name="value">The span to write.</param>
        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <summary>Writes the specified string of chars to the handler.</summary>
        /// <param name="value">The span to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
        {
            if (HasColor(format, out var colorCode))
            {
                this.handler.AppendLiteral(colorCode);
                this.handler.AppendFormatted(value, alignment);
                this.handler.AppendLiteral(ResetCode);
            }
            else
            {
                this.handler.AppendFormatted(value, alignment, format);
            }
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        public void AppendFormatted(string? value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(string? value, int alignment = 0, string? format = null)
        {
            if (HasColor(format, out var colorCode))
            {
                this.handler.AppendLiteral(colorCode);
                this.handler.AppendFormatted(value, alignment);
                this.handler.AppendLiteral(ResetCode);
            }
            else
            {
                this.handler.AppendFormatted(value, alignment, format);
            }
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="alignment">Minimum number of characters that should be written for this value.
        /// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
        /// <param name="format">The format string.</param>
        public void AppendFormatted(object? value, int alignment = 0, string? format = null)
        {
            if (HasColor(format, out var colorCode))
            {
                this.handler.AppendLiteral(colorCode);
                this.handler.AppendFormatted(value, alignment);
                this.handler.AppendLiteral(ResetCode);
            }
            else
            {
                this.handler.AppendFormatted(value, alignment, format);
            }
        }

        private bool HasColor(ReadOnlySpan<char> format, [NotNullWhen(true)] out string? color)
        {
            if (!this.isEnabled || format.IsEmpty)
            {
                color = null;
                return false;
            }

            ConsoleColor foreground, background;

            var pos = format.IndexOf('|');
            if (pos > 0 &&
                Enum.TryParse(format[..pos], true, out foreground) &&
                Enum.TryParse(format[(pos + 1)..], true, out background))
            {
                //todo: return the rest of format if another separator is present

                color = ForegroundCodes[(int)foreground] + BackgroundCodes[(int)background];
                return true;
            }

            if (Enum.TryParse(format, true, out foreground))
            {
                color = ForegroundCodes[(int)foreground];
                return true;
            }

            color = null;
            return false;
        }
    }
}
