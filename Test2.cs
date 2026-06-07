using System;
using System.Diagnostics;
using System.Threading;
using Misc;

internal partial class Tests
{
    public static void Test2(bool simpleMode)
    {
        Console.WriteLine("--- Inside functions " + (simpleMode ? "(Simple mode) " : "") + "---");

        var sw = Stopwatch.StartNew();
        var sw2 = Stopwatch2.StartNew(simpleMode);
        for (int i = 0; i < ITERATIONS_NUMBER; i++)
        {
            using (var t = sw2.Start("f1"))
                f1();

            using (var t = sw2.Start("f2"))
                f2(t);
        }
        sw2.Stop();
        sw.Stop();
        
        if (simpleMode)
            Console.WriteLine("Stopwatch2: " + sw2.Elapsed);
        else
            Console.WriteLine(sw2.Results());

        Console.WriteLine("Stopwatch: " + sw.Elapsed);
        Console.WriteLine();
    }

    static void f1()
    {
        Thread.Sleep(10);
    }

    static void f2(Stopwatch2Task t)
    {
        Thread.Sleep(10);
        using (var t1 = t.Start("f3"))
            f3(t1);
    }

    static void f3(Stopwatch2Task t)
    {
        using (var t1 = t.Start("f4"))
            f4();
    }

    static void f4()
    {
        Thread.Sleep(10);
    }
}
