using Termly;

Console.Out.WriteLineInColor($"{"Hello":blue}, {"World":white|green}!");
Console.Error.WriteLine(ConsoleColor.Red, $"{"Hello":x}, {"World":x}!");

using (var progressTwirl = new ConsoleProgressTwirl())
{
    DoWork(progressTwirl);
}

using (var progressBar = new ConsoleProgressBar())
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