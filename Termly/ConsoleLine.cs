namespace Termly;

public abstract class ConsoleLine : IDisposable
{
    private static readonly string Whitespace = new(' ', 80);
    private static readonly List<ConsoleLine> lines = [];
    private static SpinLock spinLock = new();

    private readonly (int Left, int Top) position;

    protected ConsoleLine()
    {
        this.IsEnabled = !Console.IsErrorRedirected;
        if (this.IsEnabled)
        {
            this.position = GetPosition(this);
        }
    }

    private static (int Left, int Top) GetPosition(ConsoleLine line)
    {
        var lockTaken = false;
        try
        {
            spinLock.Enter(ref lockTaken);
            var cursorPosition = (Console.CursorLeft, Console.CursorTop);

            if (lines.Count > 0)
            {
                cursorPosition.CursorLeft = 0;
                foreach (var other in lines)
                {
                    if (other.position.Top == cursorPosition.CursorTop)
                    {
                        cursorPosition.CursorLeft += 
                            other.position.Left + other.Margin.Left + other.MaxWidth + other.Margin.Right;
                    }
                }
            }

            lines.Add(line);
            return cursorPosition;
        }
        finally
        {
            if (lockTaken)
            {
                spinLock.Exit();
            }
        }
    }

    public bool IsEnabled { get; }

    public (int Left, int Right) Margin { get; init; }

    protected abstract int MaxWidth { get; }

    public void Dispose()
    {
        if (this.IsEnabled)
        {
            Clear();
            lines.Remove(this);
        }
    }

    protected virtual void Clear()
    {
        Update(con => { }, clear: true);
    }

    protected void Update(Action<TextWriter> update, bool clear = false)
    {
        if (!this.IsEnabled)
            return;

        var lockTaken = false;
        try
        {
            spinLock.Enter(ref lockTaken);
            Console.SetCursorPosition(this.position.Left, this.position.Top);

            if (clear)
            {
                Clear(Console.Error, this.MaxWidth + this.Margin.Left + this.Margin.Right);
                Console.SetCursorPosition(this.position.Left + this.Margin.Left, this.position.Top);
            }
            update(Console.Error);
        }
        finally
        {
            if (lockTaken)
            {
                spinLock.Exit();
            }
        }
    }

    private static void Clear(TextWriter output, int width)
    {
        while (width >= Whitespace.Length)
        {
            output.Write(Whitespace);
            width -= Whitespace.Length;
        }
        if (width > 0)
        {
            output.Write(Whitespace.AsSpan()[..width]);
        }
    }
}