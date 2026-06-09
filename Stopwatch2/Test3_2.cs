using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Misc;

internal partial class Tests
{
    public static void Test3_2()
    {
        Console.WriteLine("--- Simulation simultaneous execution  ---");

        var sw = Stopwatch.StartNew();
        var sw2 = Stopwatch2.StartNew();

        var list1 = new List<Stopwatch2Task>();
        var list2 = new List<Stopwatch2Task>();
        var list3 = new List<Stopwatch2Task>();
        for (int i = 0; i < ITERATIONS_NUMBER; i++)
        {
            var t1 = sw2.Start("Task1");
            for (int j = 0; j < ITERATIONS_NUMBER; j++)
            {
                var t2 = t1.Start("Task2");
                list2.Add(t2);
                for (int k = 0; k < ITERATIONS_NUMBER; k++)
                {
                    var t3 = t2.Start("Task3");
                    list3.Add(t3);
                    //t3.Stop();
                }
            }

                list1.Add(t1);
        }
        Thread.Sleep(1000);

        foreach (var t in list3)
            t.Stop();
        foreach (var t in list2)
            t.Stop();
        foreach (var t in list1)
            t.Stop();

        sw2.Stop();
        Console.WriteLine(sw2.Results());

        Console.WriteLine("Stopwatch: " + sw.Elapsed);
        Console.WriteLine();
    }
}
