// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.


open System.Diagnostics

open System


open System.Collections.Generic
open System.Threading
let inline ( ** ) a b = pown a b



type MutableList<'a> = System.Collections.Generic.List<'a>
let warmup = 2
let sw = Stopwatch()
let time = ref (TimeSpan())
let invoke_test action o = 
    let thread = Thread(fun () ->
            for __ = 0 to warmup do
                action o
            GC.Collect()
            sw.Restart()
            action o |> ignore
            sw.Stop()
            time := sw.Elapsed)
    thread.Priority <- ThreadPriority.Highest
    thread.Start()
    thread.Join()
    time.Value

type TestResults = 
    {
        Name : string
        Time : TimeSpan
    }

let run_test_on action (objs : seq<TestTarget<int>>) = 
    let results = MutableList()
    for target in objs do
        let time = target |> invoke_test action
        results.Add({Name = target.Name; Time = time})

    results


open SolidTesting.Tests

open SolidTesting.Targets

let print x = x |> Seq.iter (fun t -> printfn "%A" t)
[<EntryPoint>]
let main argv = 
    let lst = MutableList<int>()
    let lst2 = MutableList<int>()
    let sw = Stopwatch()
  

    let all = 
        [
            fsharpx_vector 100000;
            solid_vector 100000; 
            solid_xlist 100000;
            ]


    let results = run_test_on (test_get_each) all
    
    print results
    //let results = run_test_on (test_addl_many 1000000 >> ignore) all
    print results
    Console.Read()
    0
