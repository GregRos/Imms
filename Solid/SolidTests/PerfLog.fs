module SolidTesting.Tests
open System.Diagnostics
open System
open System.Threading
open SolidTesting.Generators
open Solid.Ops
open SolidTesting.Targets
let inline dropl x = (^s : (member DropLast : unit -> 's) x)
let inline dropf x = (^s : (member DropFirst : unit -> 's) x)
let rnd = Random()

let flip_coin ()= rnd.Next(0,2) = 0

let test_addl_many iter (o : TestTarget<_>) = 
    let mutable o = o
    for i in 0 .. iter do
        o <- o <+ i
    o

let test_addf_many iter (o : TestTarget<_>) = 
    let mutable o = o
    for i in 0 .. iter do
        o <- i +> o
    o

let test_addl_bulk content (o : TestTarget<_>)  = 
    let mutable o =o
    o <- o <++ content
    o

let test_addf_bulk iter (o : TestTarget<_>) content = 
    let mutable o=o
    for i in 0 .. iter do
        o <- content ++> o
    o

let test_add_mixed iter (o : TestTarget<_>) = 
    let mutable o =o
    for i in 0 .. iter do
        if flip_coin() then
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

let test_iter_length iter (o : TestTarget<_>) = 
    let mutable o=o
    for i = 0 to iter do
        o.Length |> ignore
    o

let test_iter_first iter (o : TestTarget<_>) = 
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
    for i = 0 to o.Length() do
        o <- o.Set i i
    o

let test_set_rnd iter (o : TestTarget<_>) = 
    let mutable o=o
    let len = o.Length()
    for i = 0 to iter do
        let rndIndex = rnd.Next(0,len)
        o<-o.Set rndIndex i
    o

let test_random_access iter (o : TestTarget<_>)= 
    let len = o.Length()
    if len = 0 then
        o
    else
        let mutable o=o
        for i = 0 to iter do
            let rndIndex = rnd.Next(0,len)
            if flip_coin() then
                o.Get rndIndex |> ignore
            else
                o <- o.Set rndIndex i
        o

let test_insert_ascending iter (o : TestTarget<_>)=  
    let len = o.Length()
    let mutable o = o
    for i = 0 to (o.Length() - 2) do
        o <- o.InsertAt i i

[<AbstractClassAttribute>]
type Test(name,parameter) = 
    member __.Name = name
    member __.Parameter = parameter
    abstract member Test : TestTarget<'a> -> unit
    
