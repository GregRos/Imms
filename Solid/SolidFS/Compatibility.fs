[<AutoOpen>]
module internal Solid.FSharp.Compatibility
open System
let inline toFunc1 f = Func<_,_>(f)
let inline toFunc2 f = Func<_,_,_>(f)
let inline toAction (f : 'a -> unit) = Action<'a>(f)


