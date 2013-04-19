module MapTestTargets

open Solid
open SolidFS
type MapTestNotAvailableException<'key when 'key : comparison>(m : MapTestTarget<'key>) = 
    inherit System.Exception()

and [<AbstractClass>] MapTestTarget<'key when 'key : comparison>() as this= 
    let not_available = MapTestNotAvailableException<'key>(this)
    abstract member Add : 'key -> 'key -> MapTestTarget<'key>
    abstract member Drop : 'key -> MapTestTarget<'key>
    abstract member Get : 'key -> 'key
    abstract member Set : 'key -> 'key -> MapTestTarget<'key>
    abstract member IsEmpty : bool
    abstract member Contains : 'key -> bool
    abstract member Name : string
    default __.Add _ _ = raise not_available
    default __.Drop _ = raise not_available
    default __.Get _ = raise not_available
    default __.Set _ _ = raise not_available
    default __.Contains _ = raise not_available

type internal SolidMapTestTarget<'key when 'key : comparison>(inner : HashMap<'key,'key>) = 
    inherit MapTestTarget<'key>()
    static let cns x = SolidMapTestTarget(x) :> MapTestTarget<'key>
    override __.Add k v = inner.Add(k,v) |> cns
    override __.Drop k = inner.Remove(k) |> cns
    override __.Get k = inner.[k]
    override __.Set k v = inner.Set(k,v) |> cns
    override __.Contains k = inner.ContainsKey(k)
    override __.IsEmpty = inner.Count = 0
    override __.Name = "Solid Map"
    static member FromSeq s = 
        let mutable o = HashMap<'key,'key>.Empty
        for i in s do
            o<-o.Add(i,i)
        o |> cns
type FSMapTestTarget<'key when 'key : comparison>(inner : Map<'key,'key>) = 
    inherit MapTestTarget<'key>()
    static let cns x = FSMapTestTarget(x) :> MapTestTarget<'key>
    override __.Add k v = inner.Add(k,v) |> cns
    override __.Drop k = inner.Remove(k) |> cns
    override __.Get k = inner.[k]
    override __.Set k v =inner.Add(k,v) |> cns
    override __.Contains k = inner.ContainsKey(k)
    override __.IsEmpty = inner.IsEmpty
    override __.Name = "FSharp Map"
    static member FromSeq s = 
        let mutable o= Map.empty
        for i in s do
            o <- o.Add(i,i)
        o |> cns

let test_add_many (keys:_ array) (o : MapTestTarget<_>) = 
    let mutable o = o
    for key in keys do
        o <- o.Add key key
    o

let test_get_many (keys:_ array) (o : MapTestTarget<_>) = 
    let mutable o=o
    for key in keys do
        o.Get(key) |> ignore
    o

let test_rem_many (keys:_ array) (o : MapTestTarget<_>) = 
    let mutable o=o
    for key in keys do
        o <- o.Drop(key)
    o

let test_contains_many (keys:_ array) (o : MapTestTarget<_>) = 
    let mutable o=o
    for key in keys do
        o.Contains(key) |> ignore
    o

type MapTest<'a when 'a : comparison> = 
    {
        Name : string
        Test : MapTestTarget<'a> -> MapTestTarget<'a>
    }

let Test_add_many keys  =
    {
        Name = "Test Add Many"
        Test = test_add_many keys
    }

let Test_rem_many keys = 
    {
        Name = "Test Drop Many"
        Test = test_rem_many keys
    }

let Test_contains_many keys = 
    {
        Name = "Test Contains Many"
        Test = test_contains_many keys
    }

let Test_get_many keys = 
    {
        Name = "Test Get Many"
        Test = test_contains_many keys
    }