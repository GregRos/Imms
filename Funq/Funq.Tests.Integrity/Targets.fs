module Funq.Tests.Integrity.Target
open Funq.Tests.Integrity.Wrappers
open System.Collections.Immutable
open Funq.FSharp
open Funq.Collections
let FunqList n = FunqListWrapper<_>(FunqList.Empty().AddLastRange [0 .. n]) :> TestWrapper<_>
let FunqArray n = FunqVectorWrapper<_>(FunqVector.Empty().AddLastRange [0 .. n]) :> TestWrapper<_>
let Reference n = ReferenceWrapper<_>(ImmutableList.Empty.AddRange [0 .. n]) :> TestWrapper<_>


