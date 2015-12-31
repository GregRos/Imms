module Funq.Tests.Performance.Bench
open Funq.Tests.Performance
open System.Diagnostics
open System
open System.Threading
open System.IO
open Funq.Tests
open System.CodeDom.Compiler
open Funq.FSharp.Implementation
open ExtraFunctional
type MutableList<'t> = System.Collections.Generic.List<'t>

let Results = MutableList<MetaContainer>()
let mutable Watch = Stopwatch()
let mutable Timer : Timer = null
let mutable Removes = 5
let mutable Runs = 20
let mutable Timeout = 20000
let mutable RunningThread : Thread = null
let inline (./) num1 num2 = num1 / (float num2)
let mutable OnRun = 
    fun () -> 
        GC.Collect()
        GC.WaitForPendingFinalizers()
    
let invoke (test : unit -> unit)  = 
    let runner() = 
        for i = 0 to Removes do
            test()
        Watch.Reset()
        OnRun()
        Watch.Start()
        for i = 0 to Runs do
            test()
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
        let time = Watch.Elapsed.TotalMilliseconds / (float Runs)
        let time = if finished then Time(time) else TimedOut
        time
    with
        | ex -> 
            Error ex.Message

