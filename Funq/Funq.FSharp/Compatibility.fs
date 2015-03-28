[<AutoOpen>]
module Funq.FSharp.Implementation.Compatibility
open Funq.Collections
open Funq.FSharp
open System
type ComplRep = CompilationRepresentationAttribute
type ComplFlags = CompilationRepresentationFlags

let toFunc1 f = Func<_,_>(f)
let toFunc2 f = Func<_,_,_>(f)
let toFunc3 f = Func<_,_,_,_>(f)
let toAction (f : 'a -> unit) = Action<'a>(f)
let toPredicate (f : 'a -> bool) = Predicate<'a>(f)
let toConverter (f : 'a -> 'b) = Converter(f)
let toOption maybe =
    match maybe with
    | Some v -> Funq.Option.Some v
    | None -> Funq.Option.NoneOf()
let fromOption (c_option : Funq.Option<_>) = 
    if c_option.IsSome then Some c_option.Value else None
let (|Kvp|) (kvp : Kvp<_,_>) = Kvp(kvp.Key, kvp.Value)
let fromPair(k,v) = Funq.Kvp.Of(k, v)