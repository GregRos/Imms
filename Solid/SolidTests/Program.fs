5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39
40
41
42
43
44
45
46
47
48
49
50
51
52
53
54
55
56
57
58
59
60
61
62
63
64
65
66
67
68
69
70
71
72
73
74
75
76
77
78
79
80
81
82
83
84
85
86
87
88
89
90
91
92
93
94
95
96
97
98
99
100
101
102
103
104
105
106
107
108
109
110
111
112
113
114
115
116
117
118
119
120
121
122
123
124
125
126
127
128
129
130
131
132
133
134
135
136
137
138
139
140
141
142
143



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



type TestResult = {Name : string; Result : TestOutcome}
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


let perf_testing() =
    let lst = MutableList<int>()
    let lst2 = MutableList<int>()
    let sw = Stopwatch()
    let iterations = 10**5
    System.Console.BufferHeight <- Console.BufferHeight * 3
    let all_tests = [Test_insert_ascending iterations;
        Test_add_first iterations;
        Test_add_last iterations;
        Test_set_rnd iterations;
        Test_remove_rnd iterations;
        Test_set_each ;
        //Test_iter_take_first iterations;
        Test_get_rnd <|10**5;
        Test_get_each ;
        Test_concat_self iterations]
    let results = run_test_sequence all_tests (delay1 all_test_targets iterations)
    results |> Seq.iter print

let then_verify (x : TestTarget<_>) =
    if x.Verify() |> not then
        failwith "Error"
    else
        x

let unit_consistency_testing() =
    let mutable group = TestGroup([| solid_xlist 0; core_list 0|]) :> TestTarget<_>
    let mutable group2 = TestGroup([| core_list 0; solid_vector 0 |]) :> TestTarget<_>
    let iters = 10 ** 4
    group <- group |> test_addl_many iters |> then_verify
    group <- group |> test_addf_many iters |> then_verify
    group <- group |> test_add_mixed iters 0.5 |> then_verify
    group <- group |> test_insert_ascending iters |> then_verify
    group <- group |> test_set_rnd iters |> then_verify
    group |> test_get_each |> ignore
    group <- group |> test_set_each |> then_verify
    group <- group |> test_drop_all_mixed |> then_verify
    group <- group |> test_addl_many iters |> then_verify
    group <- group |> test_addf_many iters |> then_verify
    group |> test_dropl_num_then_verify 100 |> ignore
    group2 <- group2 |> test_addl_many iters |> then_verify
    group |> test_dropf_num_then_verify 100 |> ignore
    group2 <- group2 |> test_iterate_take_verify 100
    group2 <- group2 |> test_set_rnd iters |> then_verify
    group2 <- group2 |> test_get_rnd iters
    group2 <- group2 |> test_set_each |> then_verify
    group2 <- group2 |> test_dropl_num_then_verify 100
    group2 <- group2 |> test_addl_many 10000
    group2 <- group2 |> test_dropl_all |> then_verify
    
    test_iterate_slices 10 group
   


[<EntryPoint>]
let main argv =
    let mutable group = TestGroup([| solid_xlist 0; core_list 0 |]) :> TestTarget<_>
    perf_testing()


    if group.Verify() |> not then
        failwith "Nope"
    printfn "Done"
    Console.Read()
    0