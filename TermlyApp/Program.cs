using System.Text;
using Termly;
using Termly.Widgets;

Console.Out.PrintLine($"{"Hello":blue}, {"World":white|green}!");
Console.Error.PrintLine(ConsoleColor.Red, $"{"Hello":x}, {"World":x}!");
Console.Out.PrintLine($"{12345.67:N2:cyan}");
Console.Out.PrintLine($"{DateTimeOffset.Now:HH:mm:yellow}");
using (var progressTwirl = new ConsoleProgressTwirl())
{
    DoWork(progressTwirl);
}

using (var progressTwirl = new ConsoleProgressTwirl { Style = ConsoleProgressTwirl.Braille, Done = '\u2713' })
using (var status = new ConsoleStatus())
{
    status.Write(ConsoleColor.Blue, "Doing work...");
    DoWork(progressTwirl);
    status.Write(ConsoleColor.Green, "Done!");
}
Console.Error.WriteLine();

using (var stopwatch = new ConsoleStopwatch { Format = @"ss\.fff", Resolution = TimeSpan.FromMilliseconds(5) })
using (var progressBar = new ConsoleProgressBar())
using (var percentage = new ConsoleStatus())
{
    stopwatch.Start();
    var progress = new Progress<int>(value =>
    {
        progressBar.Report(value);
        percentage.Write(value.ToString());
    });
    DoWork(progress);
    percentage.Write("");
}

using (var progressBar = new ConsoleProgressBar { Block = ConsoleProgressBar.Square, Width = 20 })
{
    DoWork(progressBar);
}

static void DoWork(IProgress<int> progress)
{
    progress.Report(0);
    for (var i = 0; i <= 100; ++i)
    {
        progress.Report(i);
        Thread.Sleep(50);
    }
}