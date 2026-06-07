using Misc;
using System;
using System.Threading;

internal partial class Tests
{
    public static void Test5(bool simpleMode)
    {
        Console.WriteLine("--- Dummy stopwatch " + (simpleMode ? "(Simple mode) " : "") + "---");

        var sw = new Stopwatch2(simpleMode);
        sw.Start();
        var t = sw.Start("Dummy");
        Dummy(t);
        t.Stop();
        sw.Stop();

        if (simpleMode)
            Console.WriteLine("Stopwatch2: " + sw.Elapsed);
        else
            Console.WriteLine(sw.Results());

        Dummy(Stopwatch2Task.Dummy); //no measurement needed, to avoid NullReferenceException

        Console.WriteLine();
    }

    static void Dummy(Stopwatch2Task t)
    {
        using (var t1 = t.Start("Some task"))
            Thread.Sleep(100);
    }
}
