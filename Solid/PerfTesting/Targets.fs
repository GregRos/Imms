namespace Benchmarks
open Solid
open System.Collections.Immutable
open Benchmarks.Wrappers

[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module Target =  
       
    type System.Int32 with
        member x.ToRange = Cache.MemoizeNoArgs x "ToRange" <| fun() -> [0 .. x]

    type System.Collections.Generic.IEnumerable<'a> with
        member x.ToImmutableStack() = ImmutableStack.Create<'a>(x)
        member x.ToImmutableQueue() = ImmutableQueue.Create<'a>(x)

    //Each target is constructed using an object expression. 
    
    let inline NewTarget name size extra_params ctor =
        { Name = name; Size = size; Ctor = ctor; Metadata=extra_params}:>ITarget<_>

    module My = 
        let List size = 
            {
                Name = "Solid.FlexibleList"
                Size = size
                Ctor = fun o -> o.Size.ToRange.ToFlexList()
                Metadata = []
            }:>ITarget<_>

        let Vector size = 
            {
                Name = "Solid.Vector"
                Size = size
                Ctor = fun o -> o.Size.ToRange.ToVector()
                Metadata = []
            }:>ITarget<_>

    module Sys = 
        let List size = 
            {
                Name = "System.ImmutableList"
                Size = size
                Ctor = fun o -> Sys.List<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>

        let Queue size = 
            {
                Name = "System.ImmutableQueue"
                Size = size
                Ctor = fun o -> Sys.Queue<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
    
        let Stack size = 
            {
                Name = "System.ImmutableStack"
                Size = size
                Ctor = fun o -> Sys.Stack<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
    module FSharpx = 
        let Deque size= 
            {
                Name = "FSharpx.Deque"
                Size = size
                Ctor = fun o -> FSharpx'.Deque<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>
    
    
        let Vector size = 
            {
                Name = "FSharpx.Vector"
                Size = size
                Ctor = fun o -> FSharpx'.Vector<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>

        let RanAccList size = 
            {
                Name = "FSharpx.RanAccList"
                Size = size
                Ctor = fun o -> FSharpx'.RanAccList<_>.FromSeq o.Size.ToRange
                Metadata = []
            }:>ITarget<_>   


    let Many (targets:#seq<_>) (ns:#seq<_>) = 
        let crs = List.cross targets ns
        crs |> List.mapPairs (fun targ arg -> targ arg)