namespace Imms.Tests.Performance.Data
open Imms.Tests.Performance
open System.Collections.Immutable
open Imms.Tests.Performance.Wrappers
open System.Linq
open Imms
#nowarn"0044"
[<AutoOpen>]
module internal Shorthand = 
    let data ctor kind library name size gen = DataStructure.Create(name,kind, size, library, gen, ctor)

module Imms = 
    open Imms.Tests.Performance.Wrappers
    let List (size, s) = data (ImmList.ToImmList) Sequential "Imm" "ImmList"  size s
    let Vector (size, s)= data (ImmVector.ToImmVector) Sequential "Imm" "ImmVector" size s
    let Set (size, s) = data    (Set.FromSeq) SetLike "Imm"  "ImmSet" size s
    let OrderedSet (size, s) = data (OrderedSet.FromSeq) SetLike "Imm" "ImmSortedSet" size s
    let Map (size, s) = data (ImmMap.FromSeq) MapLike "Imm" "ImmMap"  size s
    let OrderedMap (size, s) = data (OrderedMap.FromSeq) MapLike "Imm" "ImmSortedMap" size s
    ///Returns a target binding for a Imms.Vector<int> siz
    
module internal Sys = 
    open Imms.Tests.Performance.Wrappers
    open System.Collections.Immutable
    ///Returns a target binding for a System.ImmutableList<int>
    let List (size, s) = data (Sys.List<_>.FromSeq) Sequential "System" "System.ImmutableList" size s
    let Queue (size, s) = data (Sys.Queue<_>.FromSeq) Sequential "System" "System.ImmutableQueue" size s
    let Stack (size, s) = data (Sys.Stack<_>.FromSeq) Sequential "System" "System.ImmutableStack" size s
    let Dict (size, s) = data (Sys.Dict<_>.FromSeq) MapLike "System" "System.ImmutableDict" size s
    let Set (size, s) = data (Sys.Set<_>.FromSeq) SetLike "System" "System.ImmutableSet" size s
    let SortedSet (size, s) = data (Sys.SortedSet<_>.FromSeq) SetLike "System" "System.ImmutableSortedSet" size s
    let SortedDict (size, s) = data (Sys.SortedDict<_>.FromSeq) MapLike "System" "System.ImmutableSortedDict" size s

module internal FSharpx = 
    ///Returns a target binding for a FSharpx.Deque<int>
    open Imms.Tests.Performance.Wrappers
    let Deque (size, s) = data (FSharpx.Deque<_>.FromSeq) Sequential "FSharpx" "FSharpx.Deque" size s
    let Vector (size, s) = data (FSharpx.Vector<_>.FromSeq) Sequential "FSharpx" "FSharpx.Vector" size s
    let RanAccList (size, s) = data (FSharpx.RanAccList<_>.FromSeq) Sequential "FSharpx" "FSharpx.RandomAccessList" size s

module internal FSharp = 
    let Map (size, s) = data (FSharp.Map<_>.FromSeq) MapLike "FSharp" "FSharp.Map" size s
    let Set (size, s) = data (FSharp.Set<_>.FromSeq) SetLike "FSharp"  "FSharp.Set" size s