namespace Funq.FSharp.Implementation
open System
open System.Runtime.CompilerServices
#nowarn"0044" //Hide 'obsolete' warning

///Contains representations of various static, strongly-typed literal values (such as integers) in the type system.
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
    module Extensions = 
        type IStaticInt with
            member x.Value = x.Value

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Tuple = 
    [<Literal>]
    let private warning = "This is an infrastructure element. It should not be visible or used by user code."
    let private indexOutOfRange name value = invalidArg name (sprintf "The index value %d is out of range for this tuple." value) 

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple2<'a0, 'a1>(a0 : 'a0, a1 : 'a1) = 
        member x.StaticLength() = S2
        member x.Last() = a1
        member x.Rev() = a1, a0
        member x.Item S0 = a0
        member x.Item S1 = a1
        member x.Zip (b0, b1) = (a0, b0), (a1, b1)
        member x.Set(S0,b0) = (b0,a1)
        member x.Set(S1,b1) = (a0,b1)
        member x.Set(S2,b2) = (a0,a1,b2)
        member x.Cons b0 = (b0, a0, a1)
        member x.Conj b2 = (a0, a1, b2)

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple2SameType<'a>(a0 : 'a, a1 : 'a) = 
        member x.ToList() = [a0; a1]
        member x.ToArray() = [|a0; a1|]
        member x.Map f = f a0, f a1
        member x.Fold initial f = f a0 initial |> f a1
        member x.Reduce f = f a1 a0
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | _ -> indexOutOfRange "index" index
            
    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple3<'a0, 'a1, 'a2>(a0 : 'a0, a1 : 'a1, a2 : 'a2) = 
        member x.StaticLength() = S3
        member x.Last() = a2
        member x.Rev() = a2, a1, a0
        member x.Item S0 = a0
        member x.Item S1 = a1
        member x.Item S2 = a2
        member x.Map (f0, f1, f2) = f0 a0, f1 a1, f2 a2
        member x.Zip (b0, b1, b2) = (a0, b0), (a1, b1), (a2, b2)
        member x.Set(S0,b0) = (b0,a1, a2)
        member x.Set(S1,b1) = (a0,b1, a2)
        member x.Set(S2,b2) = (a0,a1,b2)
        member x.Set(S3,b3) = (a0,a1,a2,b3)
        member x.Fold initial (f0, f1, f2) = f0 a0 initial |> f1 a1 |> f2 a2
        member x.Reduce (f0, f1) = f0 a1 a0 |> f1 a2
        member x.Cons b0 = (b0, a0, a1, a2)
        member x.Conj b3 = (a0, a1, a2, b3)
        member x.Initial() = (a0, a1)
        member x.Tail() = (a1, a2)

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple3SameType<'a>(a0 : 'a, a1 : 'a, a2 : 'a) = 
        member x.ToList() = [a0; a1; a2]
        member x.ToArray() = [|a0; a1; a2|]
        member x.Map f = f a0, f a1, f a2
        member x.Fold initial f = f a0 initial |> f a1 |> f a2
        member x.Reduce f = f a1 a0 |> f a2
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | _ -> indexOutOfRange "index" index

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple4<'a0, 'a1, 'a2, 'a3>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3) = 
        member x.StaticLength() = S4
        member x.Last() = a3
        member x.Rev() = a3, a2, a1, a0
        member x.Item S0 = a0
        member x.Item S1 = a1
        member x.Item S2 = a2
        member x.Item S3 = a3
        member x.Map (f0, f1, f2, f3) = f0 a0, f1 a1, f2 a2, f3 a3
        member x.Zip (b0, b1, b2, b3) = (a0, b0), (a1, b1), (a2, b2), (a3, b3)
        member x.Set(S0,b0) = (b0,a1, a2, a3)
        member x.Set(S1,b1) = (a0,b1, a2, a3)
        member x.Set(S2,b2) = (a0,a1,b2, a3)
        member x.Set(S3,b3) = (a0,a1,a2,b3)
        member x.Set(S4,b4) = (a0,a1,a2,a3,b4)
        member x.Fold initial (f0, f1, f2, f3) = f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3
        member x.Reduce (f0, f1, f2) = f0 a1 a0 |> f1 a2 |> f2 a3
        member x.Cons b0 = (b0, a0, a1, a2, a3)
        member x.Conj b4 = (a0, a1, a2, a3, b4)
        member x.Initial() = (a0, a1, a2)
        member x.Tail() = (a1, a2, a3)

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple4SameType<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a) = 
        member x.ToList() = [a0; a1; a2; a3]
        member x.ToArray() = [|a0; a1; a2; a3|]
        member x.Map f = f a0, f a1, f a2, f a3
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3
        member x.Reduce f = f a1 a0 |> f a2 |> f a3
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | _ -> indexOutOfRange "index" index

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple5<'a0, 'a1, 'a2, 'a3, 'a4>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4) = 
        member x.StaticLength() = S5
        member x.Last() = a4
        member x.Rev() = a4, a3, a2, a1, a0
        member x.Item S0 = a0
        member x.Item S1 = a1
        member x.Item S2 = a2
        member x.Item S3 = a3
        member x.Item S4 = a4
        member x.Map (f0, f1, f2, f3, f4) = f0 a0, f1 a1, f2 a2, f3 a3, f4 a4
        member x.Zip (b0, b1, b2, b3, b4) = (a0, b0), (a1, b1), (a2, b2), (a3, b3), (a4, b4)
        member x.Set(S0,b0) = (b0,a1, a2, a3, a4)
        member x.Set(S1,b1) = (a0,b1, a2, a3, a4)
        member x.Set(S2,b2) = (a0,a1,b2, a3, a4)
        member x.Set(S3,b3) = (a0,a1,a2,b3, a4)
        member x.Set(S4,b4) = (a0,a1,a2,a3,b4)
        member x.Set(S5,b5) = (a0,a1,a2,a3,a4, b5)
        member x.Fold initial (f0, f1, f2, f3, f4) = f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4
        member x.Reduce (f0, f1, f2, f3) = f0 a1 a0 |> f1 a2 |> f2 a3 |> f3 a4
        member x.Cons b0 = (b0, a0, a1, a2, a3, a4)
        member x.Conj b5 = (a0, a1, a2, a3, a4, b5)
        member x.Initial() = (a0, a1, a2, a3)
        member x.Tail() = (a1, a2, a3, a4)

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple5SameType<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a) = 
        member x.ToList() = [a0; a1; a2; a3; a4]
        member x.ToArray() = [|a0; a1; a2; a3; a4|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4
        member x.Reduce f = f a1 a0 |> f a2 |> f a3 |> f a4
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | _ -> indexOutOfRange "index" index

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple6<'a0, 'a1, 'a2, 'a3, 'a4, 'a5>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5) = 
        member x.StaticLength() = S6
        member x.Last() = a5
        member x.Rev() = a5, a4, a3, a2, a1, a0
        member x.Item S0 = a0
        member x.Item S1 = a1
        member x.Item S2 = a2
        member x.Item S3 = a3
        member x.Item S4 = a4
        member x.Item S5 = a5
        member x.Map (f0, f1, f2, f3, f4, f5) = f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5
        member x.Zip (b0, b1, b2, b3, b4, b5) = (a0, b0), (a1, b1), (a2, b2), (a3, b3), (a4, b4), (a5, b5)
        member x.Set(S0,b0) = (b0,a1, a2, a3, a4, a5)
        member x.Set(S1,b1) = (a0,b1, a2, a3, a4, a5)
        member x.Set(S2,b2) = (a0,a1,b2, a3, a4, a5)
        member x.Set(S3,b3) = (a0,a1,a2,b3, a4, a5)
        member x.Set(S4,b4) = (a0,a1,a2,a3,b4, a5)
        member x.Set(S5,b5) = (a0,a1,a2,a3,a4, b5)
        member x.Set(S6,b6) = (a0,a1,a2,a3,a4, a5,b6)
        member x.Fold initial (f0, f1, f2, f3, f4, f5) = f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5
        member x.Reduce (f0, f1, f2, f3, f4) = f0 a1 a0 |> f1 a2 |> f2 a3 |> f3 a4 |> f4 a5
        member x.Cons b0 = (b0, a0, a1, a2, a3, a4, a5)
        member x.Conj b6 = (a0, a1, a2, a3, a4, a5, b6)
        member x.Initial() = (a0, a1, a2, a3, a4)
        member x.Tail() = (a1, a2, a3, a4, a5)

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple6SameType<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a) = 
        member x.ToList() = [a0; a1; a2; a3; a4; a5]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5
        member x.Reduce f = f a1 a0 |> f a2 |> f a3 |> f a4 |> f a5
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | _ -> indexOutOfRange "index" index
    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple7<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5, a6 : 'a6) = 
        member x.StaticLength() = S7
        member x.Last() = a6
        member x.Rev() = a6, a5, a4, a3, a2, a1, a0
        member x.Item S0 = a0
        member x.Item S1 = a1
        member x.Item S2 = a2
        member x.Item S3 = a3
        member x.Item S4 = a4
        member x.Item S5 = a5
        member x.Item S6 = a6
        member x.Map (f0, f1, f2, f3, f4, f5, f6) = f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5, f6 a6
        member x.Zip (b0, b1, b2, b3, b4, b5, b6) = (a0, b0), (a1, b1), (a2, b2), (a3, b3), (a4, b4), (a5, b5), (a6, b6)
        member x.Set(S0,b0) = (b0,a1, a2, a3, a4, a5, a6)
        member x.Set(S1,b1) = (a0,b1, a2, a3, a4, a5, a6)
        member x.Set(S2,b2) = (a0,a1,b2, a3, a4, a5, a6)
        member x.Set(S3,b3) = (a0,a1,a2,b3, a4, a5, a6)
        member x.Set(S4,b4) = (a0,a1,a2,a3,b4, a5, a6)
        member x.Set(S5,b5) = (a0,a1,a2,a3,a4, b5, a6)
        member x.Set(S6,b6) = (a0,a1,a2,a3,a4, a5,b6)
        member x.Set(S7,b7) = (a0,a1,a2,a3,a4, a5,a6, b7)
        member x.Fold initial (f0, f1, f2, f3, f4, f5, f6) = f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6
        member x.Reduce (f0, f1, f2, f3, f4, f5) = f0 a1 a0 |> f1 a2 |> f2 a3 |> f3 a4 |> f4 a5 |> f5 a6
        member x.Cons b0 = (b0, a0, a1, a2, a3, a4, a5, a6)
        member x.Conj b7 = (a0, a1, a2, a3, a4, a5, a6, b7)
        member x.Initial() = (a0, a1, a2, a3, a4, a5)
        member x.Tail() = (a1, a2, a3, a4, a5, a6)

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple7SameType<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a, a6 : 'a) = 
        member x.ToList() = [a0; a1; a2; a3; a4; a5; a6]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5; a6|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5, f a6
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6
        member x.Reduce f = f a1 a0 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | 6 -> a6
            | _ -> indexOutOfRange "index" index

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple8<'a0, 'a1, 'a2, 'a3, 'a4, 'a5, 'a6, 'a7>(a0 : 'a0, a1 : 'a1, a2 : 'a2, a3 : 'a3, a4 : 'a4, a5 : 'a5, a6 : 'a6, a7 : 'a7) = 
        member x.StaticLength() = S8
        member x.Last() = a7
        member x.Rev() = a7, a6, a5, a4, a3, a2, a1, a0
        member x.Item S0 = a0
        member x.Item S1 = a1
        member x.Item S2 = a2
        member x.Item S3 = a3
        member x.Item S4 = a4
        member x.Item S5 = a5
        member x.Item S6 = a6
        member x.Item S7 = a7
        member x.Map (f0, f1, f2, f3, f4, f5, f6, f7) = f0 a0, f1 a1, f2 a2, f3 a3, f4 a4, f5 a5, f6 a6, f7 a7
        member x.Zip (b0, b1, b2, b3, b4, b5, b6, b7) = (a0, b0), (a1, b1), (a2, b2), (a3, b3), (a4, b4), (a5, b5), (a6, b6), (a7, b7)
        member x.Set(S0,b0) = (b0,a1, a2, a3, a4, a5, a6, a7)
        member x.Set(S1,b1) = (a0,b1, a2, a3, a4, a5, a6, a7)
        member x.Set(S2,b2) = (a0,a1,b2, a3, a4, a5, a6, a7)
        member x.Set(S3,b3) = (a0,a1,a2,b3, a4, a5, a6, a7)
        member x.Set(S4,b4) = (a0,a1,a2,a3,b4, a5, a6, a7)
        member x.Set(S5,b5) = (a0,a1,a2,a3,a4, b5, a6, a7)
        member x.Set(S6,b6) = (a0,a1,a2,a3,a4, a5,b6, a7)
        member x.Set(S8,b8) = (a0,a1,a2,a3,a4, a5,a6, a7, b8)
        member x.Fold initial (f0, f1, f2, f3, f4, f5, f6, f7) = f0 a0 initial |> f1 a1 |> f2 a2 |> f3 a3 |> f4 a4 |> f5 a5 |> f6 a6 |> f7 a7
        member x.Reduce (f0, f1, f2, f3, f4, f5, f6) = f0 a1 a0 |> f1 a2 |> f2 a3 |> f3 a4 |> f4 a5 |> f5 a6 |> f6 a7
        member x.Cons b0 = (b0, a0, a1, a2, a3, a4, a5, a6, a7)
        member x.Conj b8 = (a0, a1, a2, a3, a4, a5, a6, a7, b8)
        member x.Initial() = (a0, a1, a2, a3, a4, a5, a6)
        member x.Tail() = (a1, a2, a3, a4, a5, a6, a7)

    [<Obsolete(warning)>]
    [<Struct>]
    type Tuple8SameType<'a>(a0 : 'a, a1 : 'a, a2 : 'a, a3 : 'a, a4 : 'a, a5 : 'a, a6 : 'a, a7 : 'a) = 
        member x.ToList() = [a0; a1; a2; a3; a4; a5; a6; a7]
        member x.ToArray() = [|a0; a1; a2; a3; a4; a5; a6; a7|]
        member x.Map f = f a0, f a1, f a2, f a3, f a4, f a5, f a6, f
        member x.Fold initial f = f a0 initial |> f a1 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6
        member x.Reduce f = f a1 a0 |> f a2 |> f a3 |> f a4 |> f a5 |> f a6
        member x.Nth index = 
            match index with
            | 0 -> a0
            | 1 -> a1
            | 2 -> a2
            | 3 -> a3
            | 4 -> a4
            | 5 -> a5
            | 6 -> a6
            | _ -> indexOutOfRange "index" index

    [<Obsolete(warning)>]
    type Helper internal() = 
        static member Init (count : S2,f : int -> 'element) = f 0, f 1
        static member Init (count : S3, f : int -> 'element) = f 0, f 1, f 2
        static member Init (count : S4, f : int -> 'element) = f 0, f 1, f 2, f 3
        static member Init (count : S5, f : int -> 'element) = f 0, f 1, f 2, f 3, f 4
        static member Init (count : S6, f : int -> 'element) = f 0, f 1, f 2, f 3, f 4, f 5
        static member Init (count : S7, f : int -> 'element) = f 0, f 1, f 2, f 3, f 4, f 5, f 6
        static member Init (count : S8, f : int -> 'element) = f 0, f 1, f 2, f 3, f 4, f 5, f 6, f 8
        static member Init (count : S9, f : int -> 'element) = f 0, f 1, f 2, f 3, f 4, f 5, f 6, f 8, f 9
        static member Init (count : S10, f : int -> 'element) = f 0, f 1, f 2, f 3, f 4, f 5, f 6, f 8, f 9, f 10
            
        static member InitElement (count : S2,element) = element, element
        static member InitElement (count : S3, element) = element, element, element
        static member InitElement (count : S4, element) = element, element, element, element
        static member InitElement (count : S5, element) = element, element, element, element, element
        static member InitElement (count : S6, element) = element, element, element, element, element, element
        static member InitElement (count : S7, element) = element, element, element, element, element, element, element
        static member InitElement (count : S8, element) = element, element, element, element, element, element, element, element
        static member InitElement (count : S9, element) = element, element, element, element, element, element, element, element, element
        static member InitElement (count : S10, element) = element, element, element, element, element, element, element, element, element, element
            
        static member AsTuple ((a0, a1)) = Tuple2(a0, a1) 
        static member AsTuple ((a0, a1, a2)) = Tuple3(a0,a1,a2) 
        static member AsTuple ((a0, a1, a2, a3)) = Tuple4(a0,a1,a2,a3) 
        static member AsTuple ((a0, a1, a2, a3,a4)) = Tuple5(a0,a1,a2,a3,a4)  
        static member AsTuple ((a0, a1, a2, a3,a4, a5)) = Tuple6(a0,a1,a2,a3,a4,a5)  
        static member AsTuple ((a0, a1, a2, a3,a4, a5, a6)) = Tuple7(a0,a1,a2,a3,a4,a5,a6)  
        static member AsTuple ((a0, a1, a2, a3,a4, a5, a6, a7)) = Tuple8(a0,a1,a2,a3,a4,a5,a6, a7)  

        static member AsTupleSameType ((a0, a1)) = Tuple2SameType(a0, a1) 
        static member AsTupleSameType ((a0, a1, a2)) = Tuple3SameType(a0,a1,a2) 
        static member AsTupleSameType ((a0, a1, a2, a3)) = Tuple4SameType(a0,a1,a2,a3) 
        static member AsTupleSameType ((a0, a1, a2, a3,a4)) = Tuple5SameType(a0,a1,a2,a3,a4)
        static member AsTupleSameType ((a0, a1, a2, a3,a4,a5)) = Tuple6SameType(a0,a1,a2,a3,a4,a5)
        static member AsTupleSameType ((a0, a1, a2, a3,a4,a5,a6)) = Tuple7SameType(a0,a1,a2,a3,a4,a5,a6)
        static member AsTupleSameType ((a0, a1, a2, a3,a4,a5,a6,a7)) = Tuple8SameType(a0,a1,a2,a3,a4,a5,a6,a7)
        
    // ==================================================================
    //                     HELPER AUXILIARY FUNCTIONS               
    // ==================================================================

    [<Obsolete(warning)>]
    let helper = Helper()

    [<Obsolete(warning)>]
    let inline private asTuple' (helper : 'helper, tuple : 'tuple) = 
        ((^helper or ^tuple) : (static member AsTuple : 'tuple -> 'wrappedTuple) tuple) 

    [<Obsolete(warning)>]
    let inline private asTupleSameType'(helper : 'helper, tuple : 'tuple) =
        ((^helper or ^tuple) : (static member AsTupleSameType : 'tuple -> 'wrappedTuple) tuple)

    let inline private asTuple(tuple : 'tuple) = 
        asTuple'(helper, tuple)

    let inline private asTupleSameType(tuple : 'tuple) = 
        asTupleSameType'(helper, tuple)
                
    //    ==================================================================
    //    ======              TUPLE GENERATORS                        ======
    //    ==================================================================

    let inline private initAux(helper : 'helper, count : 'count :> IStaticInt, f : int -> 'element) = 
        ((^helper or ^count) : (static member Init : 'count * (int -> 'element) -> 'tuple) count, f)

    let inline private initElementAux(helper : 'helper, count : 'count :> IStaticInt, element : 'element) =
        ((^helper or ^count) : (static member InitElement : 'count * 'element -> 'tuple) count, element)

    //    ==================================================================
    //                     FOR TUPLES WITH SAME ELEMENT TYPES         
    //    ==================================================================

    let inline private nthAux (tuple : 'tuple, index : int) : 'element = 
        (^tuple : (member Nth : int -> 'element) tuple, index)

    let inline private toListAux (tuple : 'tuple) : 'element list = 
        (^tuple : (member ToList : unit -> 'element list) tuple)

    let inline private foldAux (tuple : 'tuple, initial, f) =
        (^tuple : (member Fold : 'state -> ('element -> 'state -> 'state) -> 'state) tuple, initial, f)

    let inline private reduceAux (tuple : 'tuple, f) = 
        (^tuple : (member Reduce : ('element -> 'element -> 'element) -> 'element) tuple, f)

    let inline private mapAux f (x : 'inTuple) = 
        (^inTuple : (member Map : ('inElement -> 'outElement) -> 'outTuple) x, f)

    let inline private toArrayAux(inTuple : 'inTuple) : 'element array = 
        (^inTuple : (member ToArray : unit -> 'element array) inTuple)

    //    ==================================================================
    //    ======         FOR GENERAL TUPLE OBJECTS OF ANY TYPE        ======
    //    ==================================================================

    let inline private staticLengthAux (tuple : 'tuple) : 'length :> IStaticInt = 
        (^tuple : (member StaticLength : unit -> 'length) tuple)

    let inline private initialAux (inTuple : 'inTuple) : 'initialTuple =
        (^inTuple: (member Initial : unit -> 'initialTuple) inTuple)

    let inline private tailAux (inTuple : 'inTuple) : 'tailTuple =
        (^inTuple : (member Tail : unit -> 'tailTuple) inTuple)

    let inline private lastAux (tuple : 'tuple) : 'last =
        (^tuple : (member Last : unit -> 'last) tuple)

    let inline private itemAux (tuple : 'tuple, index : 'index) : 'item1 =
        (^tuple : (member Item : 'index -> 'item1) tuple, index)

    let inline private revAux (tuple : 'tuple) : 'revTuple = 
        (^tuple: (member Rev :  unit -> 'revTuple) tuple)

    let inline private zipAux(tuple1 : 'tuple1, tuple2 : 'tuple2) =
        (^tuple1  : (member Zip : 'tuple2 -> 'out) tuple1, tuple2)
    
    let inline private consAux(first : 'first, inTuple : 'inTuple) : 'outTuple =
        (^inTuple : (member Cons : 'first -> 'outTuple) inTuple, first)

    let inline private conjAux(inTuple : 'inTuple, last : 'last) : 'outTuple =
        (^inTuple : (member Conj : 'last -> 'outTuple) inTuple,last)
        
    let inline private setAux(inTuple : 'inTuple, index : 'index :> IStaticInt, newK : 'newK) =
        (^inTuple: (member Set : 'index * 'newK -> 'outTuple) inTuple, index, newK)

    // ==================================================================
    // ======                 MODULE BINDINGS                      ======
    // ==================================================================

    //    ==================================================================
    //    ======         FOR GENERAL TUPLE OBJECTS OF ANY TYPE        ======
    //    ==================================================================

    ///Returns the statically determined length of the tuple, encoded as one of the strongly-typed numeric symbols, S{N} (from S0 to S10)
    let inline staticLength (tuple : '``(item1 * ... * itemN)``) : '``S{N}`` :> IStaticInt = staticLengthAux(asTuple tuple)
        
    ///Returns the length of the specified tuple, as a 32-bit integer value.
    let inline length (tuple : '``(item1 * ... * itemN)``) : int = staticLength(tuple).Value   

    ///Returns a tuple consisting of the input tuple, without the last element. Tuple must be of size 3 or higher.
    let inline initial (tuple : '``(item1 * ... * itemN)``) : '``(item1 * ... * itemN-1)`` = initialAux(asTuple tuple)

    ///Returns a tuple consisting of the input tuple, without the first element. Tuple must be of size 3 or higher.
    let inline tail (tuple : '``(item1 * ... * itemN)``) : '``(item2 * ... * itemN)`` = tailAux(asTuple tuple)

    ///Returns the first element in the specified tuple. Identical to 'item0'.
    let inline first (tuple : '``(item1 * ... * itemN)`` ) : 'item1 = itemAux(asTuple tuple,S0)

    ///Returns the last element in the specified tuple.
    let inline last (tuple : '``(item1 * ... * itemN)``) : 'itemN = lastAux(asTuple tuple)

    ///Returns the element at position 'k' in the tuple, where 'k' is a strongly-typed numeric symbol, S{N} (from S0 to S10). 
    ///If the tuple has no such element, a compilation error is produced (overload resolution failure). This method works on tuples with differently typed elements.
    let inline item (k : '``S{k}`` :> IStaticInt) (tuple : '``(item1 * ... * itemK * ... * itemN)`` ) : 'itemK  = itemAux(asTuple tuple, k)

    ///Returns the 1st element in the specified tuple. Identical to 'first'.
    let inline item0 (tuple : '``(item1 * ... * itemN)`` ) : 'item0 = tuple |> item S0

    ///Returns the 2nd element in the specified tuple. The tuple must of size 2 or higher.
    let inline item1 (tuple : '``(item1 * ... * itemN)`` ) : 'item1 = tuple |> item S1

    ///Returns the 3rd element in the specified tuple. The tuple must of size 3 or higher.
    let inline item2 (tuple : '``(item1 * ... * itemN)`` ) : 'item2 = tuple |> item S2

    ///Returns the 4th element in the specified tuple. The tuple must of size 4 or higher.
    let inline item3 (tuple : '``(item1 * ... * itemN)`` ) : 'item3 = tuple |> item S3

    ///Returns the 5th element in the specified tuple. The tuple must of size 5 or higher.
    let inline item4 (tuple : '``(item1 * ... * itemN)`` ) : 'item4 = tuple |> item S4

    ///Returns the 6th element in the specified tuple. The tuple must of size 6 or higher.
    let inline item5 (tuple : '``(item1 * ... * itemN)`` ) : 'item5 = tuple |> item S5

    ///Returns the 7th element in the specified tuple. The tuple must of size 7 or higher.
    let inline item6 (tuple : '``(item1 * ... * itemN)`` ) : 'item6 = tuple |> item S6

    ///Sets the item at the index 'k' to the value 'newK', returning a tuple of the results.
    ///gdg
    let inline set (k : 'index :> IStaticInt) (newK : 'newK) (tuple : '``(item1 * ... * itemK * ... * itemN)``) : '``(item1 * ... * newK * ... * itemN)`` =
        setAux(asTuple tuple, k, newK)

    ///Returns the specified tuple, with the 1st element set to the given value.
    let inline set0 (new0 : 'new0) (tuple : '``(item1 * ... * itemN)``) : '``(new1 * ... * itemN)`` = tuple |> set S0 new0

    ///Returns the specified tuple, with the 2nd element set to the given value. 
    let inline set1 (new1 : 'new1) (tuple : '``(item1 * ... * itemN)``) : '``(item1 * new2 * ... * itemN)`` = tuple |> set S1 new1

    ///Returns the specified tuple, with the 3rd element set to the given value, or adds a new value if the size is 2.
    let inline set2 (new2 : 'new2) (tuple : '``(item1 * ... * item3 * ... * itemN)``) : '``(item1 * ... new3 * ... * itemN)`` = tuple |> set S2 new2

    ///Returns the specified tuple, with the 4th element set to the given value, or adds a new value if the size is 3.
    let inline set3 (new3 : 'new3) (tuple : '``(item1 * ... * item4 * ... * itemN)``) : '``(item1 * ... * new4 * ... * itemN)`` = tuple |> set S3 new3

    ///Returns the specified tuple, with the 5th element set to the given value, or adds a new value if the size is 4.
    let inline set4 (new4 : 'new4) (tuple : '``(item1 * ... * item5 * ... * itemN)``) : '``(item1 * ... * new5 * ... * itemN)`` = tuple |> set S4 new4

    ///Returns the specified tuple, with the 6th element set to the given value, or adds a new value if the size is 5.
    let inline set5 (new5 : 'new5) (tuple : '``(item1 * ... * item6 * ... * itemN)``) : '``(item1 * ... * new6 * ... * itemN)`` = tuple |> set S5 new5

    ///Returns the specified tuple, with the 7th element set to the given value, or adds a new value if the size is 6.
    let inline set6 (new6 : 'new6) (tuple : '``(item1 * ... * item7 * ... * itemN)``) : '``(item1 * ... * new7 * ... * itemN)`` = tuple |> set S6 new6

    let inline mapItem (k : 'index :> IStaticInt) (f : 'itemK -> 'newK) (tuple : '``(item1 * ... * itemK * ... * itemN)``) : '``(item1 * ... * newK * ... * itemN)`` =
        let item = tuple |> item k
        let result = tuple |> set k (f item)
        result

    ///Applies the function on the 1st element of the tuple, returning a new tuple as a result.
    let inline mapItem0 (f : 'item1 -> 'new1) (tuple : '``(item1 * ... * itemN)``) : '``(new1 * ... * itemN)`` = 
        tuple |> mapItem S0 f

    ///Applies the function on the 2nd element of the tuple, returning a new tuple as a result.
    let inline mapItem1 (f : 'item2 -> 'new2) (tuple : '``(item1 * item2 * ... itemN)``) : '``(item1 * new2 * ... * itemN)`` = 
        tuple |> mapItem S1 f

    ///Applies the function on the 3rd element of the tuple, returning a new tuple as a result.
    let inline mapItem2 (f : 'item3 -> 'new3) (tuple : '``(item1 * item2 * item3 * ... * itemN)``) : '``(item1 * item2 * new3 * ... * itemN)`` = 
        tuple |> mapItem S2 f

    ///Applies the function on the 4th element of the tuple, returning a new tuple as a result.
    let inline mapItem3 (f : 'item4 -> 'new4) (tuple : '``(item1 * ... * item4 * ... * itemN)``) : '``(item1 * ... * new4 * ... * itemN)`` = 
        tuple |> mapItem S3 f

    ///Applies the function on the 5th element of the tuple, returning a new tuple as a result.
    let inline mapItem4 (f : 'item5 -> 'new5) (tuple : '``(item1 * ... * item5 * ... * itemN)``) : '``(item1 * ... * new5 * ... * itemN)`` = 
        tuple |> mapItem S4 f

    ///Applies the function on the 6th element of the tuple, returning a new tuple as a result.
    let inline mapItem5 (f : 'item6 -> 'new6) (tuple : '``(item1 * ... * item6 * ... * itemN)``) : '``(item1 * ... * new6 * ... * itemN)`` = 
        tuple |> mapItem S5 f

    ///Applies the function on the 7th element of the tuple, returning a new tuple as a result.
    let inline mapITem5 (f : 'item7 -> 'new7) (tuple : '``(item1 * ... * item7 * ... * itemN)``) : '``(item1 * ... * new7 * ... * itemN)`` = 
        tuple |> mapItem S6 f

    ///Returns the specified tuple, with its elements in reverse order.
    let inline rev (tuple : '``(item1 * ... * itemN)``) : '``(itemN * ... * item1)`` = revAux(tuple)
     
    ///Zips two tuples of equal length together, returning a tuple consisting of their elements correlated by order.
    let inline zip (tuple2 : '``(b1 * ... * bN)``) (tuple1 : '``(a1 * ... * aN)``) = zipAux(asTuple tuple1, tuple2)

    ///Adds an element to the start of the tuple.
    let inline cons (head : 'head) (inTuple : '``(item1 * ... * itemN)``) : 'outTuple = consAux(head, asTuple inTuple) : 'outTuple

    ///Adds an element to the end of the tuple.
    let inline conj (last : 'last) (inTuple : '``(item1 * ... * itemN)``) : 'outTuple = conjAux(asTuple inTuple,last)
    
    //    ==================================================================
    //                     FOR TUPLES WITH SAME ELEMENT TYPES         
    //    ==================================================================

    ///Maps a function over a tuple type. All tuple elements must be of the same type.
    let inline mapAll (f : '``in`` -> 'out) (tuple : '``('in * ... * 'in)``) : '``('out * ... * 'out)`` = mapAux f (asTupleSameType tuple)

    ///Iterates a function over the elements of a tuple type. All tuple elements must be of the same type.
    let inline iter (f : 'element -> unit) (tuple : '``(element * ... * element)``) : unit = 
        tuple |> mapAll f |> ignore

    ///Returns the nth element in a tuple consisting of identically-typed elements, or throws an exception if the element doesn't exist.
    let inline get (index : int) (tuple : '``(element * ... * element)``) : 'element = nthAux(asTupleSameType tuple, index)

    ///Converts an identically typed tuple to a linked list.
    let inline toList (tuple : '``(element * ... * element)``) : 'element list = toListAux(asTupleSameType tuple)

    ///Folds over a tuple type. All tuple elements must be of the same type.
    let inline fold initial (f : 'state -> 'element -> 'state) (tuple : '``(element * ... * element)``) = foldAux(asTuple tuple, initial, fun e s -> f s e)

    ///Reduces over a tuple type. All elements must be of the same type.
    let inline reduce (f : 'element -> 'element -> 'element) (tuple : '``(state * element * ... * element)``) = 
        reduceAux(asTuple tuple, fun a b -> f b a)

    ///Takes as a parameter a tuple of functions, and invokes the functions with the two arguments, returning a tuple of the results.
    let inline apply2 (arg1 : 'arg1) (arg2 : 'arg2) (tuple : '``(item1 * ... * itemN)``) = tuple |> mapAll (fun f -> f arg1 arg2)

    ///Converts the tuple to an array.
    let inline toArray (tuple : '``(item1 * ... * itemN)``) : 'element array = toArrayAux(asTuple tuple)

    ///Converts the specified tuple to a sequence, iterating over its elements.
    let inline toSeq (tuple : '``(item1 * ... * itemN)``) : 'element seq = tuple |> toArray :> _ seq    

    //    ==================================================================
    //    ======              TUPLE GENERATORS                        ======
    //    ==================================================================

    let inline init (f : int -> 'element) (count : 'count :> IStaticInt) : '``(element * ... * element)`` = initAux(helper, count, f) 

    let inline initElement (element : 'element) (count : 'count :> IStaticInt) : '``(element * ... * element)`` = initElementAux(helper, count, element)

    let inline ofList (listLength : '``S{n}`` :> IStaticInt) (lst : 'element list) : '``(element * ... * element)`` =
        let arr = lst |> List.toArray
        init (fun i -> arr.[i]) listLength

    let inline ofArray (length : '``S{n}`` :> IStaticInt) (arr : 'element array) : '``(element * ... * element)`` =
        init (fun i -> arr.[i]) length

    ///Invokes the specified function twice, returning a tuple of the results.
    let invoke2 f = f(),f()

    ///Invokes the specified function 3 times, returning a tuple of the results.
    let invoke3 f = f(),f(),f()

    ///Invokes the specified function 4 times, returning a tuple of the results.
    let invoke4 f = f(),f(),f(),f()

    ///Invokes the specified function 5 times, returning a tuple of the results.
    let invoke5 f = f(),f(),f(),f(),f()

    ///Invokes the specified function 6 times, returning a tuple of the results.
    let invoke6 f = f(),f(),f(),f(),f(),f()

    ///Invokes the specified function 7 times, returning a tuple of the results.
    let invoke7 f = f(),f(),f(),f(),f(),f(),f()

    ///Invokes the specified function 8 times, returning a tuple of the results.
    let invoke8 f = f(),f(),f(),f(),f(),f(),f(),f()

    ///Invokes the specified function 9 times, returning a tuple of the results.
    let invoke9 f = f(),f(),f(),f(),f(),f(),f(),f(),f()

    ///Invokes the specified function 10 times, returning a tuple of the results.
    let invoke10 f = f(),f(),f(),f(),f(),f(),f(),f(),f(),f()

    type Tuple<'a, 'b> with
        member x.df = 5

    
