namespace Termly;

using System;

public abstract class ConsoleProgress : IProgress<int>, IDisposable
{
    protected const char Backspace = '\b';

    protected ConsoleProgress()
    {
        this.IsEnabled = !Console.IsErrorRedirected;
        if (this.IsEnabled)
        {
            Console.CursorVisible = false;
        }
    }

    public bool IsEnabled { get; }

    public void Dispose()
    {
        if (this.IsEnabled)
        {
            Clear();
            Console.CursorVisible = true;
        }
    }

    public void Report(int value)
    {
        if (this.IsEnabled)
        {
            Update(value);
        }
    }

    protected abstract void Update(int value);

    protected abstract void Clear();
}

public class ConsoleProgressTwirl : ConsoleProgress
{
    private const string DefaultTwirl = @"-\|/";

    private readonly string twirl = DefaultTwirl;

    public ConsoleProgressTwirl()
    {
    }

    public ConsoleProgressTwirl(string twirl)
    {
        this.twirl = twirl;
    }

    protected override void Update(int value)
    {
        Console.Error.Write(Backspace);
        Console.Error.Write(this.twirl[value % this.twirl.Length]);
    }

    protected override void Clear()
    {
        Console.Error.Write(Backspace);
        Console.Error.Write(' ');
        Console.Error.Write(Backspace);
    }
}

public class ConsoleProgressBar : ConsoleProgress
{
    public const char DefaultBlock = '■';
    public const char DefaultSpot = ' ';

    private readonly char block = DefaultBlock;
    private readonly char spot = DefaultSpot;
    private readonly bool displayBorder = true;
    private readonly bool displayPercent = true;
    private readonly string backspace;

    public ConsoleProgressBar() : base()
    {
        this.backspace = new(Backspace, 17);
    }

    public ConsoleProgressBar(char barBlock, char barSpot = DefaultSpot, bool displayBorder = true, bool displayPercent = true)
        : base()
    {
        this.block = barBlock;
        this.spot = barSpot;
        this.displayBorder = displayBorder;
        this.displayPercent = displayPercent;
        var backspaceLength = 10;
        if (displayBorder)
        {
            backspaceLength += 2;
        }
        if (displayPercent)
        {
            backspaceLength += 5;
        }
        this.backspace = new(Backspace, backspaceLength);
    }

    protected override void Update(int value)
    {
        Console.Error.Write(backspace);

        if (this.displayBorder)
        {
            Console.Error.Write("[");
        }

        var p = (int)((value / 10f) + .5f);
        for (var i = 0; i < 10; ++i)
        {
            if (i >= p)
            {
                Console.Error.Write(this.spot);
            }
            else
            {
                Console.Error.Write(this.block);
            }
        }

        if (this.displayBorder)
        {
            Console.Error.Write("]");
        }

        if (this.displayPercent)
        {
            Console.Error.Write(" {0,3:##0}%", value);
        }
    }

    protected override void Clear()
    {
        Console.Error.Write(this.backspace);
        Console.Error.Write(new string(' ', this.backspace.Length));
        Console.Error.Write(this.backspace);
    }
}
