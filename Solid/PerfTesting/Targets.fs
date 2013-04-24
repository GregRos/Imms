namespace Benchmarks
open Solid
open System.Collections.Immutable
open Benchmarks.Wrappers

///A module that defines test targets annotated with metadata.
[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module Target =  
       
    type System.Int32 with
        member x.ToRange = Cache.MemoizeNoArgs x "ToRange" <| fun() -> [0 .. x]

    type System.Collections.Generic.IEnumerable<'a> with
        member x.ToImmutableStack() = ImmutableStack.Create<'a>(x)
        member x.ToImmutableQueue() = ImmutableQueue.Create<'a>(x)

    ///A module that defines test bindings for Solid classes.
    module My = 
        ///Returns a target binding for a Solid.FlexibleList<int>
        let List size = 
            {
                Name = "Solid.FlexibleList"
                Size = size
                Ctor = fun o -> o.Size.ToRange.ToFlexList()
                Metadata = []
            }:>ITarget<_>
        ///Returns a target binding for a Solid.Vector<int>
        let Vector size = 
            {
                Name = "Solid.Vector"
                Size = size
                Ctor = fun o -> o.Size.ToRange.ToVector()
                Metadata = []
            }:>ITarget<_>
    ///A module that defines test bindings for System.Collections.Immutable classes.
    module Sys = 
        ///Returns a target binding for a System.ImmutableList<int>
        let List size = 
            {
                Name = "System.ImmutableList"
                Size = size
                Ctor = fun o -> Sys.List<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
        ///Returns a target binding for a System.ImmutableQueue<int>
        let Queue size = 
            {
                Name = "System.ImmutableQueue"
                Size = size
                Ctor = fun o -> Sys.Queue<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
        ///Returns a target binding for a System.ImmutableStack
        let Stack size = 
            {
                Name = "System.ImmutableStack"
                Size = size
                Ctor = fun o -> Sys.Stack<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
    ///A module that defines test bindings for FSharpx.Collections classes
    module FSharpx = 
        ///Returns a target binding for a FSharpx.Deque<int>
        let Deque size= 
            {
                Name = "FSharpx.Deque"
                Size = size
                Ctor = fun o -> FSharpx'.Deque<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
    
        ///Returns a target binding for a FSharpx.Vector<int>
        let Vector size = 
            {
                Name = "FSharpx.Vector"
                Size = size
                Ctor = fun o -> FSharpx'.Vector<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
        ///Returns a target binding for a FSharpx.RandomAccessList<int>
        let RanAccList size = 
            {
                Name = "FSharpx.RanAccList"
                Size = size
                Ctor = fun o -> FSharpx'.RanAccList<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>   
