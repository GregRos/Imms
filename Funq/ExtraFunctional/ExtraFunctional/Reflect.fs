module ExtraFunctional.Reflect
open System.Reflection
open System
type Bind = BindingFlags

///A set of flags that matches any static, instance, public, or non-public member.
let AnyMember = 
    BindingFlags.Public 
    ||| BindingFlags.NonPublic 
    ||| BindingFlags.Instance 
    ||| BindingFlags.Static 

///A set of flags that matches any non-static member, whether public or not.
let AnyInstance = 
    AnyMember - BindingFlags.Static

///A set of flags that matches any public member.
let AnyPublic = 
    AnyMember - BindingFlags.NonPublic

///A set of flags that matches any static member.
let AnyStatic = 
    AnyMember - BindingFlags.Instance

let AnyDeclared = 
    AnyMember ||| BindingFlags.DeclaredOnly

type PropertySearch(?Name : string, ?Flags : BindingFlags, ?ReturnType : Type) =
    member x.Name = Name
    member x.Flags = Flags
    member x.ReturnType = ReturnType

let (|PropertySearch|) (ps : PropertySearch) = PropertySearch(ps.Name, ps.Flags, ps.ReturnType)

let getAllProperties flags (o : obj) = o.GetType().GetProperties(flags) |> Array.toList

let getProperties name flags (o : obj) = 
    o.GetType().GetMember(name, MemberTypes.Property, flags) 
    |> Array.map (fun x -> x :?> PropertyInfo) 
    |> Array.toList

let getPropertyValue name flags o =
    match o |> getProperties name flags with
    | [ prop ] -> prop.GetValue(o, null)
    | [] -> failwith "No property found"
    | _ -> failwith "Ambiguous match"
 
