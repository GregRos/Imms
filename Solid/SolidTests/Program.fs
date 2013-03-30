// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.


open System.Diagnostics

open System

open SolidTests.Targets
open SolidTests.Tests
open System.Collections.Generic
open System.Threading
open SolidFS
let inline ( ** ) a b = pown a b



type MutableList<'a> = System.Collections.Generic.List<'a>
let warmup = 10
let sw = Stopwatch()
let mutable time = 0.
let mutable maybe_failed = false
[<NoEqualityAttribute(); NoComparisonAttribute()>]
type TestOutcome = 
    | Failed
    | Succeeded of float

let invoke_test_simple action = 
    maybe_failed <- false
    
    let thread() = 
        let thread = Thread(fun () ->
            //try
            for __ = 0 to warmup do
                action()
            GC.Collect()
            GC.WaitForPendingFinalizers()
            sw.Restart()
            action() |> ignore
            sw.Stop()
            time <- time + sw.Elapsed.TotalMilliseconds
            //with //empirically this try-with block does not affect performance.
            //| :? TestNotAvailableException<int> as x -> maybe_failed <- true
            )
        thread.Priority <- ThreadPriority.Highest
        thread.Start()
        thread.Join()
        
        
    for __ = 0 to 10 do
        thread()
    let t = time
    time <- 0.
    if maybe_failed then Failed else Succeeded(t)


   

open FSharpx.Collections
[<EntryPoint>]
let main argv = 
    let deq = FSharpx.Collections.Deque.empty
    let len = Math.Pow(10., 5.) |> int
    let solid_vector = SolidFS.Operators.Vector.ofSeq [|0|]
    let fsharpx_vector = FSharpx.Collections.Vector.ofSeq  [0]

    let  xl = XList.ofSeq [0 .. len]
    let mutable arr = {0 .. len}
    let mutable iter = 1
    let mutable time1 = 0L
    let mutable time2 = 0L
    let n = 5
    let rnd = Random()
    for __ = 0 to n do
        GC.Collect()
        GC.WaitForPendingFinalizers()
        for i = 0 to iter do
            let x = FSharpx.Collections.Vector.ofSeq arr
            ()
        sw.Restart()
        
        for i = 0 to iter do
            let x = FSharpx.Collections.Vector.ofSeq arr
            ()
        sw.Stop()
        time1 <- time1 + sw.ElapsedTicks
    printfn "%A" (time1 )
   
    for __ = 0 to n do

        GC.Collect()
        GC.WaitForPendingFinalizers()
        for i = 0 to iter do
            let x = SolidFS.Operators.Vector.ofSeq arr
            ()  
        sw.Restart()
        for i = 0 to iter do
            let x = SolidFS.Operators.Vector.ofSeq arr
            ()  
        //test_deq.Tail
  
        sw.Stop()
        time2 <- time2 + sw.ElapsedTicks
    printfn "%A" time2
    printfn "%A" ((float time1) / (float time2) )
    0
    