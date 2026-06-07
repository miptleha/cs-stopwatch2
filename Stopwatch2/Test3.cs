using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Misc;

internal partial class Tests
{
    public static void Test3(bool simpleMode)
    {
        Console.WriteLine("--- Simultaneous execution " + (simpleMode ? "(Simple mode) " : "") + "---");

        var sw = Stopwatch.StartNew();
        var sw2 = Stopwatch2.StartNew(simpleMode);

        var tasks = new List<Task>();
        for (int i = 0; i < ITERATIONS_NUMBER; i++)
        {
            tasks.Add(
                Task.Run(() =>
                {
                    using (var t = sw2.Start("f1"))
                        f1();

                    using (var t = sw2.Start("f2"))
                        f2(t);
                })
            );
        }
        Task.WaitAll(tasks.ToArray());
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
