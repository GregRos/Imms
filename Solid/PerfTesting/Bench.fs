namespace Benchmarks

open System.Diagnostics
open System
open System.Threading
type MutableList<'t> = System.Collections.Generic.List<'t>
//This is the test bench that runs the tests.
module Bench = 
    let Results = MutableList<Tag>()
    let mutable Watch = Stopwatch()
    let mutable Timer : Timer = null
    let mutable Drops = 5
    let mutable Runs = 10
    let mutable Timeout = 10000
    let mutable RunningThread : Thread = null
    let inline ( ./) num1 num2 = num1 / (float num2)
    let mutable OnRun = 
        fun () ->
            GC.Collect()
            GC.WaitForPendingFinalizers()

    let Invoke (test : IErasedTest) = 
        
        let run = test.Test
        let tag = test.Tag
        printfn "Beginning Test '%s' for '%s'. \n Meta: %A\n" tag.Test tag.Target tag.Metadata
        run()
        for i = 0 to Drops do run()
        Watch.Reset()
        OnRun()
        let thread =
         new Thread(fun () -> 
            for i = 0 to Runs do
                run())
        Watch.Start()
        thread.Start()
        let finished = thread.Join(Timeout)
        Watch.Stop()
        tag.Time <- Watch.Elapsed.TotalMilliseconds / (float Runs)
        printfn "\tEnding Test: '%s' for '%s' with time, '%A'\n" tag.Test tag.Target tag.Time
        if not finished then printfn "\t\tThe test timed out."
        if finished then tag else {tag with Tag.Metadata = tag.Metadata.On(fun t -> Timed_out::t)}







    