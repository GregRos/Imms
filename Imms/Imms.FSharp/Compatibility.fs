[<AutoOpen>]
module Imms.FSharp.Implementation.Compatibility
open Imms
open Imms.FSharp
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
    | Some v -> Imms.Optional.Some v
    | None -> Imms.Optional.NoneOf()
let inline fromOption (c_option : Imms.Optional<_>) = 
    if c_option.IsSome then Some c_option.Value else None
let inline (|Kvp|) (kvp : Kvp<_,_>) = Kvp(kvp.Key, kvp.Value)
let inline fromPair(k,v) = Imms.Kvp.Of(k, v)