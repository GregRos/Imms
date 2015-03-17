namespace Funq.Tests.Performance.Data
open Funq.Collections
open Funq.Tests.Performance
open System.Collections.Immutable
open Funq.Tests.Performance.Wrappers
open System.Linq
open Funq
[<AutoOpen>]
module Shorthand = 
    let data ctor name size gen = DataStructure.Create(name, size, gen, ctor)

module Funq = 
    open Funq.Tests.Performance.Wrappers
    let List size s = data (FunqList.ToFunqList) "FunqList" size s
    let Array size s= data (FunqArray.ToFunqArray) "FunqArray" size s
    let Set size s = data    (Set.FromSeq) "FunqSet" size s
    let OrderedSet size s = data (OrderedSet.FromSeq) "FunqOrderedSet" size s
    let Map size s = data (FunqMap.FromSeq) "FunqMap" size s
    let OrderedMap size s = data (OrderedMap.FromSeq) "FunqOrderedMap" size s
    ///Returns a target binding for a Funq.Vector<int> siz
    
module Sys = 
    open Funq.Tests.Performance.Wrappers
    open System.Collections.Immutable
    ///Returns a target binding for a System.ImmutableList<int>
    let List size s = data (Sys.List<_>.FromSeq) "System.ImmutableList" size s
    let Queue size s = data (Sys.Queue<_>.FromSeq) "System.ImmutableQueue" size s
    let Stack size s = data (Sys.Stack<_>.FromSeq) "System.ImmutableStack" size s
    let Dict size s = data (Sys.Dict<_>.FromSeq) "System.ImmutableDict" size s
    let Set size s = data (Sys.Set<_>.FromSeq) "System.ImmutableSet" size s
    let SortedSet size s = data (Sys.SortedSet<_>.FromSeq) "System.ImmutableSortedSet" size s
    let SortedDict size s = data (Sys.SortedDict<_>.FromSeq) "System.ImmutableSortedDict" size s

module FSharpx = 
    ///Returns a target binding for a FSharpx.Deque<int>
    open Funq.Tests.Performance.Wrappers
    let Deque size s = data (FSharpx.Deque<_>.FromSeq) "FSharpx.Deque" size s
    let Vector size s = data (FSharpx.Vector<_>.FromSeq) "FSharpx.Vector" size s
    let RanAccList size s = data (FSharpx.RanAccList<_>.FromSeq) "FSharpx.RandomAccessList" size s

module FSharp = 
    let Map size s = data (FSharp.Map<_>.FromSeq) "FSharp.Map" size s