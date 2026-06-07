using System;
using System.Diagnostics;
using System.Threading;
using Misc;

internal partial class Tests
{
    public static void Test1(bool simpleMode)
    {
        Console.WriteLine("--- Sequential operations " + (simpleMode ? "(Simple mode) " : "") + "---");

        var sw = Stopwatch.StartNew();
        var sw2 = Stopwatch2.StartNew(simpleMode);
        for (int i = 0; i < ITERATIONS_NUMBER; i++)
        {
            var t = sw2.Start("Task1");
            Thread.Sleep(20);
            t.Stop();

            using (var t1 = sw2.Start("Task2")) //Automatic t1.Stop() call
                Thread.Sleep(10);
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
}
