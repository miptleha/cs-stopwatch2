using Misc;
using System;
using System.Diagnostics;
using System.Linq;

internal partial class Tests
{
    public static void Test6(bool simpleMode)
    {
        Console.WriteLine("--- Million tasks " + (simpleMode ? "(Simple mode) " : "") + "---");

        var sw = Stopwatch.StartNew();
        var sw2 = Stopwatch2.StartNew(simpleMode);
        for (int i = 0; i < MILLION; i++)
        {
            var t = sw2.Start("Task " + i);
            t.Stop();
        }
        sw2.Stop();
        sw.Stop();

        Console.WriteLine("Stopwatch2: " + sw2.Elapsed);
        Console.WriteLine("Stopwatch: " + sw.Elapsed);

        sw.Restart();
        var results = sw2.Results();
        sw.Stop();
        int cnt = results.Count(c => c == '\n') + 1;
        Console.WriteLine("Stopwatch2 result rows: " + cnt.ToString("N0") + ", time of execution Results(): " + sw.Elapsed);

        Console.WriteLine();
    }
}
