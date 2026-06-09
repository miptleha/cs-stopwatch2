using Misc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

internal partial class Tests
{
    public static void Test7(bool simpleMode)
    {
        Console.WriteLine("--- Custom output " + (simpleMode ? "(Simple mode) " : "") + "---");

        var sw = Stopwatch.StartNew();
        var sw2 = Stopwatch2.StartNew(simpleMode);
        for (int i = 0; i < ITERATIONS_NUMBER; i++)
        {
            using (var t1 = sw2.Start("Task 1"))
            {
                using (var t2 = t1.Start("Task 1.1"))
                {
                    using (var t3 = t2.Start("Task 1.1.1"))
                    {
                        Thread.Sleep(10);
                    }
                }
                var t4 = t1.Start("Task 1.2");
                using (var t5 = t4.Start("Task 1.2.1"))
                {
                    Thread.Sleep(20);
                }
                t4.Stop();
            }
        }
        sw2.Stop();
        sw.Stop();
        
        Console.WriteLine("# Default output");
        Console.WriteLine(sw2.Results());
        Console.WriteLine();

        Console.WriteLine("# Hide root");
        Console.WriteLine(sw2.Results(new Stopwatch2Options { HideRoot = true }));
        Console.WriteLine();

        Console.WriteLine("# Hide percent");
        Console.WriteLine(sw2.Results(new Stopwatch2Options { HidePercent = true }));
        Console.WriteLine();

        Console.WriteLine("# Hide time");
        Console.WriteLine(sw2.Results(new Stopwatch2Options { HideTime = true }));
        Console.WriteLine();

        Console.WriteLine("# Hide count");
        Console.WriteLine(sw2.Results(new Stopwatch2Options { HideCount = true }));
        Console.WriteLine();

        Console.WriteLine("# Execution time in miliseconds");
        Console.WriteLine(sw2.Results(new Stopwatch2Options { MsMode = true }));
        Console.WriteLine();

        Console.WriteLine("# Minimal output");
        Console.WriteLine(sw2.Results(new Stopwatch2Options 
        { 
            HideRoot = true, 
            HidePercent = true, 
            HideTime = true, 
            HideCount = true, 
            MsMode = true 
        }));
        Console.WriteLine();


        Console.WriteLine("Stopwatch: " + sw.Elapsed);
        Console.WriteLine();
    }
}
