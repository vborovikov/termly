namespace Termly;

public class ConsoleStatus : ConsoleLine
{
    private int maxWidth;

    public ConsoleStatus(bool indent = false) : base(indent)
    {
    }

    protected override int MaxWidth => this.maxWidth;

    public void Write(string value)
    {
        Update(con =>
        {
            this.maxWidth = Math.Max(this.maxWidth, value.Length);
            con.Write(value);
        }, clear: true);
    }

    public void Write(ConsoleColor foreground, string value)
    {
        Write(value.InColor(foreground)!);
    }

    protected override void Clear()
    {
        // keep whatever we have
    }
}