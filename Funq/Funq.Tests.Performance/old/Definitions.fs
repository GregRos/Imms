namespace Funq.Benchmarking
open System
open System.Collections.Generic
open Funq


///A fully bound test and additional metadata. 
///It is erased of all type information and parameters.
type IErasedTest = 
    abstract Test : (unit -> unit)
    abstract Entry : Entry

///An unbound test for a specific collection type.
type ITest<'s> = 
    abstract Test : ('s -> unit)
    abstract Metadata : Meta list  
    abstract Name : string
    abstract Kind : TestKind
    abstract AddMeta : Meta -> ITest<'s>
///A test for a specific collection type. Annotated with a number of iterations.

type IDataGenerator<'v> = 
    inherit seq<'v>
    abstract Metadata : Meta list

type IDataStruct<'t> = 
    abstract Metadata : Meta list
    abstract Object   : 't
    abstract Count    : int
    abstract Name     : string

type Iterations = 
    | Iterations of int
    interface Meta with
        member x.Value = x.Iters:>_
        member x.Name = "Iterations"
    member x.Iters = match x with Iterations n -> n
    override x.ToString() = sprintf "Iterations: %A" (x.Iters + 1)
[<AutoOpen>]
module Ext = 
    type ITest<'s> with
        ///Binds a test for a specific collection type to a concrete instance of the collection.
        ///Results in a test annotated with logging information, but erased of all type information.
        member x.Bind (target : IDataStruct<'s>) = 
            let test = x.Test
            let targ = target.Object
            let delayed = fun () -> test targ
            {new IErasedTest  with
                member y.Test = delayed
                member y.Entry = 
                    {
                        Target = target.Name; 
                        Test = x.Name; 
                        ``Target Metadata`` = target.Metadata 
                        ``Test Metadata`` =  x.Metadata
                        Kind = x.Kind
                    }
            }
        member x.WithName (name : string) = 
            { new ITest<'s> with
                member y.Test = x.Test
                member y.Name = sprintf "%s %s" x.Name name
                member y.Kind = x.Kind
                member y.AddMeta m = x.AddMeta m
                member y.Metadata = x.Metadata
            }

                
            
            
 


