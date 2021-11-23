namespace Termly;

using System;
using System.Runtime.CompilerServices;
using System.Text;

public static partial class Colorizer
{
    [InterpolatedStringHandler]
    public ref struct InterpolatedStringHandler
    {
        private StringBuilder stringBuilder;
        private StringBuilder.AppendInterpolatedStringHandler handler;
        private readonly string colorCode;

        public InterpolatedStringHandler(int literalLength, int formattedCount, ConsoleColor foreground)
        {
            this.stringBuilder = new StringBuilder();
            this.handler = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, this.stringBuilder);
            this.colorCode = ForegroundCodes[(int)foreground];
        }

        public InterpolatedStringHandler(int literalLength, int formattedCount, ConsoleColor foreground, ConsoleColor background)
            : this(literalLength, formattedCount, foreground)
        {
            this.colorCode += BackgroundCodes[(int)background];
        }

        public override string ToString() => this.stringBuilder.ToString();

        internal string ToStringAndClear()
        {
            var str = this.stringBuilder.ToString();
            this.handler = default;
            this.stringBuilder.Clear();
            this.stringBuilder = default;
            return str;
        }

        public void AppendLiteral(string value) => this.handler.AppendLiteral(value);

        public void AppendFormatted<T>(T value)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted<T>(T value, string format)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value, format);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted<T>(T value, int alignment)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value, alignment);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted<T>(T value, int alignment, string format)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string format = null)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted(string value)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted(string value, int alignment = 0, string format = null)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            this.stringBuilder.Append(ResetCode);
        }

        public void AppendFormatted(object value, int alignment = 0, string format = null)
        {
            this.stringBuilder.Append(colorCode);
            this.handler.AppendFormatted(value, alignment, format);
            this.stringBuilder.Append(ResetCode);
        }
    }
}

