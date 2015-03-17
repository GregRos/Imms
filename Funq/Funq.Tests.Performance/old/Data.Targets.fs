namespace Funq.Benchmarking.Data
open Funq.Collections
open Funq.Benchmarking
open System.Collections.Immutable
open Funq.Benchmarking.Wrappers
open System.Linq
open Funq
module Funq = 

    let List size (s : NamedSeq<_>)= 
        {
            Name = "Funq.FunqList"
            Count = size
            Ctor = fun x -> s.Take(x.Count).ToFunqList()
            Metadata = s.Meta
        }:>IDataStruct<_>
    ///Returns a target binding for a Funq.Vector<int>
    let Vector size (s : NamedSeq<_>)= 
        {
            Name = "Funq.FunqArray"
            Count = size
            Ctor = fun x -> s |> Seq.take x.Count |> FunqArray.Empty.AddLastRange
            Metadata = s.Meta
        }:>IDataStruct<_>

    let Set size (s : NamedSeq<_>) = 
        {
            Name = "Funq.FunqSet"
            Count = size
            Ctor = fun x -> Wrappers.Funq.Set.FromSeq(s.Take(x.Count))
            Metadata = s.Meta
        }:>IDataStruct<_>

    let OrderedSet size (s : NamedSeq<_>) = 
        {
            Name = "Funq.FunqOrderedSet"
            Count = size
            Ctor = fun x -> s.Take(x.Count) |> Wrappers.Funq.OrderedSet.FromSeq
            Metadata = s.Meta
        } :> IDataStruct<_>

    let Map size  (s : NamedSeq<_>) = 
        {
            Name = "Funq.FunqMap"
            Count = size
            Ctor = fun x -> s.Take(x.Count) |> Wrappers.Funq.FunqMap.FromSeq
            Metadata = s.Meta
        } :> IDataStruct<_>

    let OrderedMap size (s : NamedSeq<_>) = 
        {
            Name = "Funq.FunqOrderedMap"
            Count = size
            Ctor = fun x -> s.Take(x.Count) |> Wrappers.Funq.OrderedMap.FromSeq
            Metadata = s.Meta
        } :> IDataStruct<_>



module Sys = 
    open Funq.Benchmarking.Wrappers
    open System.Collections.Immutable
    ///Returns a target binding for a System.ImmutableList<int>
    let List size (s : NamedSeq<_>)= 
        {
            Name = "System.ImmutableList"
            Count = size
            Ctor = fun x -> Sys.List(s.Take(x.Count).ToImmutableList())
            Metadata = s.Meta
        }:>IDataStruct<_>
    ///Returns a target binding for a System.ImmutableQueue<int>
    let Queue size (s : NamedSeq<_>)= 
        {
            Name = "System.ImmutableQueue"
            Count = size
            Ctor = fun x -> s |> Seq.take x.Count |> Sys.Queue<_>.FromSeq
            Metadata = s.Meta
        }:>IDataStruct<_>
    ///Returns a target binding for a System.ImmutableStack
    let Stack size (s : NamedSeq<_>)= 
        {
            Name = "System.ImmutableStack"
            Count = size
            Ctor = fun x -> s |> Seq.take x.Count |> Sys.Stack<_>.FromSeq
            Metadata = s.Meta
        }:>IDataStruct<_>
    let Dict size (s : NamedSeq<_>) = 
        {
            Name = "System.ImmutableDictionary"
            Count = size
            Ctor = fun x -> s |> Seq.take x.Count |> Sys.Dict<_>.FromSeq
            Metadata = s.Meta
        }:>IDataStruct<_>
    let Set size (s : NamedSeq<_>) = 
        {
            Name = "System.ImmutableSet"
            Count = size
            Ctor = fun x ->s |> Seq.take x.Count |> Sys.Set<_>.FromSeq
            Metadata = s.Meta
        }:>IDataStruct<_>    

    let SortedDict size (s : NamedSeq<_>) = 
        {
            Name = "System.ImmutableSortedDictionary"
            Count = size
            Ctor = fun x -> s |> Seq.take x.Count |> Sys.SortedDict<_>.FromSeq
            Metadata = s.Meta
        }:>IDataStruct<_>
    let SortedSet size (s : NamedSeq<_>) = 
        {
            Name = "System.ImmutableSortedSet"
            Count = size
            Ctor = fun x ->s |> Seq.take x.Count |> Sys.SortedSet<_>.FromSeq
            Metadata = s.Meta
        }:>IDataStruct<_>  

    
module FSharpx = 
    ///Returns a target binding for a FSharpx.Deque<int>
    let Deque size (s : NamedSeq<_>)= 
        {
            Name = "FSharpx.Deque"
            Count = size
            Ctor = fun x -> Wrappers.FSharpx.Deque<_>.FromSeq(s.Take(x.Count))
            Metadata = s.Meta
        }:>IDataStruct<_>
    
    ///Returns a target binding for a FSharpx.Vector<int>
    let Vector size (s : NamedSeq<_>)= 
        {
            Name = "FSharpx.Vector"
            Count = size
            Ctor = fun x -> Wrappers.FSharpx.Vector<_>.FromSeq(s.Take(x.Count))
            Metadata = s.Meta
        }:>IDataStruct<_>
    ///Returns a target binding for a FSharpx.RandomAccessList<int>
    let RanAccList size (s : NamedSeq<_>)= 
        {
            Name = "FSharpx.RanAccList"
            Count = size
            Ctor = fun x -> Wrappers.FSharpx.RanAccList<_>.FromSeq(s.Take(x.Count))
            Metadata = s.Meta
        }:>IDataStruct<_>
module FSharp = 
    let Map size (s : NamedSeq<_>) = 
        {
            Name = "FSharp.Map"
            Count = size
            Ctor = fun x -> Wrappers.FSharp.Map<_>.FromSeq(s.Take(x.Count))
            Metadata=[]
        }:>IDataStruct<_> 
