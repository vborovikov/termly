namespace Termly;

using System;

public abstract class ConsoleProgress : ConsoleLine, IProgress<int>, IDisposable
{
    protected ConsoleProgress(bool indent) : base(indent)
    {
        if (this.IsEnabled)
        {
            Console.CursorVisible = false;
            Report(default);
        }
    }

    public void Report(int value)
    {
        Update(con => Update(con, value));
    }

    protected override void Clear()
    {
        base.Clear();
        Console.CursorVisible = true;
    }

    protected abstract void Update(TextWriter con, int value);
}

public class ConsoleProgressTwirl : ConsoleProgress
{
    private const string DefaultStyle = @"-\|/";
    public const string Braille = "⣾⣽⣻⢿⡿⣟⣯⣷";
    public const string Clock = "╷┐╴┘╵└╶┌";

    public ConsoleProgressTwirl(bool indent = false) : base(indent) { }

    protected override int MaxWidth => 1;

    public string Style { get; init; } = DefaultStyle;

    public char Done { get; init; } = ' ';

    protected override void Update(TextWriter con, int value)
    {
        con.Write(this.Style[value % this.Style.Length]);
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

public class ConsoleProgressBar : ConsoleProgress
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

    public ConsoleProgressBar(bool indent = false) : base(indent)
    {
    }

    protected override int MaxWidth => this.Border.Width + this.Width;

    public BlockStyle Block { get; init; } = DefaultBlock;

    public BorderStyle Border { get; init; } = DefaultBorder;

    public int Width { get; init; } = 10;

    protected override void Update(TextWriter con, int value)
    {
        if (this.Border.Left != default)
        {
            con.Write(this.Border.Left);
        }

        var p = (int)MathF.Ceiling(this.Width * value / (float)100);
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
