namespace Imms.Tests.Performance
    open ExtraFunctional
    open Imms.FSharp.Implementation
    ///A fully bound test and additional metadata. 
    ///It is erased of all type information and parameters.
    open System.Reflection
    open System
    [<StructuredFormatDisplayAttribute("{AsString}")>]
    type Time =
        | Unknown
        | TimedOut
        | Error of string
        | Time of float
        override x.ToString() = 
            match x with
            | TimedOut -> "Timed Out"
            | Error s -> sprintf "Error: %s" s
            | Time t -> sprintf "%.3f" t
            | Unknown -> sprintf "No result yet"
        member x.AsString = x.ToString()

    [<StructuralEqualityAttribute; StructuralComparisonAttribute>]
    type StrKind = 
    | Core
    | Sequential
    | SetLike
    | MapLike
        override x.ToString() = sprintf "%A" x

    ///Metadata for a generator
    type GeneratorMeta(Name : string, ElementType : Type) =
        inherit MetaContainer()
        [<IsMeta>]
        member val Name = Name with get, set
        [<IsMeta>]
        member val ElementType = ElementType with get, set

    type DataGenerator<'v>(Name : string, inner : 'v seq) =
        inherit GeneratorMeta(Name, typeof<'v>)
        member val Generate = inner
        member x.Map f = DataGenerator(Name, inner |> Seq.map f)

    type DataStructureMeta(Name : string, Count : int, Generator : GeneratorMeta, Kind : StrKind, Library : string) =
        inherit MetaContainer()
        [<IsMeta>]
        member val Name = Name with get,set
        [<IsMeta>]
        member val Count = Count with get, set
        [<IsMeta>]
        member val Generator = Generator with get, set
        [<property: IsMeta>]
        member val Kind = Kind with get, set
        [<IsMeta>]
        member val Library = Library with get,set

    type DataStructure<'t> private (Name : string, Kind : StrKind , Count : int, Generator : GeneratorMeta, Library : string, ctor : int -> 't)= 
        inherit DataStructureMeta(Name, Count, Generator, Kind, Library)
        let cached = lazy(ctor Count)
        member x.Object = cached.Value
        static member Create(name : string, kind : StrKind, count : int, library : string, generator : DataGenerator<'v>, ctor : 'v seq -> 't) =
            let ds = DataStructure(name, kind, count, generator, library,(fun count -> generator.Generate |> Seq.take count |> ctor))
            ds

    type TestMeta(Name : string, Group : string, Iters : int, DataSource : DataStructureMeta option) = 
        inherit MetaContainer()
        [<IsMeta>]
        member val Name = Name with get,set
        [<IsMeta>]
        member val Group = Group with get,set
        [<IsMeta>]
        member val Iters = Iters with get,set
        [<IsMeta>]
        member val DataSource = DataSource with get,set


    type TestInstanceMeta(Test : TestMeta, Target : DataStructureMeta) =
        inherit MetaContainer() 
        [<IsMeta>]
        member val Test = Test with get,set
        [<IsMeta>]
        member val Target = Target with get,set
        [<IsMeta>]
        member val Time = Unknown with get,set

    type ErasedTest(Test : TestMeta, Target : DataStructureMeta, test : unit -> unit) = 
        inherit TestInstanceMeta(Test, Target)
        member val RunTest = test

    ///An unbound test for a specific collection type.
    [<AbstractClass>]
    type Test<'s>(Name : string, Group : string, Iterations : int, ?DataSource : DataStructureMeta) = 
        inherit TestMeta(Name, Group, Iterations, DataSource)
        abstract Test : 's -> unit
        member x.Bind (target : DataStructure<'s>) = 
            let test = x.Test
            let targetObject = target.Object
            let delayed = fun () -> test targetObject
            let erased = ErasedTest(x, target, delayed)
            erased

    module Seqs = 
        let rnd = Random()
        module Strings = 
            let letters(min,max)= 
                let s = seq {while true do yield rnd.String(min, max)} |> Seq.cache
                DataGenerator("Strings of Letters by Length",s)
                    <+. Meta("Range", (min,max))

            let distinctLetters(min,max) =
                let s = seq {while true do yield Chars.alphanumeric |> rnd.String(min,max)} |> Seq.lazyDistinct |> Seq.cache
                DataGenerator("Unique Strings of Letters by Length",s)
                    <+. Meta("Range", (min,max))
        module Numbers = 
            let length(min,max) = 
                let min, max = pown 2 min, pown 2 max
                let s = seq {while true do yield rnd.Next(min, max)} |> Seq.cache
                DataGenerator("Integers by Length", s) <+. Meta("Range", (min,max))
            let distinctLength(min,max) = 
                let min, max = pown 2 min, pown 2 max
                let s = seq {while true do yield rnd.Next(min, max)} |> Seq.lazyDistinct |> Seq.cache
                DataGenerator("Unique Integers By Length", s) <+. Meta("Range", (min,max))
            let float(min,max) = 
                let s = seq {while true do yield rnd.Double(min,max)} |> Seq.cache
                DataGenerator("Floating Point by Length", s) <+. Meta("Range", (min,max))

namespace Imms.Tests.Performance.Data
    open Imms.Tests.Performance
    module internal Basic = 
        let Array(count, generator)  = DataStructure.Create("Array", Core, count,"System", generator, Seq.toArray)
        let List(count, generator)   = DataStructure.Create("FSharpList", Core, count, "FSharp", generator, Seq.toList)
        let ArraySeq(count, generator) = DataStructure.Create("Array+Sequence", Core, count, "Core", generator, Seq.toList >> Seq.map (fun x -> x))
