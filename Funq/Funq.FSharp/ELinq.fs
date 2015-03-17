module Solid.ELinq

let inline iter f x = (^s : (member ForEach : ('a -> unit) -> unit) x, f)

let inline iterBack f x = (^s : (member ForEachBack : ('a -> unit) -> unit) x, f)

let inline iterWhile f x = (^s : (member ForEachWhile : ('a -> unit) -> unit) x, f)

let inline iterBackWhile f x = (^s : (member ForEachBackWhile : ('a -> unit) -> unit) x, f)

type LinqOp = interface end

type Choose<'a,'b>(f : 'a option -> 'b option) = 
    interface LinqOp
    member val Selector = f

type ELinqQuery<'a,'b>(top : LinqOp) = 
    member x.Choose (f : 'b option -> 'c option) = 
        match top with
        | :? Choose<'a,'b> as choose -> 
            let newChoose = fun x -> f(choose.Selector(x))
            ELinqQuery<'a, 'c>(Choose(newChoose))
        | _ -> failwith "?"



