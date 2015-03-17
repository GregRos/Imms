[<AutoOpen>]
module Funq.Benchmarking.Presentation
open Funq.Benchmarking
open Funq
open System.Drawing
open System.Windows.Forms.DataVisualization.Charting
[<AutoOpen>]
module Exts = 
    type Meta with
        member x.Name = x.GetType().JustTypeName()
    


    let getValue (meta : Meta) = meta.Value
open FSharp.Charting
type TestScore =
    {
        Target : string
        Value : double
        Parameters : Map<string,string>
    }


type TestInstance = 
    {
        Name : string
        Parameters : Map<string, string>
        Description : string
        Scores : TestScore list
        Kind : TestKind
    }

let from_test_entries (entries : Entry list) =
    let by_test_name = query {
        for entry in entries do 
        groupBy entry.Test
        }
    
    let mutable instances = []

    for test_type in by_test_name do
        let name = test_type.Key
        let desc = test_type |> Seq.head |> (fun x -> x.``Test Metadata``)  |> Seq.ofType<DescMeta> |> Seq.singleOrNone |> Option.map (fun x -> x.Text) |> Option.orValue ""
        //let meta = meta |> List.filter (fun x -> x :? DescMeta |> not) |> List.map (fun meta -> (meta.Name, meta.Value |> string)) |> Map.ofList
        let mutable scores = []
        let mutable kind = TestKind.Simple
        for test_entry in test_type do
            let target_meta = test_entry.``Target Metadata`` |> List.map(fun meta -> (meta.Name, meta.Value |> string)) |> Map.ofList
            scores <- {Target = test_entry.Target; Value = test_entry.Time; Parameters = target_meta}::scores
            kind <- test_entry.Kind
        instances <- {Name = name; Kind = kind;Parameters = Map.empty; Description = desc; Scores = scores }::instances
    //printf "%A" instances
    instances

let present (entries : Entry list) = 
    let instances = from_test_entries entries
    
    let example = instances.[2]

    let first10 = instances |> Seq.take 3
    let mutable charts = []
    let mutable color_map = Map.empty
    let instances = instances |> Seq.sortBy (fun instance -> instance.Name)
    for instance in instances do
        let mutable labels = []
        let mutable points = []
        let title = instance.Name
        for score in instance.Scores do
            labels <- score.Value::labels
            points <- (score.Target, score.Value)::points

        let labels = labels |> List.sort |> List.map string
        let points = points |> List.sortBy (fun (name,v) -> v) //|> List.map snd
        let chart =  
            Chart.Bar(points, Title= title,Labels= labels, XTitle = "Collection", YTitle = "Time (in ms)")
            //|> Chart.WithStyling(Margin=(0.,0.,0.,0.))
        charts <- (chart,instance)::charts

    let charts = charts |> Seq.groupBy (fun (chart,instance) -> instance.Kind) 
                        |> Seq.map snd 
                        |> Seq.map (Seq.map fst >> Seq.takeIter 4 >> Seq.map Chart.Rows)
                        |> Seq.concat

    charts
    
    


    
    


