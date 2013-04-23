namespace Benchmarks
open System
open System.Collections.Generic
open Solid




//Here we define targets of benchmarks.
type ITarget<'s> = 
    abstract Name : string
    abstract Object : 's
    abstract Metadata : Meta list

[<ReferenceEqualityAttribute; NoComparisonAttribute>]
type TargetWithSize<'s> = 
    {
        Name : string
        Ctor : TargetWithSize<'s> -> 's
        Metadata : Meta list
        Size : int
    }
    interface ITarget<'s> with
        member x.Name = x.Name
        member x.Object = Cache.MemoizeNoArgs x "Object" <| fun () -> x.Ctor x
        member x.Metadata = Initial_size(x.Size)::x.Metadata


//This erased test is what gets executed and logged by the test bench.
type IErasedTest = 
    abstract Test : (unit -> unit)
    abstract Tag : Tag

//This is an unbound test (meaning, a test without a specific target).

type ITest<'s> = 
    abstract Test : ('s -> unit)
    abstract Metadata : Meta list  
    abstract Name : string
    abstract Kind : string
//This is a more specialized type of test that includes iterations.
[<ReferenceEqualityAttribute; NoComparisonAttribute>]
type TestWithIterations<'s> = 
    {
        Test : TestWithIterations<'s> -> ('s -> unit)
        Metadata : Meta list
        Name : string
        Iterations : int
    }
    interface ITest<'s> with
        member x.Test = x.Test x
        member x.Metadata = Meta.Iterations(x.Iterations)::x.Metadata
        member x.Name = x.Name
        member x.Kind = "Test with iterations."
[<ReferenceEqualityAttribute; NoComparisonAttribute>]
type TestWithData<'s, 'a> = 
    {
        Test : TestWithData<'s, 'a> -> ('s -> unit)
        Metadata : Meta list
        Name : string
        Data : DataSource
        DataLoaded : 'a seq
        Iterations : int
    }
    interface ITest<'s> with
        member x.Test = x.Test x
        member x.Metadata = Meta.DataSource(x.Data_source)::Meta.Iterations(x.Iterations)::Meta.Data_size(x.Data_size)::x.Metadata
        member x.Name = x.Name
        member x.Kind = "Test with data source and iterations" 
    member x.Data_size = x.Data.Size
    member x.Data_source = x.Data.ToString()

[<AutoOpen>]
module Ext = 
    type ITest<'s> with
        member me.Bind (target : ITarget<'s>) = 
            let test = me.Test
            let targ = target.Object
            let delayed = fun () -> test targ
            {new IErasedTest  with
                member x.Test = delayed
                member x.Tag = {Target = target.Name; Test = me.Name; Kind = me.Kind; Metadata = MetaList(me.Metadata @ target.Metadata)}
            }
                
            
            
 


