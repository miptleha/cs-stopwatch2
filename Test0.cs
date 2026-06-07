using Misc;
using System;
using System.Threading;

internal partial class Tests
{
    public static void Test0()
    {
        Console.WriteLine("--- Minimal working example ---");

        var sw = Stopwatch2.StartNew();
        var t = sw.Start("Task");
        Thread.Sleep(100);
        t.Stop();
        sw.Stop();
        Console.WriteLine(sw.Results());
        Console.WriteLine();
    }
}
