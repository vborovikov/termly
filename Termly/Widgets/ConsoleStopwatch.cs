namespace Termly.Widgets;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class ConsoleStopwatch : ConsoleLine
{
    private readonly Stopwatch stopwatch;
    private CancellationTokenSource? cancellation;

    public ConsoleStopwatch()
    {
        this.stopwatch = new();
        this.Margin = (1, 1);
    }

    public string Format { get; init; } = @"mm\:ss";

    public TimeSpan Resolution { get; init; } = TimeSpan.FromSeconds(1);

    protected override int MaxWidth => this.Format.Length - this.Format.Count(ch => ch is '\\' or '%');

    protected override void Clear()
    {
        Stop();
    }

    public void Start()
    {
        stopwatch.Start();
        this.cancellation ??= new CancellationTokenSource();
        RunUpdate(this.cancellation.Token);
    }

    public void Stop()
    {
        this.cancellation?.Cancel();
        this.cancellation = null;
        this.stopwatch.Stop();
    }

    public void Reset()
    {
        Stop();
        this.stopwatch.Reset();
        Update();
    }

    public void Restart()
    {
        Reset();
        Start();
    }

    private void RunUpdate(CancellationToken cancellationToken)
    {
        var updateTask = Task.Run(() => UpdateAsync(cancellationToken), cancellationToken);
        if (!updateTask.IsCompleted || updateTask.IsFaulted)
        {
            _ = UpdateAwaited(updateTask);
        }

        async static Task UpdateAwaited(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(this.Resolution);
        do
        {
            Update();
        } while (await timer.WaitForNextTickAsync(cancellationToken));
    }

    private void Update()
    {
        Update(con => con.Write(this.stopwatch.Elapsed.ToString(this.Format)));
    }
}
