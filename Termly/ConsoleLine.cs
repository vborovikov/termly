namespace Termly;

public abstract class ConsoleLine : IDisposable
{
    private static readonly string Whitespace = new(' ', 80);

    private readonly (int Left, int Top) position;
    private SpinLock spinLock = new();

    protected ConsoleLine()
    {
        this.IsEnabled = !Console.IsErrorRedirected;
        if (this.IsEnabled)
        {
            //todo: have global registry of currently occupied positions on the line?
            this.position = (Console.CursorLeft, Console.CursorTop);
        }
    }

    public bool IsEnabled { get; }

    //todo: use instead of indent
    public (int Left, int Right) Margin { get; init; }

    protected abstract int MaxWidth { get; }

    public void Dispose()
    {
        if (this.IsEnabled)
        {
            Clear();
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
            this.spinLock.Enter(ref lockTaken);
            Console.SetCursorPosition(this.position.Left - this.Margin.Left, this.position.Top);

            if (clear)
            {
                Clear(Console.Error, this.MaxWidth + this.Margin.Left + this.Margin.Right);
                Console.SetCursorPosition(this.position.Left, this.position.Top);
            }
            update(Console.Error);
        }
        finally
        {
            if (lockTaken)
            {
                this.spinLock.Exit();
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