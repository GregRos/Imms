module SolidTests.Tests
open System.Diagnostics
open System
open System.Threading
open SolidTests.Generators
open SolidFS.Operators
open SolidTests.Targets
let inline dropl x = (^s : (member DropLast : unit -> 's) x)
let inline dropf x = (^s : (member DropFirst : unit -> 's) x)
let rnd = Random()



let flip_coin (chance_heads) = 
    if rnd.NextDouble() > chance_heads then
        true
    else
        false


type TestType = 
    | Single = 1
    | Multi = 2
    | Complex = 3
type Test = 
    {
        Name : string
        Parameter : obj
        Test : TestTarget<int> -> TestTarget<int>
    }



let inline test_addl_many  iter (o : ^s)  = 
    let mutable o = o
    for i in 0 .. iter do
        o <- o <+ i
    o

let test_concat_self iter (o : TestTarget<_>) = 
    let mutable r = o
    for i = 0 to iter do
        r <- o <+> o
    o

let test_addf_many  iter (o : TestTarget<_>) = 
    let mutable o = o
    for i in 0 .. iter do
        o <- i +> o
    o

let test_add_mixed  iter ratio (o : TestTarget<_>) = 
    let mutable o =o
    for i in 0 .. iter do
        if flip_coin(ratio) then
            o <- o <+ i
        else
            o <- i +> o
    o

let test_dropl_all (o : TestTarget<_>) = 
    let mutable o = o
    while not o.IsEmpty do
        o <- o |> dropl
    o

let test_dropf_all (o : TestTarget<_>) = 
    let mutable o=o
    while not o.IsEmpty do
        o <- o |> dropf
    o

let test_drop_all_mixed (o : TestTarget<_>) = 
    let mutable o=o
    while not o.IsEmpty do
        if rnd.Next(0,2) = 1 then
            o <- o |> dropl
        else
            o <- o |> dropf
    o

let test_mixed_add_drop iter (o : TestTarget<_>) = 
    let mutable o=o
    for i = 0 to iter do
        let n = rnd.Next(0,4)
        match n with
        | 0 -> o <- o <+ i
        | 1 -> o <- i +> o
        | 2 -> o <- o |> dropl
        | 3 -> o <- o |> dropf
    o

let test_add_mixed_then_drop_all_mixed iter (o : TestTarget<_>) = 
    let mutable o =o
    for i = 0 to iter do
        if flip_coin(0.5) then
            o <- o <+ i
        else
            o <- i +> o
    
    while o |> isntEmpty do
        if flip_coin(0.5) then
            o <- o |> dropl
        else
            o <- o |> dropf
    o

let test_iter_length iter (o : TestTarget<_>) = 
    let mutable o=o
    for i = 0 to iter do
        o.Length |> ignore
    o

let     test_iter_first iter (o : TestTarget<_>) = 
    let mutable o=o
    for i = 0 to iter do
        o.First |> ignore
    o

let test_iter_last iter (o : TestTarget<_>) = 
    let mutable o=o
    for i = 0 to iter do
        o.Last |> ignore
    o

let test_get_each (o : TestTarget<_>) = 
    for i = 0 to o.Length()-1 do
        o.Get i |> ignore
    o

let test_get_rnd iter (o : TestTarget<_>) =
    let len = o.Length()
    if len = 0 then
        o
    else
        for i = 0 to iter do
            let rndIndex = rnd.Next(0,len - 1)
            o.Get rndIndex |> ignore
        o

let test_set_each (o : TestTarget<_>)  = 
    let mutable o=o
    for i = 0 to o.Length()-1 do
        o <- o.Set i i
    o

let test_iter_take_last iter (o : TestTarget<_>) = 
    let mutable r= o
    let len = o.Length()
    for i = 0 to o.Length() - 1 do
        let rndI = rnd.Next(1,len)
        r <- o.TakeLast(rndI)
    r

let test_set_rnd iter (o : TestTarget<_>) = 
    let mutable o=o
    let len = o.Length()
    for i = 0 to iter do
        let rndIndex = rnd.Next(0,len-3)
        o<-o.Set rndIndex i
    o

let test_random_access iter ratio (o : TestTarget<_>)= 
    let len = o.Length()
    if len = 0 then
        o
    else
        let mutable o=o
        for i = 0 to iter do
            let rndIndex = rnd.Next(0,len)
            if flip_coin(ratio) then
                o.Get rndIndex |> ignore
            else
                o <- o.Set rndIndex i
        o

let test_iterate_take iter (o : TestTarget<_>) = 
    let mutable r=o
    
    let len = o.Length()
    for i = 0 to iter do
        let rndI = rnd.Next(1,len)
        r <- o.TakeFirst(rndI)
    r
let test_insert_ascending iter (o : TestTarget<_>)=  
    let len = o.Length()
    let mutable o = o
    for i = 0 to (o.Length() - 2) do
        o <- o.InsertAt i i
    o
let Test_add_last iter = 
    {
        Name = "Iterate AddLast"
        Parameter = iter
        Test = test_addl_many iter
    }

let Test_add_first iter = 
    {
        Name = "Iterate AddFirst"
        Parameter = iter
        Test = test_addf_many iter
    }

let Test_add_mixed iter ratio = 
    {
        Name = "Iterate Mixed AddFirst/AddLast"
        Parameter = iter
        Test = test_add_mixed iter 0.5
    }

let Test_drop_last = 
    {
        Name = "DropLast All"
        Parameter = ()
        Test = test_dropl_all
    }

let Test_drop_first = 
    {
        Name = "DropFirst All"
        Parameter = ()
        Test = test_dropf_all
    }

let Test_add_mixed_then_drop_all_mixed iter = 
    {
        Name = "Add Mixed Then Drop All, Mixed"
        Parameter = iter
        Test = test_add_mixed_then_drop_all_mixed iter
    }

let Test_get_rnd iter= 
    {
        Name = "Iterate Get Random"
        Parameter = iter
        Test = test_get_rnd iter
    }

let Test_get_each = 
    {
        Name = "Get Each Index"
        Parameter = ()
        Test = test_get_each
    }

let Test_insert_ascending iter =
    {
        Name = "Insert Ascending Iterate"
        Parameter = iter
        Test = test_insert_ascending iter
    }

let Test_set_rnd iter = 
    {
        Name = "Iterate Set Random"
        Parameter = iter
        Test = test_set_rnd iter
    }

let Test_set_each = 
    {
        Name = "Set Every Index"
        Parameter = ()
        Test = test_set_each
    }

let Test_random_access iter= 
    {
        Name = "Iterate Get/Set Random"
        Parameter = iter
        Test = test_random_access iter 0.5
    }

let Test_concat_self iter = 
    {
        Name = "Concat Self Iterate"
        Parameter = iter
        Test = test_concat_self iter
    }

let Test_iter_first iter= 
    {
        Name = "Iterate First"
        Parameter = iter
        Test = test_iter_first iter
    }

let Test_iter_last iter= 
    {
        Name = "Iterate First"
        Parameter = iter
        Test = test_iter_last iter
    }

let Test_mixed_add_drop iter = 
    {
        Name = "Iterate Mixed Add/Drop"
        Parameter = iter
        Test = test_mixed_add_drop iter
    }

let Test_iter_length iter = 
    {
        Name = "Iterate Length"
        Parameter = iter
        Test = test_iter_length iter
    }

let Test_iter_take_first iter = 
    {
        Name = "Iterate TakeFirst rnd"
        Parameter = iter
        Test = test_iterate_take iter
    }

let Test_iter_take_last iter =  
    {
        Name = "Iterate TakeLast rnd"
        Parameter = iter
        Test = test_iter_take_last iter

    }