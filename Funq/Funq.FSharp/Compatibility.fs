[<AutoOpen>]
module Funq.FSharp.Implementation.Compatibility
open Funq.Collections
open System
type ComplRep = CompilationRepresentationAttribute
type ComplFlags = CompilationRepresentationFlags

let inline toFunc1 f = Func<_,_>(f)
let inline toFunc2 f = Func<_,_,_>(f)
let inline toFunc3 f = Func<_,_,_,_>(f)
let inline toAction (f : 'a -> unit) = Action<'a>(f)
let inline toPredicate (f : 'a -> bool) = Predicate<'a>(f)
let inline toConverter (f : 'a -> 'b) = Converter(f)
let inline toOption maybe =
    match maybe with
    | Some v -> Funq.Option.Some v
    | None -> Funq.Option.NoneOf()
let inline fromOption (c_option : Funq.Option<_>) = 
    if c_option.IsSome then Some c_option.Value else None
let (|Kvp|) (kvp : Funq.Kvp<_,_>) = Kvp(kvp.Key, kvp.Value)
let fromPair(k,v) = Funq.Kvp.Of(k, v)