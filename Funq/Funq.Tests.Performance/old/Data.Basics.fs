namespace Funq.Benchmarking.Data
open Funq.Benchmarking
type BasicCollection<'t> = 
    {
        Name : string
        Ctor : BasicCollection<'t> -> 't
        Count  : int
        Metadata : Meta list
    }
    interface IDataStruct<'t> with
        member x.Metadata =  x.Metadata
        member x.Object = Fun.memoize <@ x.Ctor x @>
        member x.Count = x.Count
        member x.Name = x.Name
module Basic = 
    let Array count (s : NamedSeq<_>) = 
        {
            Ctor = fun x -> s |> Seq.take(x.Count) |> Seq.toArray
            Count = count
            Metadata = s.Meta
            Name = "Array"
        }:>IDataStruct<_>

    let List count (s : NamedSeq<_>) = 
        {
            Ctor = fun x -> s |> Seq.take (x.Count) |> Seq.toList
            Count = count
            Metadata = s.Meta
            Name = "FSharp List"
        }:>IDataStruct<_>

