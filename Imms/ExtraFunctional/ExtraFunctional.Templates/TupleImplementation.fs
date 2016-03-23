namespace ExtraFunctional
#nowarn"0044" //Hide obsolete warning.
open System
open System.Runtime.CompilerServices
[<AutoOpen>]
module TypeLiterals = 
    type IStaticInt =
        abstract Value : int


    

    type S0 = S0 with interface IStaticInt with member x.Value = 0
    

    type S1 = S1 with interface IStaticInt with member x.Value = 1
    

    type S2 = S2 with interface IStaticInt with member x.Value = 2
    

    type S3 = S3 with interface IStaticInt with member x.Value = 3
    

    type S4 = S4 with interface IStaticInt with member x.Value = 4
    

    type S5 = S5 with interface IStaticInt with member x.Value = 5
    

    type S6 = S6 with interface IStaticInt with member x.Value = 6
    

    type S7 = S7 with interface IStaticInt with member x.Value = 7
    

    type S8 = S8 with interface IStaticInt with member x.Value = 8
    

    type S9 = S9 with interface IStaticInt with member x.Value = 9
    

    type S10 = S10 with interface IStaticInt with member x.Value = 10
    [<AutoOpen>]
    module Ext = 
        type IStaticInt with
            member x.Value = x.Value

[<Obsolete("This object is not meant to be visible or used in user code.")>]
module TupleImplementation =
    [<Literal>]
    let ImplementationWarning = "This object is not meant to be visible or used in user code."
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple2<'a0, 'a1>(a0 : 'a0, a1 : 'a1) =
        member x.Invoke f = f a0 a1
        member x.StaticLength() = S2
        member x.Last() = a1

        member x.Rev() = (a1, a0)

        member x.Zip(a0, a1) = 
            (a0, a0), (a1, a1)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't

        member x.Map(f0, f1) =
            f0 a0, f1 a1

        member x.Fold(initial, f0, f1) =
            f0 a0 initial |> f1 a1

        member x.Reduce(f0) =
            f0 a0 a1 
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1
        member x.SetItem(S2, b2) = a0, a1, b2 

        member x.Cons b0 = b0, a0, a1

        member x.Conj b2 = a0, a1, b2
    
    type FuncTuple<'inp, 'outp0, 'outp1>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1) = 
        member x.Apply arg = f0 arg, f1 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple2<'a>(a0 : 'a, a1 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1
                | 1 -> a0, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 2 )
        member x.ToList() = [a0; a1]
        member x.ToArray() = [|a0; a1|]
        member x.Map f = f a0, f a1
        member x.Fold initial f = f a0 initial |> f a1
        member x.Reduce f = f a1 a0 
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 2)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple3<'a0, 'a1, 'a2>(a0 : 'a0, a1 : 'a1, a2 : 'a2) =
        member x.Invoke f = f a0 a1 a2
        member x.StaticLength() = S3
        member x.Last() = a2

        member x.Rev() = (a2, a1, a0)

        member x.Zip(a0, a1, a2) = 
            (a0, a0), (a1, a1), (a2, a2)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't

        member x.Map(f0, f1, f2) =
            f0 a0, f1 a1, f2 a2

        member x.Fold(initial, f0, f1, f2) =
            f0 a0 initial |> f1 a1 |> f2 a2

        member x.Reduce(f0, f1) =
            f0 a0 a1 
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2
        member x.SetItem(S3, b3) = a0, a1, a2, b3 

        member x.Cons b0 = b0, a0, a1, a2

        member x.Conj b3 = a0, a1, a2, b3
        member x.Initial() = a0, a1

        member x.Tail() = a1, a2

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple3<'a>(a0 : 'a, a1 : 'a, a2 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2
                | 1 -> a0, v, a2
                | 2 -> a0, a1, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 3 )
        member x.ToList() = [a0; a1; a2]
        member x.ToArray() = [|a0; a1; a2|]
        member x.Map f = f a0, f a1, f a2
        member x.Fold initial f = f a0 initial |> f a1 |> f a2
        member x.Reduce f = f a1 a0  |> f a2
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 3)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple4<'a0, 'a1, 'a2, 'a3>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3) =
        member x.Invoke f = f a0 a1 a2 a3
        member x.StaticLength() = S4
        member x.Last() = a3

        member x.Rev() = (a3, a2, a1, a0)

        member x.Zip(a0, a1, a2, a3) = 
            (a0, a0), (a1, a1), (a2, a2), (a3, a3)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't, a3 :> obj :?> 't

        member x.Map(f0, f1, f2, f3) =
            f0 a0, f1 a1, f2 a2, f3 a3

        member x.Fold(initial, f0, f1, f2, f3) =
            f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3

        member x.Reduce(f0, f1, f2) =
            f0 a0 a1  |> f2 a2
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2, a3
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2, a3
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2, a3
        member x.Item S3 = a3

        member x.SetItem(S3, b3) = a0, a1, a2, b3
        member x.SetItem(S4, b4) = a0, a1, a2, a3, b4 

        member x.Cons b0 = b0, a0, a1, a2, a3

        member x.Conj b4 = a0, a1, a2, a3, b4
        member x.Initial() = a0, a1, a2

        member x.Tail() = a1, a2, a3

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2, 'outp3>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2, f3 : 'inp -> 'outp3) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg, f3 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple4<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2, a3
                | 1 -> a0, v, a2, a3
                | 2 -> a0, a1, v, a3
                | 3 -> a0, a1, a2, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 4 )
        member x.ToList() = [a0; a1; a2; a3]
        member x.ToArray() = [|a0; a1; a2; a3|]
        member x.Map f = f a0, f a1, f a2, f a3
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3
        member x.Reduce f = f a1 a0  |> f a2 |> f a3
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 4)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple5<'a0, 'a1, 'a2, 'a3, 'a4>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4) =
        member x.Invoke f = f a0 a1 a2 a3 a4
        member x.StaticLength() = S5
        member x.Last() = a4

        member x.Rev() = (a4, a3, a2, a1, a0)

        member x.Zip(a0, a1, a2, a3, a4) = 
            (a0, a0), (a1, a1), (a2, a2), (a3, a3), (a4, a4)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't, a3 :> obj :?> 't, a4 :> obj :?> 't

        member x.Map(f0, f1, f2, f3, f4) =
            f0 a0, f1 a1, f2 a2, f3 a3, f4 a4

        member x.Fold(initial, f0, f1, f2, f3, f4) =
            f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4

        member x.Reduce(f0, f1, f2, f3) =
            f0 a0 a1  |> f2 a2 |> f3 a3
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2, a3, a4
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2, a3, a4
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2, a3, a4
        member x.Item S3 = a3

        member x.SetItem(S3, b3) = a0, a1, a2, b3, a4
        member x.Item S4 = a4

        member x.SetItem(S4, b4) = a0, a1, a2, a3, b4
        member x.SetItem(S5, b5) = a0, a1, a2, a3, a4, b5 

        member x.Cons b0 = b0, a0, a1, a2, a3, a4

        member x.Conj b5 = a0, a1, a2, a3, a4, b5
        member x.Initial() = a0, a1, a2, a3

        member x.Tail() = a1, a2, a3, a4

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2, 'outp3, 'outp4>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2, f3 : 'inp -> 'outp3, f4 : 'inp -> 'outp4) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg, f3 arg, f4 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple5<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2, a3, a4
                | 1 -> a0, v, a2, a3, a4
                | 2 -> a0, a1, v, a3, a4
                | 3 -> a0, a1, a2, v, a4
                | 4 -> a0, a1, a2, a3, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 5 )
        member x.ToList() = [a0; a1; a2; a3; a4]
        member x.ToArray() = [|a0; a1; a2; a3; a4|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4
        member x.Reduce f = f a1 a0  |> f a2 |> f a3 |> f a4
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 5)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple6<'a0, 'a1, 'a2, 'a3, 'a4, 'a5>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5) =
        member x.Invoke f = f a0 a1 a2 a3 a4 a5
        member x.StaticLength() = S6
        member x.Last() = a5

        member x.Rev() = (a5, a4, a3, a2, a1, a0)

        member x.Zip(a0, a1, a2, a3, a4, a5) = 
            (a0, a0), (a1, a1), (a2, a2), (a3, a3), (a4, a4), (a5, a5)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't, a3 :> obj :?> 't, a4 :> obj :?> 't, a5 :> obj :?> 't

        member x.Map(f0, f1, f2, f3, f4, f5) =
            f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5

        member x.Fold(initial, f0, f1, f2, f3, f4, f5) =
            f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5

        member x.Reduce(f0, f1, f2, f3, f4) =
            f0 a0 a1  |> f2 a2 |> f3 a3 |> f4 a4
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2, a3, a4, a5
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2, a3, a4, a5
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2, a3, a4, a5
        member x.Item S3 = a3

        member x.SetItem(S3, b3) = a0, a1, a2, b3, a4, a5
        member x.Item S4 = a4

        member x.SetItem(S4, b4) = a0, a1, a2, a3, b4, a5
        member x.Item S5 = a5

        member x.SetItem(S5, b5) = a0, a1, a2, a3, a4, b5
        member x.SetItem(S6, b6) = a0, a1, a2, a3, a4, a5, b6 

        member x.Cons b0 = b0, a0, a1, a2, a3, a4, a5

        member x.Conj b6 = a0, a1, a2, a3, a4, a5, b6
        member x.Initial() = a0, a1, a2, a3, a4

        member x.Tail() = a1, a2, a3, a4, a5

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2, 'outp3, 'outp4, 'outp5>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2, f3 : 'inp -> 'outp3, f4 : 'inp -> 'outp4, f5 : 'inp -> 'outp5) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg, f3 arg, f4 arg, f5 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple6<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2, a3, a4, a5
                | 1 -> a0, v, a2, a3, a4, a5
                | 2 -> a0, a1, v, a3, a4, a5
                | 3 -> a0, a1, a2, v, a4, a5
                | 4 -> a0, a1, a2, a3, v, a5
                | 5 -> a0, a1, a2, a3, a4, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 6 )
        member x.ToList() = [a0; a1; a2; a3; a4; a5]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5
        member x.Reduce f = f a1 a0  |> f a2 |> f a3 |> f a4 |> f a5
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 6)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple7<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5, a6 : 'a6) =
        member x.Invoke f = f a0 a1 a2 a3 a4 a5 a6
        member x.StaticLength() = S7
        member x.Last() = a6

        member x.Rev() = (a6, a5, a4, a3, a2, a1, a0)

        member x.Zip(a0, a1, a2, a3, a4, a5, a6) = 
            (a0, a0), (a1, a1), (a2, a2), (a3, a3), (a4, a4), (a5, a5), (a6, a6)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't, a3 :> obj :?> 't, a4 :> obj :?> 't, a5 :> obj :?> 't, a6 :> obj :?> 't

        member x.Map(f0, f1, f2, f3, f4, f5, f6) =
            f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5, f6 a6

        member x.Fold(initial, f0, f1, f2, f3, f4, f5, f6) =
            f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6

        member x.Reduce(f0, f1, f2, f3, f4, f5) =
            f0 a0 a1  |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2, a3, a4, a5, a6
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2, a3, a4, a5, a6
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2, a3, a4, a5, a6
        member x.Item S3 = a3

        member x.SetItem(S3, b3) = a0, a1, a2, b3, a4, a5, a6
        member x.Item S4 = a4

        member x.SetItem(S4, b4) = a0, a1, a2, a3, b4, a5, a6
        member x.Item S5 = a5

        member x.SetItem(S5, b5) = a0, a1, a2, a3, a4, b5, a6
        member x.Item S6 = a6

        member x.SetItem(S6, b6) = a0, a1, a2, a3, a4, a5, b6
        member x.SetItem(S7, b7) = a0, a1, a2, a3, a4, a5, a6, b7 

        member x.Cons b0 = b0, a0, a1, a2, a3, a4, a5, a6

        member x.Conj b7 = a0, a1, a2, a3, a4, a5, a6, b7
        member x.Initial() = a0, a1, a2, a3, a4, a5

        member x.Tail() = a1, a2, a3, a4, a5, a6

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2, 'outp3, 'outp4, 'outp5, 'outp6>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2, f3 : 'inp -> 'outp3, f4 : 'inp -> 'outp4, f5 : 'inp -> 'outp5, f6 : 'inp -> 'outp6) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg, f3 arg, f4 arg, f5 arg, f6 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple7<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a, a6 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2, a3, a4, a5, a6
                | 1 -> a0, v, a2, a3, a4, a5, a6
                | 2 -> a0, a1, v, a3, a4, a5, a6
                | 3 -> a0, a1, a2, v, a4, a5, a6
                | 4 -> a0, a1, a2, a3, v, a5, a6
                | 5 -> a0, a1, a2, a3, a4, v, a6
                | 6 -> a0, a1, a2, a3, a4, a5, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 7 )
        member x.ToList() = [a0; a1; a2; a3; a4; a5; a6]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5; a6|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5, f a6
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6
        member x.Reduce f = f a1 a0  |> f a2 |> f a3 |> f a4 |> f a5 |> f a6
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | 6 -> a6
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 7)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple8<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6, 'a7>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5, a6 : 'a6, a7 : 'a7) =
        member x.Invoke f = f a0 a1 a2 a3 a4 a5 a6 a7
        member x.StaticLength() = S8
        member x.Last() = a7

        member x.Rev() = (a7, a6, a5, a4, a3, a2, a1, a0)

        member x.Zip(a0, a1, a2, a3, a4, a5, a6, a7) = 
            (a0, a0), (a1, a1), (a2, a2), (a3, a3), (a4, a4), (a5, a5), (a6, a6), (a7, a7)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't, a3 :> obj :?> 't, a4 :> obj :?> 't, a5 :> obj :?> 't, a6 :> obj :?> 't, a7 :> obj :?> 't

        member x.Map(f0, f1, f2, f3, f4, f5, f6, f7) =
            f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5, f6 a6, f7 a7

        member x.Fold(initial, f0, f1, f2, f3, f4, f5, f6, f7) =
            f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6 |> f7 a7

        member x.Reduce(f0, f1, f2, f3, f4, f5, f6) =
            f0 a0 a1  |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2, a3, a4, a5, a6, a7
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2, a3, a4, a5, a6, a7
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2, a3, a4, a5, a6, a7
        member x.Item S3 = a3

        member x.SetItem(S3, b3) = a0, a1, a2, b3, a4, a5, a6, a7
        member x.Item S4 = a4

        member x.SetItem(S4, b4) = a0, a1, a2, a3, b4, a5, a6, a7
        member x.Item S5 = a5

        member x.SetItem(S5, b5) = a0, a1, a2, a3, a4, b5, a6, a7
        member x.Item S6 = a6

        member x.SetItem(S6, b6) = a0, a1, a2, a3, a4, a5, b6, a7
        member x.Item S7 = a7

        member x.SetItem(S7, b7) = a0, a1, a2, a3, a4, a5, a6, b7
        member x.SetItem(S8, b8) = a0, a1, a2, a3, a4, a5, a6, a7, b8 

        member x.Cons b0 = b0, a0, a1, a2, a3, a4, a5, a6, a7

        member x.Conj b8 = a0, a1, a2, a3, a4, a5, a6, a7, b8
        member x.Initial() = a0, a1, a2, a3, a4, a5, a6

        member x.Tail() = a1, a2, a3, a4, a5, a6, a7

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2, 'outp3, 'outp4, 'outp5, 'outp6, 'outp7>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2, f3 : 'inp -> 'outp3, f4 : 'inp -> 'outp4, f5 : 'inp -> 'outp5, f6 : 'inp -> 'outp6, f7 : 'inp -> 'outp7) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg, f3 arg, f4 arg, f5 arg, f6 arg, f7 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple8<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a, a6 : 'a, a7 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2, a3, a4, a5, a6, a7
                | 1 -> a0, v, a2, a3, a4, a5, a6, a7
                | 2 -> a0, a1, v, a3, a4, a5, a6, a7
                | 3 -> a0, a1, a2, v, a4, a5, a6, a7
                | 4 -> a0, a1, a2, a3, v, a5, a6, a7
                | 5 -> a0, a1, a2, a3, a4, v, a6, a7
                | 6 -> a0, a1, a2, a3, a4, a5, v, a7
                | 7 -> a0, a1, a2, a3, a4, a5, a6, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 8 )
        member x.ToList() = [a0; a1; a2; a3; a4; a5; a6; a7]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5; a6; a7|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5, f a6, f a7
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6 |> f a7
        member x.Reduce f = f a1 a0  |> f a2 |> f a3 |> f a4 |> f a5 |> f a6 |> f a7
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | 6 -> a6
            | 7 -> a7
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 8)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple9<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6, 'a7, 'a8>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5, a6 : 'a6, a7 : 'a7, a8 : 'a8) =
        member x.Invoke f = f a0 a1 a2 a3 a4 a5 a6 a7 a8
        member x.StaticLength() = S9
        member x.Last() = a8

        member x.Rev() = (a8, a7, a6, a5, a4, a3, a2, a1, a0)

        member x.Zip(a0, a1, a2, a3, a4, a5, a6, a7, a8) = 
            (a0, a0), (a1, a1), (a2, a2), (a3, a3), (a4, a4), (a5, a5), (a6, a6), (a7, a7), (a8, a8)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't, a3 :> obj :?> 't, a4 :> obj :?> 't, a5 :> obj :?> 't, a6 :> obj :?> 't, a7 :> obj :?> 't, a8 :> obj :?> 't

        member x.Map(f0, f1, f2, f3, f4, f5, f6, f7, f8) =
            f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5, f6 a6, f7 a7, f8 a8

        member x.Fold(initial, f0, f1, f2, f3, f4, f5, f6, f7, f8) =
            f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6 |> f7 a7 |> f8 a8

        member x.Reduce(f0, f1, f2, f3, f4, f5, f6, f7) =
            f0 a0 a1  |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6 |> f7 a7
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2, a3, a4, a5, a6, a7, a8
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2, a3, a4, a5, a6, a7, a8
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2, a3, a4, a5, a6, a7, a8
        member x.Item S3 = a3

        member x.SetItem(S3, b3) = a0, a1, a2, b3, a4, a5, a6, a7, a8
        member x.Item S4 = a4

        member x.SetItem(S4, b4) = a0, a1, a2, a3, b4, a5, a6, a7, a8
        member x.Item S5 = a5

        member x.SetItem(S5, b5) = a0, a1, a2, a3, a4, b5, a6, a7, a8
        member x.Item S6 = a6

        member x.SetItem(S6, b6) = a0, a1, a2, a3, a4, a5, b6, a7, a8
        member x.Item S7 = a7

        member x.SetItem(S7, b7) = a0, a1, a2, a3, a4, a5, a6, b7, a8
        member x.Item S8 = a8

        member x.SetItem(S8, b8) = a0, a1, a2, a3, a4, a5, a6, a7, b8
        member x.SetItem(S9, b9) = a0, a1, a2, a3, a4, a5, a6, a7, a8, b9 

        member x.Cons b0 = b0, a0, a1, a2, a3, a4, a5, a6, a7, a8

        member x.Conj b9 = a0, a1, a2, a3, a4, a5, a6, a7, a8, b9
        member x.Initial() = a0, a1, a2, a3, a4, a5, a6, a7

        member x.Tail() = a1, a2, a3, a4, a5, a6, a7, a8

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2, 'outp3, 'outp4, 'outp5, 'outp6, 'outp7, 'outp8>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2, f3 : 'inp -> 'outp3, f4 : 'inp -> 'outp4, f5 : 'inp -> 'outp5, f6 : 'inp -> 'outp6, f7 : 'inp -> 'outp7, f8 : 'inp -> 'outp8) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg, f3 arg, f4 arg, f5 arg, f6 arg, f7 arg, f8 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple9<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a, a6 : 'a, a7 : 'a, a8 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2, a3, a4, a5, a6, a7, a8
                | 1 -> a0, v, a2, a3, a4, a5, a6, a7, a8
                | 2 -> a0, a1, v, a3, a4, a5, a6, a7, a8
                | 3 -> a0, a1, a2, v, a4, a5, a6, a7, a8
                | 4 -> a0, a1, a2, a3, v, a5, a6, a7, a8
                | 5 -> a0, a1, a2, a3, a4, v, a6, a7, a8
                | 6 -> a0, a1, a2, a3, a4, a5, v, a7, a8
                | 7 -> a0, a1, a2, a3, a4, a5, a6, v, a8
                | 8 -> a0, a1, a2, a3, a4, a5, a6, a7, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 9 )
        member x.ToList() = [a0; a1; a2; a3; a4; a5; a6; a7; a8]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5; a6; a7; a8|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5, f a6, f a7, f a8
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6 |> f a7 |> f a8
        member x.Reduce f = f a1 a0  |> f a2 |> f a3 |> f a4 |> f a5 |> f a6 |> f a7 |> f a8
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | 6 -> a6
            | 7 -> a7
            | 8 -> a8
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 9)
            
    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple10<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6, 'a7, 'a8, 'a9>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5, a6 : 'a6, a7 : 'a7, a8 : 'a8, a9 : 'a9) =
        member x.Invoke f = f a0 a1 a2 a3 a4 a5 a6 a7 a8 a9
        member x.StaticLength() = S10
        member x.Last() = a9

        member x.Rev() = (a9, a8, a7, a6, a5, a4, a3, a2, a1, a0)

        member x.Zip(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9) = 
            (a0, a0), (a1, a1), (a2, a2), (a3, a3), (a4, a4), (a5, a5), (a6, a6), (a7, a7), (a8, a8), (a9, a9)

        member x.Cast<'t>() = a0 :> obj :?> 't, a1 :> obj :?> 't, a2 :> obj :?> 't, a3 :> obj :?> 't, a4 :> obj :?> 't, a5 :> obj :?> 't, a6 :> obj :?> 't, a7 :> obj :?> 't, a8 :> obj :?> 't, a9 :> obj :?> 't

        member x.Map(f0, f1, f2, f3, f4, f5, f6, f7, f8, f9) =
            f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5, f6 a6, f7 a7, f8 a8, f9 a9

        member x.Fold(initial, f0, f1, f2, f3, f4, f5, f6, f7, f8, f9) =
            f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6 |> f7 a7 |> f8 a8 |> f9 a9

        member x.Reduce(f0, f1, f2, f3, f4, f5, f6, f7, f8) =
            f0 a0 a1  |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6 |> f7 a7 |> f8 a8
        member x.Item S0 = a0

        member x.SetItem(S0, b0) = b0, a1, a2, a3, a4, a5, a6, a7, a8, a9
        member x.Item S1 = a1

        member x.SetItem(S1, b1) = a0, b1, a2, a3, a4, a5, a6, a7, a8, a9
        member x.Item S2 = a2

        member x.SetItem(S2, b2) = a0, a1, b2, a3, a4, a5, a6, a7, a8, a9
        member x.Item S3 = a3

        member x.SetItem(S3, b3) = a0, a1, a2, b3, a4, a5, a6, a7, a8, a9
        member x.Item S4 = a4

        member x.SetItem(S4, b4) = a0, a1, a2, a3, b4, a5, a6, a7, a8, a9
        member x.Item S5 = a5

        member x.SetItem(S5, b5) = a0, a1, a2, a3, a4, b5, a6, a7, a8, a9
        member x.Item S6 = a6

        member x.SetItem(S6, b6) = a0, a1, a2, a3, a4, a5, b6, a7, a8, a9
        member x.Item S7 = a7

        member x.SetItem(S7, b7) = a0, a1, a2, a3, a4, a5, a6, b7, a8, a9
        member x.Item S8 = a8

        member x.SetItem(S8, b8) = a0, a1, a2, a3, a4, a5, a6, a7, b8, a9
        member x.Item S9 = a9

        member x.SetItem(S9, b9) = a0, a1, a2, a3, a4, a5, a6, a7, a8, b9
        member x.Initial() = a0, a1, a2, a3, a4, a5, a6, a7, a8

        member x.Tail() = a1, a2, a3, a4, a5, a6, a7, a8, a9

    
    type FuncTuple<'inp, 'outp0, 'outp1, 'outp2, 'outp3, 'outp4, 'outp5, 'outp6, 'outp7, 'outp8, 'outp9>(f0 : 'inp -> 'outp0, f1 : 'inp -> 'outp1, f2 : 'inp -> 'outp2, f3 : 'inp -> 'outp3, f4 : 'inp -> 'outp4, f5 : 'inp -> 'outp5, f6 : 'inp -> 'outp6, f7 : 'inp -> 'outp7, f8 : 'inp -> 'outp8, f9 : 'inp -> 'outp9) = 
        member x.Apply arg = f0 arg, f1 arg, f2 arg, f3 arg, f4 arg, f5 arg, f6 arg, f7 arg, f8 arg, f9 arg

    [<Obsolete(ImplementationWarning)>]
    [<Struct>]
    type Tuple10<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a, a6 : 'a, a7 : 'a, a8 : 'a, a9 : 'a) =
        member x.Set(index, v) = 
            match index with
                | 0 -> v, a1, a2, a3, a4, a5, a6, a7, a8, a9
                | 1 -> a0, v, a2, a3, a4, a5, a6, a7, a8, a9
                | 2 -> a0, a1, v, a3, a4, a5, a6, a7, a8, a9
                | 3 -> a0, a1, a2, v, a4, a5, a6, a7, a8, a9
                | 4 -> a0, a1, a2, a3, v, a5, a6, a7, a8, a9
                | 5 -> a0, a1, a2, a3, a4, v, a6, a7, a8, a9
                | 6 -> a0, a1, a2, a3, a4, a5, v, a7, a8, a9
                | 7 -> a0, a1, a2, a3, a4, a5, a6, v, a8, a9
                | 8 -> a0, a1, a2, a3, a4, a5, a6, a7, v, a9
                | 9 -> a0, a1, a2, a3, a4, a5, a6, a7, a8, v
                | _ -> invalidArg "index" (sprintf "The index '%d' is out of bounds of the tuple, which has '%d' elements" index 10 )
        member x.ToList() = [a0; a1; a2; a3; a4; a5; a6; a7; a8; a9]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5; a6; a7; a8; a9|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5, f a6, f a7, f a8, f a9
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6 |> f a7 |> f a8 |> f a9
        member x.Reduce f = f a1 a0  |> f a2 |> f a3 |> f a4 |> f a5 |> f a6 |> f a7 |> f a8 |> f a9
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | 6 -> a6
            | 7 -> a7
            | 8 -> a8
            | 9 -> a9
            | _ -> invalidArg "index" (sprintf "The index value '%d' was out of bounds, as this tuple contains '%d' elements" index 10)
            
    
    type Helper internal() = 
        static member IsTuple((a0, a1)) = Tuple2<'a0, 'a1>(a0, a1)

        static member IsTupleSameType((a0, a1)) = Tuple2<'a>(a0, a1)

        static member Init(S2, f : int -> 'out) = f 0, f 1

        

        static member InitElement(S2, element) = element, element
        static member IsTuple((a0, a1, a2)) = Tuple3<'a0, 'a1, 'a2>(a0, a1, a2)

        static member IsTupleSameType((a0, a1, a2)) = Tuple3<'a>(a0, a1, a2)

        static member Init(S3, f : int -> 'out) = f 0, f 1, f 2

        

        static member InitElement(S3, element) = element, element, element
        static member IsTuple((a0, a1, a2, a3)) = Tuple4<'a0, 'a1, 'a2, 'a3>(a0, a1, a2, a3)

        static member IsTupleSameType((a0, a1, a2, a3)) = Tuple4<'a>(a0, a1, a2, a3)

        static member Init(S4, f : int -> 'out) = f 0, f 1, f 2, f 3

        

        static member InitElement(S4, element) = element, element, element, element
        static member IsTuple((a0, a1, a2, a3, a4)) = Tuple5<'a0, 'a1, 'a2, 'a3, 'a4>(a0, a1, a2, a3, a4)

        static member IsTupleSameType((a0, a1, a2, a3, a4)) = Tuple5<'a>(a0, a1, a2, a3, a4)

        static member Init(S5, f : int -> 'out) = f 0, f 1, f 2, f 3, f 4

        

        static member InitElement(S5, element) = element, element, element, element, element
        static member IsTuple((a0, a1, a2, a3, a4, a5)) = Tuple6<'a0, 'a1, 'a2, 'a3, 'a4, 'a5>(a0, a1, a2, a3, a4, a5)

        static member IsTupleSameType((a0, a1, a2, a3, a4, a5)) = Tuple6<'a>(a0, a1, a2, a3, a4, a5)

        static member Init(S6, f : int -> 'out) = f 0, f 1, f 2, f 3, f 4, f 5

        

        static member InitElement(S6, element) = element, element, element, element, element, element
        static member IsTuple((a0, a1, a2, a3, a4, a5, a6)) = Tuple7<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6>(a0, a1, a2, a3, a4, a5, a6)

        static member IsTupleSameType((a0, a1, a2, a3, a4, a5, a6)) = Tuple7<'a>(a0, a1, a2, a3, a4, a5, a6)

        static member Init(S7, f : int -> 'out) = f 0, f 1, f 2, f 3, f 4, f 5, f 6

        

        static member InitElement(S7, element) = element, element, element, element, element, element, element
        static member IsTuple((a0, a1, a2, a3, a4, a5, a6, a7)) = Tuple8<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6, 'a7>(a0, a1, a2, a3, a4, a5, a6, a7)

        static member IsTupleSameType((a0, a1, a2, a3, a4, a5, a6, a7)) = Tuple8<'a>(a0, a1, a2, a3, a4, a5, a6, a7)

        static member Init(S8, f : int -> 'out) = f 0, f 1, f 2, f 3, f 4, f 5, f 6, f 7

        

        static member InitElement(S8, element) = element, element, element, element, element, element, element, element
        static member IsTuple((a0, a1, a2, a3, a4, a5, a6, a7, a8)) = Tuple9<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6, 'a7, 'a8>(a0, a1, a2, a3, a4, a5, a6, a7, a8)

        static member IsTupleSameType((a0, a1, a2, a3, a4, a5, a6, a7, a8)) = Tuple9<'a>(a0, a1, a2, a3, a4, a5, a6, a7, a8)

        static member Init(S9, f : int -> 'out) = f 0, f 1, f 2, f 3, f 4, f 5, f 6, f 7, f 8

        

        static member InitElement(S9, element) = element, element, element, element, element, element, element, element, element
        static member IsTuple((a0, a1, a2, a3, a4, a5, a6, a7, a8, a9)) = Tuple10<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6, 'a7, 'a8, 'a9>(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9)

        static member IsTupleSameType((a0, a1, a2, a3, a4, a5, a6, a7, a8, a9)) = Tuple10<'a>(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9)

        static member Init(S10, f : int -> 'out) = f 0, f 1, f 2, f 3, f 4, f 5, f 6, f 7, f 8, f 9

        

        static member InitElement(S10, element) = element, element, element, element, element, element, element, element, element, element
    let helper = Helper()

    let inline internal asTuple' (helper : 'helper, tuple : 'tuple) = 
        ((^helper or ^tuple) : (static member IsTuple : 'tuple -> 'wrappedTuple) tuple) 

    let inline internal asTupleSameType'(helper : 'helper, tuple : 'tuple) =
        ((^helper or ^tuple) : (static member IsTupleSameType : 'tuple -> 'wrappedTuple) tuple)

    let inline internal asTuncTuple'(helper : 'helper, tuple : 'tuple) =
        ((^helper or ^tuple) : (static member IsTupleSameType : 'tuple -> 'wrappedTuple) tuple)

    let inline internal isTuple(tuple : 'tuple) = 
        asTuple'(helper, tuple)

    let inline internal isTupleSameType(tuple : 'tuple) = 
        asTupleSameType'(helper, tuple)
