namespace Funq.Tests.Performance
open Funq.Tests.Performance
open System.Diagnostics
open System
open System.Threading
open System.IO
open Funq.Tests
open System.CodeDom.Compiler
open Funq.FSharp.Implementation
type MutableList<'t> = System.Collections.Generic.List<'t>

///The module that executes benchmarks.
module Bench = 
    let Results = MutableList<MetaContainer>()
    let mutable Watch = Stopwatch()
    let mutable Timer : Timer = null
    let mutable Drops = 5
    let mutable Runs = 20
    let mutable Timeout = 20000
    let mutable RunningThread : Thread = null
    let inline (./) num1 num2 = num1 / (float num2)
    let mutable OnRun = 
        fun () -> 
            GC.Collect()
            GC.WaitForPendingFinalizers()
    
    let invoke (writer : IndentedTextWriter) (test : ErasedTest)  = 
        let run = test.RunTest
        let tag = test :> TestInstanceMeta
        fprintfn writer "Beginning Test '%s' with metadata:" tag.Test.Name
        writer.Push()
        fprintfn writer "%O" tag.Test
        fprintfn writer "Target: '%s' with metadata:" tag.Target.Name
        fprintfn writer "%O" tag.Target
        let runner() = 
            for i = 0 to Drops do
                run()
            Watch.Reset()
            OnRun()
            Watch.Start()
            for i = 0 to Runs do
                run()
            Watch.Stop()

        let thread = Thread(ThreadStart(runner))
        try
            thread.Start()
            let finished = 
                if Timeout = -1 then
                    thread.Join()
                    true 
                else 
                    thread.Join(Timeout)
            let elapsed = Watch.Elapsed.TotalMilliseconds
            let time = Watch.Elapsed.TotalMilliseconds / (float Runs)
            tag.Time <- if finished then Time(time) else TimedOut
            fprintfn writer "Ending Test: '%s' for '%s' with time, '%A'\n" tag.Test.Name tag.Target.Name tag.Time
            writer.Pop()
            if not finished then thread.Abort()
            tag
        with
            | ex -> 
                fprintfn writer "Ending Test: '%s' for '%s' due to exception of type '%s'" tag?Test?Name tag?Target?Name (ex.GetType().ToString())
                tag.Time <- Error ex.Message
                tag
