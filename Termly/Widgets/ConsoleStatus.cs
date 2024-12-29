namespace Termly.Widgets;

public class ConsoleStatus : ConsoleLine
{
    private int maxWidth;

    public ConsoleStatus()
    {
        this.Margin = (1, 1);
    }

    protected override int MaxWidth => this.maxWidth;

    public void Write(string value)
    {
        this.maxWidth = Math.Max(this.maxWidth, value.Length);
        Update(value);
    }

    public void Write(ConsoleColor foreground, string value)
    {
        this.maxWidth = Math.Max(this.maxWidth, value.Length);
        Update(value.InColor(foreground)!);
    }

    private void Update(string value) => Update(con => con.Write(value), clear: true);

    protected override void Clear()
    {
        // keep whatever we have
    }
}