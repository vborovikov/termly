using System.Text;
using Termly;

Console.OutputEncoding = Encoding.Default;

Console.Out.WriteLineInColor($"{"Hello":blue}, {"World":white|green}!");
Console.Error.WriteLine(ConsoleColor.Red, $"{"Hello":x}, {"World":x}!");Console.Out.WriteLineInColor($"{12345.67:N2:cyan}");
Console.Out.WriteLineInColor($"{DateTimeOffset.Now:HH:mm:yellow}");
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

using (var progressBar = new ConsoleProgressBar())
using (var percentage = new ConsoleStatus())
{
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