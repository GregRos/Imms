namespace Imms.Tests.Integrity
open System.Collections
open System.Collections.Generic
open System.Collections.Immutable
open Imms.FSharp.Implementation.Compatibility
open System
open Imms.Abstract
open Imms
open Imms.FSharp.Implementation
open Imms.FSharp
open ExtraFunctional
open System.Diagnostics
type CollisionFunc<'k, 'v> = ('k -> 'v -> 'v -> 'v)
type SubtractionFunc<'k, 'v> = ('k -> 'v -> 'v -> 'v option)
[<AbstractClass>]
type MapWrapper<'k when 'k : comparison>(name : string) = 
    inherit ColWrapper<KeyValuePair<'k,'k>>(name)
    interface IReadOnlyDictionary<'k,'k> with
        member x.GetEnumerator() = x.GetEnumerator()
        member x.GetEnumerator() : IEnumerator = x.GetEnumerator() :>_
        member x.Count = x.Length
        member x.get_Item k = x.Get k
        member x.ContainsKey k= x.ContainsKey k
        member x.Keys = x.Keys
        member x.Values = x.Values
        member x.TryGetValue(k, v) = 
            if x.ContainsKey k then
                v <- x.Get k
                true
            else
                false
    abstract Add : 'k * 'k -> MapWrapper<'k>
    abstract Get : 'k -> 'k
    abstract EmptySet : SetWrapper<'k>
    abstract TryGet : 'k -> 'k option
    abstract MapEquals : (Kvp<'k,'k> seq) -> bool
    abstract AddRange : (Kvp<'k,'k>) seq -> MapWrapper<'k>
    abstract SetRange : (Kvp<'k,'k>) seq -> MapWrapper<'k>
    abstract Set : 'k * 'k -> MapWrapper<'k>
    abstract Remove : 'k -> MapWrapper<'k>
    abstract ByOrder : int -> (Kvp<'k,'k>)
    abstract ContainsKey : 'k -> bool
    abstract MaxItem : 'k * 'k
    abstract MinItem : 'k * 'k
    abstract IsOrdered : bool
    abstract IsEmpty : bool
    abstract RemoveRange : seq<'k> -> MapWrapper<'k>
    abstract Merge : (Kvp<'k,'k> seq) * CollisionFunc<'k,'k> option -> MapWrapper<'k>
    abstract Join : (Kvp<'k,'k> seq) * CollisionFunc<'k,'k> option -> MapWrapper<'k>
    abstract Except : (Kvp<'k,'k> seq) * f:SubtractionFunc<'k,'k> option -> MapWrapper<'k>
    abstract Empty : MapWrapper<'k>
    abstract Difference : (Kvp<'k,'k> seq) -> MapWrapper<'k>
    member x.Keys = x |> Seq.map (Kvp.ToTuple >> fst)
    member x.Values = x |> Seq.map (Kvp.ToTuple >> snd)
    default x.ByOrder _ = raise <|  OperationNotImplemented("ByOrder")
    default x.MaxItem = raise <|  OperationNotImplemented("MaxItem")
    default x.MinItem = raise <|  OperationNotImplemented("MinItem")
    default x.Merge (_,_) = raise <|  OperationNotImplemented("Union")
    default x.TryGet k = if x.ContainsKey k then Some <| x.Get k else None
    default x.Join (_,_) = raise <|  OperationNotImplemented("Intersect")
    default x.Except(_,_) = raise <|  OperationNotImplemented("Except")
    default x.Difference _ = raise <|  OperationNotImplemented("Difference")
    default x.RemoveRange _ = raise <|  OperationNotImplemented("RemoveRange")
    override x.Equals other = 
        match other with
        | :? MapWrapper<'k> as other -> 
            let l1,l2 = x.Length,other.Length
            if l1 <> l2 then
                Debugger.Break()
            let eq1 = x.MapEquals other
            let eq2 = other.MapEquals x
            assert_eq(eq1, eq2)

            if x.IsOrdered && other.IsOrdered then
                eq1 && Seq.equalsWith (fun (a : Kvp<_,_>) (b : Kvp<_,_>) -> obj.Equals(a.Key,b.Key) && obj.Equals(a.Value,b.Value)) x other
            else eq1
        | _ -> false

    override x.GetHashCode() = failwith "Do not call this method"

type MapReferenceWrapper<'k when 'k : comparison>(Inner : Map<'k, 'k>, count : int) = 
    inherit MapWrapper<'k>("FSharpMap")
    static let wrap n x = MapReferenceWrapper<'k>(x, n) :> MapWrapper<'k>
    static let wrapL (x : Map<'k,'k>) = x |> wrap (x.Count)
    let min = lazy (Inner |> Seq.head |> Kvp.ToTuple)
    let max = lazy (Inner |> Seq.last |> Kvp.ToTuple)
    member val Inner = Inner
    override x.GetEnumerator() = (Inner :>_ seq).GetEnumerator()
    override x.Add(k,v) = 
        if x.ContainsKey k then raise <| Errors.Key_exists(k) else Inner.Add(k,v) |> wrap (count + 1)
    override x.Remove k = 
        if x.ContainsKey k then Inner.Remove(k) |> wrap (count - 1) else Inner.Remove(k) |> wrap count
    override x.ContainsKey k = Inner.ContainsKey(k)
    override x.EmptySet = ReferenceSetWrapper<'k>.FromSeq [] 
    override x.SetRange kvps =  
        let mutable mp = x.Inner
        for Kvp(item) in kvps do
            mp <- mp.Add item
        mp |> wrap (mp.Count)
    override x.MinItem = min.Value
    override x.MapEquals other = 
        let other = other |> x.Empty.AddRange
        if other.Length <> x.Length then false
        else 
            x |> Seq.forall (fun (Kvp(k,v)) -> obj.Equals(other.Get(k), v))
                
    override x.SelfTest() = true
    override x.IsOrdered = true
    override x.IsReference = true
    override x.MaxItem = max.Value
    override x.AddRange vs = 
        let mutable mp = x :> MapWrapper<_>
        for Kvp(k,v) in vs do
            mp <- mp.Add(k,v)
        mp
    override x.Empty = Map.empty |> wrap 0
    override x.Get k = Inner.[k]
    override x.Set(k,v) = 
        if x.ContainsKey k then Inner.Add(k, v) |> wrap (count) else Inner.Add(k,v) |> wrap (count + 1)

    override x.Merge (other, f) =
        let mutable working = x :> MapWrapper<_>
        let f = f.Or(fun k v1 v2 -> v2)
        for Kvp(k,v) in other do
            let mutable v = v
            let myV = working.TryGet k
            if working.ContainsKey k then
                v <- f k (myV.Value) v
            working <- working.Set(k, v)
        working

    override x.Except(other,f) =
        let f = f.Or(fun k v1 v2 -> None)
        let mutable working = x.Inner
        for Kvp(k,v) in other do
            let myVal = working.TryFind k
            if myVal.IsSome then
                let newVal = f k (myVal.Value) v
                if newVal.IsSome then
                    working <-  working.Add(k,newVal.Value)
                else
                    working <- working.Remove k
        working |> wrapL

    static member FromSeq s = s |> Seq.map (Kvp.ToTuple) |> Map.ofSeq |> wrapL

    override x.Join (other, f) =
        let mutable tally = x.Empty
        let f = f.Or (fun k v1 v2 -> v2)
        for Kvp(k,v) in other do
            let mutable v = v
            let mutable add = false
            if tally.ContainsKey k then
                v <- f k (tally.Get k) v
                add <- true
            elif x.ContainsKey k then
                v <- f k (x.Get k) v
                add <- true
            if add then
                tally <- tally.Set(k,v)
        tally

    override x.Difference other = x.Except(other, None).AddRange(MapReferenceWrapper.FromSeq(other).Except(x, None))
    override x.RemoveRange ks = 
        let mutable x = x.Inner
        for item in ks do
            x <- x.Remove item
        x |> wrapL
    override x.IsEmpty = Inner.IsEmpty
    override x.Length = count
    override x.ByOrder i = x |> Seq.nth i
    override x.All f = Inner |> Seq.forall f
    override x.Any f = Inner |> Seq.exists f
    override x.Count f = Inner |> Seq.fold (fun st cur -> if f cur then st + 1 else st) 0
    override x.Pick f = Inner |> Seq.tryPick f
    override x.Find f = Inner |> Seq.tryFind f
    override x.Fold initial f = Inner |> Seq.fold f initial
    override x.Reduce f = Inner |> Seq.reduce f
    
type ImmOrderedMapWrapper<'k when 'k : comparison>(Inner : ImmOrderedMap<'k, 'k>) =
    inherit MapWrapper<'k>("ImmOrderedMap")
    static let wrap x = ImmOrderedMapWrapper<'k>(x) :> MapWrapper<'k>
    let unwrap (y : Kvp<'k,'k> seq) = 
        match y with
        | :? ImmOrderedMapWrapper<'k> as map -> map.Inner :> _ seq
        | _ -> y
    let unwrapSet (y : 'k seq) =
        match y with
        | :? ImmOrderedSetWrapper<'k> as set -> set.Inner :> _ seq
        | _ -> y
    member val public Inner =
         Inner
    override x.GetEnumerator() = (Inner :>_ seq).GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap 
    override x.Remove k = Inner.Remove k |> wrap
    override x.ContainsKey k = Inner.ContainsKey(k)
    override x.AddRange vs = Inner.AddRange (vs |> unwrap) |> wrap
    override x.IsOrdered = true
    override x.SetRange vs= Inner.SetRange (vs |> unwrap) |> wrap
    override x.Empty = ImmOrderedMap.emptyWith (Cmp.Default) |> wrap
    override x.EmptySet = ImmOrderedSetWrapper<'k>.FromSeq []
    override x.MapEquals other = Inner.MapEquals(other)
    override x.Get k = Inner.[k]
    override x.SelfTest() = 
        let mutable i = 0
        let mutable okay = true
        let distinctKeyCount = x |> Seq.distinctBy (fun y -> y.Key) |> Seq.smartLength
        if distinctKeyCount <> x.Length then false
        else
            for item in x do
                let (Kvp(k,v)) = x.ByOrder i
                okay <- okay && obj.Equals(item.Key, k) && obj.Equals(item.Value, v)
                i <- i + 1
            okay
    override x.Set(k,v) = Inner.Set(k,v) |> wrap
    override x.Merge(other, f) =
        Inner.Merge(other |> unwrap, f.Map toFunc3 |> Option.asNull) |> wrap
    override x.Join(other, f) = Inner.Join(other |> unwrap, f.Map toFunc3 |> Option.asNull) |> wrap
    override x.Except(other,f) = Inner.Subtract(other |> unwrap, f.Map(Fun.compose3 toOption >> toFunc3).Or(null)) |> wrap
    override x.Difference other = Inner.Difference(other |> unwrap) |> wrap
    override x.RemoveRange vs = Inner.RemoveRange (vs |> unwrapSet) |> wrap
    override x.Length = Inner.Length
    override x.IsEmpty = Inner.IsEmpty
    override x.MinItem = Inner.MinItem.Key,Inner.MinItem.Value
    override x.MaxItem = Inner.MaxItem.Key, Inner.MaxItem.Value
    override x.ByOrder i = Inner.ByOrder(i)
    override x.All f = Inner.All f
    override x.Any f = Inner.Any f
    override x.Count f = Inner.Count f
    override x.Pick f = Inner |> ImmOrderedMap.pick f
    override x.Find f = Inner.Find f |> fromOption
    override x.Fold initial f = Inner |> ImmOrderedMap.fold initial f
    override x.Reduce f = Inner |> ImmOrderedMap.reduce f
    static member FromSeq s = 
        ImmOrderedMapWrapper(ImmOrderedMap.ofKvpSeqWith Cmp.Default s) :> MapWrapper<_>

type ImmMapWrapper<'k when 'k : comparison>(Inner : ImmMap<'k,'k>) =
    inherit MapWrapper<'k>("ImmMap")
    static let wrap x = ImmMapWrapper<'k>(x) :> MapWrapper<'k>
    static let unwrap (xs : Kvp<'k,'k> seq) =
        match xs with
        | :? ImmMapWrapper<'k> as wrapped -> wrapped.Inner :> _ seq
        | _ -> xs
    static let unwrapSet (xs : 'k seq) = 
        match xs with
        | :? ImmSetWrapper<'k> as wrapped -> wrapped.Inner :> _ seq
        | _ -> xs
    member val public Inner = Inner
    override x.GetEnumerator() = (Inner :> _ seq).GetEnumerator()
    override x.Add(k,v) = Inner.Add(k,v) |> wrap
    override x.Remove k = Inner.Remove k |> wrap
    override x.IsOrdered = false
    override x.EmptySet = ImmSetWrapper<'k>.FromSeq []
    override x.MapEquals other = Inner.MapEquals(other)
    override x.SetRange vs = Inner.SetRange (vs |> unwrap) |> wrap
    override x.AddRange vs = Inner.AddRange (vs |> unwrap) |> wrap
    override x.SelfTest() = 
        x |> Seq.distinctBy (fun y -> y.Key) |> Seq.smartLength = x.Length
    override x.ContainsKey k = Inner.ContainsKey(k)
    override x.Empty = ImmMap.empty |> wrap
    override x.Get k = Inner.[k]
    override x.Set(k,v) = Inner.Set(k,v) |> wrap
    override x.Merge(other, f) = Inner.Merge(other |> unwrap, f.Map toFunc3 |> Option.asNull) |> wrap
    override x.Join(other, f) = Inner.Join(other |> unwrap, f.Map toFunc3 |> Option.asNull) |> wrap
    override x.Except(other,f) = Inner.Subtract(other |> unwrap, f.Map(Fun.compose3 toOption >> toFunc3).Or(null)) |> wrap
    override x.Difference other = Inner.Difference(other |> unwrap) |> wrap
    override x.RemoveRange vs = 
        Inner.RemoveRange (vs |> unwrapSet) |> wrap
    override x.Length = Inner.Length
    override x.IsEmpty = Inner.IsEmpty
    override x.All f = Inner.All f
    override x.Any f = Inner.Any f
    override x.Count f = Inner.Count f
    override x.Pick f = Inner |> ImmMap.pick f
    override x.Find f = Inner.Find f |> fromOption
    override x.Fold initial f = Inner.Aggregate(initial, toFunc2 f)
    override x.Reduce f = Inner |> ImmMap.reduce f
    static member FromSeq s = ImmMapWrapper(ImmMap.ofKvpSeq(s))  :> MapWrapper<_>

type MapWrapper<'k when 'k : comparison> with
    member x.assert_keyExists k  =
        if not <| x.IsReference then
            assert_true(x.ContainsKey k)
            //assert_ex_arg(fun () -> x.Add(k,k))
            is_some(x.TryGet k)

    member x.assert_keyDoesntExist k  =
        if not <| x.IsReference then
            assert_false(x.ContainsKey k)
            is_none(x.TryGet k)
            assert_no_ex(fun () -> x.Add(k,k))
          //  assert_ex_key_not_found(fun () -> x.Get k)

    member x.U_get k  =
        is_some(x.TryGet k)
        assert_true(x.ContainsKey k)
        let v = x.Get k
        assert_eq(x.TryGet(k).Value, v)
        v

    member x.U_add k v  =
        let mutable next = x
        if x.ContainsKey k then
            //set |> assert_keyExists k
            next <- x.Set(k, v)
            assert_eq(x.Length, next.Length)
        else
           // set |> assert_keyDoesntExist k
            next <- x.Add(k, v)
            assert_eq(next.Length, x.Length + 1)
        assert_eq(next.U_get k, v)
        next

    member x.U_remove k  =
        let mutable next = x.Remove k
        if x.ContainsKey k then
            x.assert_keyExists k
            assert_eq(x.Length - 1, next.Length)
        else
            x.assert_keyDoesntExist k
            assert_eq(x.Length, next.Length)
        next.assert_keyDoesntExist k
        next

    member x.U_addRange kvps  =
        let shared = kvps |> Seq.countWhere (fun (Kvp(k,_)) -> x.ContainsKey k)
        let total = kvps |> Seq.smartLength
        let kvps = kvps |> Seq.canEnsureIterateOnce
        let next = 
            if shared > 0 then
                x.SetRange kvps
            else
                x.AddRange kvps

        assert_eq(next.Length, x.Length + total - shared)
        for Kvp(k,v) in kvps do
            assert_eq(next.U_get k, v)
        next

    member x.U_removeRange keys  =
        let shared = keys |> Seq.countWhere (fun k -> x.ContainsKey k)
        let next = x.RemoveRange(keys |> Seq.canEnsureIterateOnce)
        assert_eq(next.Length, x.Length - shared)
        next

    member x.U_except (f : _ option) other  =
        let shared = other |> Seq.keys |> Seq.countWhere (x.ContainsKey)
        let count = ref 0
        let result =  x.Except(other |> Seq.canEnsureIterateOnce, f.Map(countCalls_3 count))
        if f.IsSome then assert_eq(shared,!count)
        assert_eq(result.Length, shared)
        result

    member x.U_merge (f : _ option) other  =
        let shared = other |> Seq.countWhere (fun (Kvp(k,v)) -> x.ContainsKey k)
        let count = ref 0
        let result = x.Merge(other |> Seq.canEnsureIterateOnce,f.Map (countCalls_3 count))
        if f.IsSome then assert_eq(!count,shared)
        assert_eq(result.Length, shared)
        result

    member x.U_join (f : _ option) other  =
        let shared = other |> Seq.keys |> Seq.countWhere (x.ContainsKey)
        let count = ref 0 
        let result = x.Join(other |> Seq.canEnsureIterateOnce,f.Map (countCalls_3 count))
        if f.IsSome then assert_eq(!count, shared)
        assert_eq(result.Length, shared)
        result

    member x.U_difference other =
        let result = x.Difference(other |> Seq.canEnsureIterateOnce)
        result

    member x.toSet  =
        let set = x.EmptySet
        set.Union (x.Keys)
