//+
[<AutoOpen>]
module Funq.FSharp.Implementation.Metadata
open System
open System.Collections.Generic
open System.Collections
open Funq.FSharp
open Funq
open Funq.FSharp.Implementation
open System.Runtime.CompilerServices
open System.Collections.Generic
open System.Text
open System.Reflection
type IDict<'k,'v> = IDictionary<'k,'v>
(* 
    This file contains metadata classes. 
*)


///Specifies that this property or field is part of the object's meta data, which should be printed in a report.
[<AttributeUsage(AttributeTargets.Property ||| AttributeTargets.Field)>]
type IsMetaAttribute() =
    inherit Attribute()

///An abstract metadata object which can be a concrete key-value pair
[<StructuredFormatDisplay("{AsString}"); AbstractClass>]
type AbstractMeta(name : string) = 
    member val Name = name
    abstract Value : obj with get,set
    abstract ValueType : Type
    member x.AsString = x.ToString()
    override x.ToString() = sprintf "%s = %A" x.Name x.Value

type Meta(name : string, value : obj) = 
    inherit AbstractMeta(name)
    let mutable value = value
    override x.Value 
        with get() = value
        and  set(v) = value <- v
    override x.ValueType = value.GetType()

type ReflectionMeta(property : PropertyInfo, instance : obj) =
    inherit AbstractMeta(property.Name)
    override x.ValueType = property.PropertyType
    override x.Value 
        with get() = property.GetValue(instance)
        and  set v = property.SetValue(instance, v)

module Dict = 
    let(|Kvp|) (kvp : KeyValuePair<_,_>) = Kvp(kvp.Key,kvp.Value)
    let Kvp key value = KeyValuePair(key, value)
    let Kvp'(key,value) = KeyValuePair(key,value)
    let empty<'a,'b> = Dictionary<'a,'b>(Eq.Default)
    let ofSeq (pairs : (_ * _) seq) = 
        let dict = Dictionary()
        pairs |> Seq.iter (fun (k,v) -> dict.Add(k,v))
        dict
    let values (dict : IDict<_,_>) = 
        dict |> Seq.map (fun (Kvp(k,v)) -> v)

    let clone (dict : IDict<_,_>) = 
        Dictionary(dict) :> IDict<_,_>
    let toSeq dict = dict |> Seq.map (fun (Kvp(k,v)) -> k,v)

(*

        x.GetType()
            .GetProperties()
            |> Seq.filter (fun x -> x.GetCustomAttribute<IsMetaAttribute>(true) <> Unchecked.defaultof<IsMetaAttribute>)
            |> Seq.map (fun prop -> ReflectionMeta(prop, x))
            |> Seq.iter (x.AddMeta)
*)

type MetadataException(message : string) = inherit Exception(message)
and InvalidMetadataNameException private (message) = 
    inherit MetadataException(message)
    new (container : MetaContainer, name : string, ?expected_type : Type) = 
        let type_name = expected_type |> Option.map (fun t -> t.PrettyName())
        let all_fields = container.AsMetaSeq |> Seq.print ", "
        let message = 
            match type_name with
            | Some type_name -> sprintf "A metadata field with the name '%s' (expected type '%s') wasn't found in this container. Existing field names: %s." name type_name all_fields
            | None -> sprintf "A metadata field with the name '%s' wasn't found in this container. Existing field names: %s." name all_fields
        InvalidMetadataNameException(message)
and InvalidMetadataTypeException private(message) = 
    inherit MetadataException(message)
    new (container : MetaContainer, name : string, expected_type : Type) = 
        let expected = expected_type.PrettyName()
        let real = container.GetMeta(name).GetType().PrettyName()
        let message = sprintf "The metadata field '%s' was expected to have type '%s', but actually had type '%s'" name expected real
        InvalidMetadataTypeException(message)
and MetadataCollisionException(name) = 
    inherit MetadataException(sprintf "The container already has a metadata field with the name '%s'" name)
   
and[<StructuredFormatDisplay("{AsString}")>]
 MetaContainer private (dict : IDictionary<string,AbstractMeta>) as x= 
    static let propCache = Dictionary<Type, PropertyInfo list>()
    let myType = x.GetType()    
    do 
        x.LoadReflectionMetas()
        propCache.[myType] |> List.map (fun prop -> ReflectionMeta(prop, x) :> AbstractMeta) |> List.iter (x.AddMeta)
    new (?metas : AbstractMeta seq) = MetaContainer(metas |> Option.orValue (Seq.empty) |> Seq.map (fun meta -> meta.Name, meta) |> Dict.ofSeq)
    member private x.LoadReflectionMetas() = 
        if not <| propCache.ContainsKey myType then
            let props = 
                myType.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
                |> Seq.filter (fun p -> p.GetCustomAttribute<IsMetaAttribute>(true) <> Unchecked.defaultof<IsMetaAttribute>)
                |> Seq.toList
            propCache.[myType] <- props
            
    member x.AsMetaSeq = dict.Values |> seq
    member x.AddMeta (meta : AbstractMeta) = 
        dict.[meta.Name] <- meta
    member x.SetMeta name (value : 'v) = 
        x.AddMeta <| Meta(name, value)

    member x.GetMeta name : 'v = 
        if dict.ContainsKey(name) |> not then
            raise <| InvalidMetadataNameException(x, name, typeof<'v>)
        let v = dict.[name].Value
        if not <| typeof<'v>.IsAssignableFrom(v.GetType()) then
            raise <| InvalidMetadataTypeException(x, name, typeof<'v>)
        v:?> 'v
    member x.HasMeta name = 
        dict.ContainsKey name
    member x.Clone = 
        let container = MetaContainer()
        for meta in x.AsMetaSeq do
            match meta.Value with 
            | :? MetaContainer as mContainer -> mContainer.Clone |> container.SetMeta (meta.Name)
            | _ -> container.AddMeta meta
        container
    member x.TryGetOr name alt = if x.HasMeta name then x.GetMeta name else alt
    override x.ToString() = 
        let strs = dict |> Dict.values |> Seq.toList |> List.map (fun m -> m.ToString())
        String.Join("; ", strs) |> sprintf "[%s]" 
    member x.AsString = x.ToString()


[<AutoOpen>]
module MetaOps = 
    let (?) (container : MetaContainer) (metaName : string) = container.GetMeta metaName
    let (?<-) (container : MetaContainer) (metaName : string) (value : 'v) =
        container.SetMeta metaName value
    let (++) (container1 : MetaContainer) (container2 : MetaContainer) = 
        let container12 = MetaContainer()
        for meta in Seq.append (container1.AsMetaSeq) (container2.AsMetaSeq) do
            container12.AddMeta meta
        container12
    let (<+) (container : #MetaContainer) meta =
        container.AddMeta meta
        container
[<Extension>]
[<ComplRep(ComplFlags.ModuleSuffix)>]
module Meta = 
    let toPairs (l : MetaContainer) = l.AsMetaSeq |> Seq.map (fun meta -> meta.Name, meta.Value)
    let ofList (l : Meta list) = MetaContainer(l |> Seq.map (fun x -> x :> _))
    let ofSeq (l : Meta seq) = MetaContainer(l |> Seq.map (fun x -> x :> _))
    let ofSeqPairs (l : (_ * _) seq) = l |> Seq.map (fun (a,b) -> Meta(a, b)) |> ofSeq
    let ( <++ ) (a : _ list) (b : _ seq) = a @ (b |> List.ofSeq)
    let ( ++> ) (b : _ seq) (a : _ list) = (b |> List.ofSeq) @ a
    let toMap (container : MetaContainer) = 
        container |> toPairs |> Map.ofSeq

    let get name (container : MetaContainer) = container.GetMeta name
    let tryGet name (container : MetaContainer) = if container.HasMeta name then Some (container.GetMeta name) else None
    let tryGetOr name alt (container : MetaContainer) = container.TryGetOr name alt
    let copyMetadata (source : MetaContainer) (target : 's :> MetaContainer) = 
        source.AsMetaSeq |> Seq.iter (target.AddMeta)
        target

type ICloneableObject = 
    interface end

[<AutoOpen>]
[<Extension>]
type Ext private() = 
    [<Extension>]
    static member Clone (sd : 'a :> ICloneableObject) = 
        let m = sd.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic ||| BindingFlags.Instance)
        m.Invoke(sd, null) :?> 'a
    [<Extension>]
    static member With (sd : 'a :> ICloneableObject, f : 'a -> unit) = 
        let clone = Ext.Clone(sd)
        f clone
        clone


        

