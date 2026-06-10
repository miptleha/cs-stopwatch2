## Stopwatch2

A stopwatch for measuring the speed of multiple tasks.  
Cumulative, Hierarchical, Thread-Safe, Configurable.  
Open the console application to see usage examples (`VS 2026`, `VS 2022`, `VS Code`).  
To use, copy [Stopwatch2.cs](Stopwatch2/Stopwatch2.cs) into your project (it compiles even in earlier versions of `.NET`).

Usage [example](Stopwatch2/Test0.cs):  
```csharp
var sw = Stopwatch2.StartNew();
var t = sw.Start("Task");
Thread.Sleep(100);
t.Stop();
sw.Stop();
Console.WriteLine(sw.Results());
```

Execution result:  
```cmd
Total time: 00:00:00.1050561 100% 1
 Task: 00:00:00.1040372 99% 1
```

Main differences from the standard [Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0) class:
- Allows [adding tasks](Stopwatch2/Test1.cs) and displaying the total time and execution time of each task at the end
- Tasks can be [nested](Stopwatch2/Test2.cs), in which case the output will be hierarchical
- Complete time calculation in a [multithreaded](Stopwatch2/Test3.cs) application, even if tasks are executed simultaneously
- Shows call tree for [recursive](Stopwatch2/Test4.cs) functions
- A [stub](Stopwatch2/Test5.cs) is included to prevent Stopwatch2 code from being executed
- Simple mode in which Stopwatch2 turns into a standard Stopwatch
- [Fast](Stopwatch2/Test6.cs) execution even with millions of tasks
- The [output](Stopwatch2/Test7.cs) of results can be customized

[![hits](https://myhits.vercel.app/api/hit/https%3A%2F%2Fgithub.com%2Fmiptleha%2Fcs-stopwatch2?color=blue&label=hits&size=small)](https://myhits.vercel.app)
