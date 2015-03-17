namespace Funq.Benchmarking
open System
open System.Linq
open System.Collections.Generic
open System.Collections
open Funq

type Meta =
    abstract Value : obj
    abstract Name : string

type TestKind = 
    | Simple
    | DataSource

type DataSource = 
    {
        Name  : string
        Count : int
    } interface Meta with
        member x.Value = (x.Name, x.Count) :>_
        member x.Name = "Data Source"
    override x.ToString() = sprintf "Structure: %s; Count: %A" x.Name x.Count

type SeqMeta<'a> = 
    {
        Name : string
        Range : 'a * 'a
    } interface Meta with
        member x.Value = (x.Name, x.Range):>_
        member x.Name = "Sequence Type"
    override x.ToString() = sprintf "Type: %s; Range: %A" x.Name x.Range



type DescMeta = 
    | Desc of string
    interface Meta with
        member x.Value = x.Text :>_
        member x.Name = "Description"
    override x.ToString() = sprintf "Description: %s" x.Text
    member x.Text = match x with Desc s -> s
//A tag giving a summary of a single benchmark and its result.
//The record fields are mutable to improve compatibility with serialization code.

type ITestResult = 
    abstract Target : string
    abstract Test   : string
    abstract Metadata : Meta list

type Dummy() = 
    interface ITestResult with
        member x.Target = ""
        member x.Test = ""
        member x.Metadata = []
type Entry = 
    {
        mutable Target : string
        mutable Test   : string
        mutable ``Target Metadata`` : Meta list
        mutable ``Test Metadata`` : Meta list
        mutable Kind : TestKind
        [<DefaultValue>]
        mutable Time : float    
    }

module MetaOps = 
    
    let inline (?) (rest : ITestResult) (property : string) = 
        rest.Metadata 
        |> List.filter (fun meta -> meta.Name = property)
        |> Seq.singleOrNone
        |> Option.map (fun meta -> meta.Value)

    let z = Dummy()?Description