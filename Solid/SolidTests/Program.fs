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
let warmup = 2
let sw = Stopwatch()
let mutable time = TimeSpan()
let mutable maybe_failed = false

type TestOutcome = 
    | Failed
    | Succeeded of float

let invoke_test action o = 
    maybe_failed <- false
    
    let thread = Thread(fun () ->
            try
            for __ = 0 to warmup do
                action o
            GC.Collect()
            sw.Restart()
            action o |> ignore
            sw.Stop()
            time <- sw.Elapsed
            with //empirically this try-with block does not affect performance.
            | :? TestNotAvailableException<int> as x -> maybe_failed <- true
            )
    thread.Priority <- ThreadPriority.Highest
    
    thread.Start()
    thread.Join()

    if maybe_failed then Failed else Succeeded(time.TotalMilliseconds)



type TestResult =  {Name : string; Result : TestOutcome}
type TestExecution = {Name : string; Results : MutableList<TestResult>}
let run_test_on (action : Test) (objs : list<TestTarget<int>>) = 
    let results = MutableList()
    for target in objs do
        //printfn "Beginning test %s" target.Name
        let ret = target |> invoke_test (action.Test >> ignore)
        results.Add({Name = target.Name; Result=ret})



    {Name = action.Name; Results = results}

let run_test_sequence (the_sequence : Test list) get_targets = 
    seq {
        for test in the_sequence do
            let current_result = run_test_on test (get_targets())
            yield current_result
        }

open SolidTests.Tests

open SolidTests.Targets

let print {Name = name; Results = results} = printfn "%s" name; results |> Seq.iter (fun t -> printfn "%A" t)
[<EntryPoint>]
let main argv = 
    let lst = MutableList<int>()
    let lst2 = MutableList<int>()
    let sw = Stopwatch()
    let iterations = 10**5
    System.Console.BufferHeight <- Console.BufferHeight * 3
    let all_tests = 
        [Test_insert_ascending iterations; 
        Test_add_first iterations; 
        Test_add_last iterations;
        Test_drop_first;
        Test_drop_last;
        Test_get_rnd iterations;
        Test_get_each;
        Test_set_each;
        Test_set_rnd iterations;
        Test_random_access iterations;
        Test_add_mixed iterations 0.5;
        Test_insert_ascending iterations;
        Test_iter_take_first iterations;
        Test_concat_self iterations;
        Test_iter_first iterations;
        Test_iter_last iterations;
        Test_iter_length iterations;
        Test_iter_take_last iterations]
    let results = run_test_sequence all_tests (delay1 all_test_targets (10**5))
    results |> Seq.iter print
    
    Console.Read()
    0
    