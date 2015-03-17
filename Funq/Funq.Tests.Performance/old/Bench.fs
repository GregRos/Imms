namespace Funq.Benchmarking

open System.Diagnostics
open System
open System.Threading
open System.IO
type MutableList<'t> = System.Collections.Generic.List<'t>

///The module that executes benchmarks.
module Bench = 
    type Timed_out = 
        | Timed_out
        interface Meta with
            member x.Value = null
            member x.Name = "Timed Out"
    
    let Results = MutableList<Entry>()
    let mutable Watch = Stopwatch()
    let mutable Timer : Timer = null
    let mutable Drops = 5
    let mutable Runs = 10
    let mutable Timeout = 10000
    let mutable RunningThread : Thread = null
    let inline (./) num1 num2 = num1 / (float num2)
    
    let mutable OnRun = 
        fun () -> 
            GC.Collect()
            GC.WaitForPendingFinalizers()
    
    let invoke (writer : TextWriter) (test : IErasedTest)  = 
        let run = test.Test
        let tag = test.Entry
        fprintfn writer "Beginning Test '%s' with metadata:" tag.Test
        fprintfn writer "\t%A" tag.``Test Metadata``
        fprintfn writer "Target: '%s' with metadata:" tag.Target
        fprintfn writer "\t%A" tag.``Target Metadata``
        let thread = 
            new Thread(fun () -> 
            
            for i = 0 to Drops do
                run()
            Watch.Reset()
            OnRun()
            Watch.Start()
            for i = 0 to Runs do
                run()
            Watch.Stop()
            tag.Time <- Watch.Elapsed.TotalMilliseconds / (float Runs))
        thread.Start()
        let finished = thread.Join(Timeout)
        fprintfn writer "\tEnding Test: '%s' for '%s' with time, '%A'\n" tag.Test 
            tag.Target tag.Time
        if not finished then 
            tag.Time <- System.Double.NaN
            thread.Abort()
            fprintfn writer "\t\tThe test timed out."
        tag
