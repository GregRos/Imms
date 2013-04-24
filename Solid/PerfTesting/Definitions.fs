namespace Benchmarks
open System
open System.Collections.Generic
open Solid

//Here we define targets of benchmarks.
type ITarget<'s> = 
    abstract Name : string
    abstract Object : 's
    abstract Metadata : Meta list

//A target annotated with a size.
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


///A fully bound test and additional metadata. 
///It is erased of all type information and parameters.
type IErasedTest = 
    abstract Test : (unit -> unit)
    abstract Tag : Tag

///An unbound test for a specific collection type.
type ITest<'s> = 
    abstract Test : ('s -> unit)
    abstract Metadata : Meta list  
    abstract Name : string
    abstract Kind : string
///A test for a specific collection type. Annotated with a number of iterations.
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

///A test for a specific collection type. Annotated with iterations and a data source.
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
        ///Binds a test for a specific collection type to a concrete instance of the collection.
        ///Results in a test annotated with logging information, but erased of all type information.
        member me.Bind (target : ITarget<'s>) = 
            let test = me.Test
            let targ = target.Object
            let delayed = fun () -> test targ
            {new IErasedTest  with
                member x.Test = delayed
                member x.Tag = {Target = target.Name; Test = me.Name; Kind = me.Kind; Metadata = MetaList(me.Metadata @ target.Metadata)}
            }
                
            
            
 


