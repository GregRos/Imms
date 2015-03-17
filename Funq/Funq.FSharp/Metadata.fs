namespace Funq.FSharp.Implementation
open System
open System.Collections.Generic
open System.Collections
open Funq.FSharp
open Funq
open Funq.FSharp.Implementation
open System.Runtime.CompilerServices
open System.Collections.Generic
open System.Text
type IDict<'k,'v> = IDictionary<'k,'v>


[<StructuredFormatDisplay("{AsString}")>]
type Meta(name : string, value : obj) = 
    member val Name = name
    member val Value = value
    member x.ValueType = value.GetType()
    override x.ToString() = sprintf "%s = %A" name value
    member x.AsString = x.ToString()

module Dict = 
    let(|Kvp|) (kvp : KeyValuePair<_,_>) = Kvp(kvp.Key,kvp.Value)
    let Kvp key value = KeyValuePair(key, value)
    let Kvp'(key,value) = KeyValuePair(key,value)
    let ofSeq (pairs : (_ * _) seq) = 
        let dict = Dictionary()
        pairs |> Seq.iter (fun (k,v) -> dict.Add(k,v))
        dict
    let values (dict : IDict<_,_>) = 
        dict |> Seq.map (fun (Kvp(k,v)) -> v)

    let clone (dict : IDict<_,_>) = 
        Dictionary(dict) :> IDict<_,_>
    let toSeq dict = dict |> Seq.map (fun (Kvp(k,v)) -> k,v)

type MetadataException(message : string) = inherit Exception(message)
and InvalidMetadataNameException private (message) = 
    inherit MetadataException(message)
    new (container : MetaContainer, name : string, ?expected_type : Type) = 
        let type_name = expected_type |> Option.map (fun t -> t.PrettyName())
        let all_fields = container.AsSeq |> Seq.print ", "
        let message = 
            match type_name with
            | Some type_name -> sprintf "A metadata field with the name '%s' (expected type '%s') wasn't found in this container. Existing field names: %s." name type_name all_fields
            | None -> sprintf "A metadata field with the name '%s' wasn't found in this container. Existing field names: %s." name all_fields
        InvalidMetadataNameException(message)
and InvalidMetadataTypeException private(message) = 
    inherit MetadataException(message)
    new (container : MetaContainer, name : string, expected_type : Type) = 
        let expected = expected_type.PrettyName()
        let real = container.Get(name).GetType().PrettyName()
        let message = sprintf "The metadata field '%s' was expected to have type '%s', but actually had type '%s'" name expected real
        InvalidMetadataTypeException(message)
and MetadataCollisionException(name) = 
    inherit MetadataException(sprintf "The container already has a metadata field with the name '%s'" name)
   
and Delete = Delete
and[<StructuredFormatDisplay("{AsString}")>]
 MetaContainer private (dict : IDictionary<string,Meta>) = 
    new (?metas : Meta seq) = MetaContainer(metas |> Option.orValue (Seq.empty) |> Seq.map (fun meta -> meta.Name, meta) |> Dict.ofSeq)

    member x.AsSeq = dict.Values
    member x.Add (meta : Meta) = 
        dict.[meta.Name] <- meta
    member x.AddNew (meta : Meta) = 
        if dict.ContainsKey meta.Name then
            raise <| MetadataCollisionException(meta.Name)
        else
            dict.[meta.Name] <- meta
    member x.Set name (value : 'v) = 
        x.Add <| Meta(name, value)
    member x.SetNew name (value : 'v) = 
        x.AddNew <| Meta(name, value)
    member x.Get name : 'v = 
        if dict.ContainsKey(name) |> not then
            raise <| InvalidMetadataNameException(x, name, typeof<'v>)
        let v = dict.[name].Value
        if not <| typeof<'v>.IsAssignableFrom(v.GetType()) then
            raise <| InvalidMetadataTypeException(x, name, typeof<'v>)
        v:?> 'v
    member x.Has name = 
        dict.ContainsKey name
    member x.TryGet name = if x.Has name then Some <| x.Get name else None
    member x.Drop name =
        if dict.Remove(name : string) then () else raise <| InvalidMetadataNameException(x,name)
    member x.Clone = 
        let container = MetaContainer()
        for meta in x.AsSeq do
            match meta.Value with 
            | :? MetaContainer as mContainer -> mContainer.Clone |> container.SetNew (meta.Name)
            | _ -> container.AddNew meta
        container
    member x.AddMany metas = 
        for meta in metas do
            x.Add meta
    member x.AddNewMany metas = 
        for meta in metas do
            x.AddNew meta
    member x.TryGetOr name alt = if x.Has name then x.Get name else alt
        
    member x.Merge (other : MetaContainer) = 
        MetaContainer(x.Clone.AsSeq |> Seq.append (other.Clone.AsSeq))
    override x.ToString() = 
        let strs = dict |> Dict.values |> Seq.toList |> List.map (fun m -> m.ToString())
        String.Join("; ", strs) |> sprintf "[%s]" 
    member x.AsString = x.ToString()
    static member Empty = MetaContainer()

[<AutoOpen>]
module MetaOps = 
    let (?) (container : MetaContainer) (metaName : string) = container.Get metaName
    let (?<-) (container : MetaContainer) (metaName : string) (value : 'v) =
        if typeof<'v> = typeof<Delete> then
            container.Drop metaName
        else
            container.Set metaName value
    let (++) (container1 : MetaContainer) (container2 : MetaContainer) = 
        let container12 = MetaContainer()
        for meta in Seq.append (container1.AsSeq) (container2.AsSeq) do
            container12.AddNew meta
        container12
    let (<+) (container : #MetaContainer) meta =
        container.AddNew meta
        container
[<ComplRep(ComplFlags.ModuleSuffix)>]
module Meta = 
    let toPairs (l : MetaContainer) = l.AsSeq |> Seq.map (fun meta -> meta.Name, meta.Value)
    let ofList (l : Meta list) = MetaContainer(l)
    let ofSeq (l : Meta seq) = MetaContainer(l)
    let ofSeqPairs (l : (_ * _) seq) = l |> Seq.map (fun (a,b) -> Meta(a, b)) |> ofSeq
    let ( <++ ) (a : _ list) (b : _ seq) = a @ (b |> List.ofSeq)
    let ( ++> ) (b : _ seq) (a : _ list) = (b |> List.ofSeq) @ a
    let toMap (container : MetaContainer) = 
        container |> toPairs |> Map.ofSeq

    let get name (container : MetaContainer) = container.Get name
    let tryGet name (container : MetaContainer) = container.TryGet(name) 
    let tryGetOr name alt (container : MetaContainer) = container.TryGetOr name alt
    let copyMetadata (source : MetaContainer) (target : 's :> MetaContainer) = 
        target.AddMany(source.AsSeq)
        target


        

