namespace Termly;

using System;

public abstract class ConsoleProgress<T> : ConsoleLine, IProgress<T>, IDisposable
{
    protected const int MinPercent = 0;
    protected const int MaxPercent = 100;
    private readonly Func<T, int> percentage;

    protected ConsoleProgress(Func<T, int> percentage, bool indent) : base(indent)
    {
        this.percentage = percentage;
        if (this.IsEnabled)
        {
            Console.CursorVisible = false;
            Report(default!);
        }
    }

    public void Report(T value)
    {
        Update(con => Update(con, this.percentage(value)));
    }

    protected override void Clear()
    {
        base.Clear();
        Console.CursorVisible = true;
    }

    protected abstract void Update(TextWriter con, int percent);
}

public class ConsoleProgressTwirl<T> : ConsoleProgress<T>
    where T : struct, IConvertible
{
    private const string DefaultStyle = @"-\|/";
    public const string Braille = "⣾⣽⣻⢿⡿⣟⣯⣷";
    public const string Clock = "╷┐╴┘╵└╶┌";

    public ConsoleProgressTwirl(Func<T, int> percentage, bool indent = false) : base(percentage, indent) { }

    protected override int MaxWidth => 1;

    public string Style { get; init; } = DefaultStyle;

    public char Done { get; init; } = ' ';

    protected override void Update(TextWriter con, int percent)
    {
        con.Write(this.Style[percent % this.Style.Length]);
    }

    protected override void Clear()
    {
        if (char.IsWhiteSpace(this.Done))
        {
            base.Clear();
        }
        else
        {
            Update(con => con.Write(this.Done));
        }
    }
}

public class ConsoleProgressTwirl : ConsoleProgressTwirl<int>
{
    public ConsoleProgressTwirl(bool indent = false) : base(p => Math.Clamp(p, MinPercent, MaxPercent), indent) { }
}

public class ConsoleProgressBar<T> : ConsoleProgress<T>
    where T : struct, IConvertible
{
    public readonly record struct BlockStyle(char Filling, char Padding = ' ');
    public readonly record struct BorderStyle(char Left, char Right)
    {
        public int Width => (this.Left != default ? 1 : 0) + (this.Right != default ? 1 : 0);
    }

    public static readonly BlockStyle DefaultBlock = new('#', '-');
    public static readonly BorderStyle DefaultBorder = new('[', ']');
    public static readonly BlockStyle Square = new('■');
    public static readonly BorderStyle NoBorder = default;

    public ConsoleProgressBar(Func<T, int> percentage, bool indent = false) : base(percentage, indent) { }

    protected override int MaxWidth => this.Border.Width + this.Width;

    public BlockStyle Block { get; init; } = DefaultBlock;

    public BorderStyle Border { get; init; } = DefaultBorder;

    public int Width { get; init; } = 10;

    protected override void Update(TextWriter con, int percent)
    {
        if (this.Border.Left != default)
        {
            con.Write(this.Border.Left);
        }

        var p = (int)MathF.Ceiling(this.Width * percent / (float)MaxPercent);
        for (var i = 0; i < this.Width; ++i)
        {
            if (i < p)
            {
                con.Write(this.Block.Filling);
            }
            else
            {
                con.Write(this.Block.Padding);
            }
        }

        if (this.Border.Right != default)
        {
            con.Write(this.Border.Right);
        }
    }
}

public class ConsoleProgressBar : ConsoleProgressBar<int>
{
    public ConsoleProgressBar(bool indent = false) : base(p => Math.Clamp(p, MinPercent, MaxPercent), indent) { }
}

