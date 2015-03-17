namespace Funq.Tests.Performance
open Funq.FSharp.Implementation
///A fully bound test and additional metadata. 
///It is erased of all type information and parameters.

type DataGenerator<'v>(name : string, inner : 'v seq, ?metadata : Meta seq) as x=
    inherit MetaContainer(metadata.Or [])
    do x?Name <- name
    do x?ElementType <- typeof<'v>.Name
    member val Generate = inner

type DataStructure<'t> private (name : string, count : int, ctor : int -> 't, ?metadata : Meta seq)= 
    inherit MetaContainer([Meta("Name", name); Meta("Count", count)] |> Seq.append (metadata.Or (Seq.empty)))
    let cached = lazy(ctor count)
    member x.Object = cached.Value
    member x.Count = count
    static member Create(name : string, count : int, generator : DataGenerator<'v>, ctor : 'v seq -> 't, ?metadata : Meta seq) =
        let ds = DataStructure(name, count, (fun count -> generator.Generate |> Seq.take count |> ctor))
        ds?Generator <- generator.Clone
        ds

type ErasedTest(test : unit -> unit, ?metadata : Meta list) = 
    inherit MetaContainer(metadata.Or [])
    member val Test = test

type TestResult() = 
    inherit MetaContainer()
///An unbound test for a specific collection type.

[<AbstractClass>]
type Test<'s>(name : string, ?metadata : Meta seq) as x= 
    inherit MetaContainer(metadata.Or [])
    do 
        x?Name <- name
    abstract Test : 's -> unit
    member x.Bind (target : DataStructure<'s>) = 
        let test = x.Test
        let targetObject = target.Object
        let delayed = fun () -> test targetObject
        let erased = ErasedTest(delayed)
        erased?Test <- x.Clone
        erased?Target <- target.Clone
        erased

type TestGroup<'s>(tests : Test<'s> seq, ?metadata : Meta list) = 
    inherit MetaContainer(metadata.Or [])
    member val Tests = tests


