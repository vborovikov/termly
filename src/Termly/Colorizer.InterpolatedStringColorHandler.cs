namespace Termly;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public static partial class Colorizer
{
    public static void WriteInColor(this TextWriter writer,
        [InterpolatedStringHandlerArgument(nameof(writer))] ref InterpolatedStringColorHandler colorHandler)
    {
        writer.Write(colorHandler.ToStringAndClear());
    }

    public static void WriteLineInColor(this TextWriter writer,
        [InterpolatedStringHandlerArgument(nameof(writer))] ref InterpolatedStringColorHandler colorHandler)
    {
        writer.WriteLine(colorHandler.ToStringAndClear());
    }

    [InterpolatedStringHandler]
    public ref struct InterpolatedStringColorHandler
    {
        private readonly bool isEnabled;
        private DefaultInterpolatedStringHandler handler;

        public InterpolatedStringColorHandler(int literalLength, int formattedCount, TextWriter writer)
            : this(literalLength, formattedCount, null, writer)
        {
        }

        public InterpolatedStringColorHandler(int literalLength, int formattedCount, IFormatProvider? provider, TextWriter writer)
        {
            this.isEnabled = IsEnabledFor(writer);
            if (this.isEnabled)
                literalLength += formattedCount * 18;
            this.handler = new(literalLength, formattedCount, provider);
        }

        public override string ToString() => this.handler.ToString();

        internal ReadOnlySpan<char> ToStringAndClear() => this.handler.ToStringAndClear();

        public void AppendLiteral(string value)
        {
            this.handler.AppendLiteral(value);
        }

        public void AppendFormatted<T>(T value)
        {
            this.handler.AppendFormatted(value);
        }

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

        public void AppendFormatted<T>(T value, int alignment)
        {
            this.handler.AppendFormatted(value, alignment);
        }

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

        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            this.handler.AppendFormatted(value);
        }

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

        public void AppendFormatted(string? value)
        {
            this.handler.AppendFormatted(value);
        }

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
