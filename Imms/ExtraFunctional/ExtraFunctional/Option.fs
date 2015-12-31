namespace ExtraFunctional
module Option = 
    let orValue (v : 'v) = function Some u -> u | None -> v
    let cast<'a,'b> (opt : 'a option) : 'b option = opt |> Option.map (fun a -> a :> obj :?> 'b)
    let asNull (opt : 'a option) = if opt.IsSome then opt.Value else null
    
[<AutoOpen>]
module OptionExt =
    type Option<'v> with
        member x.Map f = x |> Option.map f
        member x.OrMaybe y = if x.IsSome then x else y
        member x.Or(v : 'v) = match x with Some v -> v | None -> v