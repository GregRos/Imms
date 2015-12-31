[<AutoOpen>]
module Imms.Tests.Performance.Presentation
open Imms.Tests.Performance
open Imms.FSharp.Implementation
open Imms
open System.Drawing
open System.Windows.Forms.DataVisualization.Charting
open Imms.Tests
open System
open System.IO
open LINQtoCSV
open ExtraFunctional
[<AutoOpen>]
module Exts = 
    type Meta with
        member x.Name = x.GetType().JustTypeName()
    
let getValue (meta : Meta) = meta.Value
open FSharp.Charting

let thd (_,_,x) = x

let present (tests : TestInstanceMeta list) = 
    let mutable charts = []
    let instances = 
        tests |> Seq.sortBy(fun test -> test.Target.Name)
        |> Seq.groupBy(fun test -> test.Target.Kind, test.Test.Name)
       
    for (kind,name), tests in instances do
        let mutable labels = []
        let mutable points = []
        let mutable cls = ""
        let mutable iters = 0
        for test in tests do
            let name = test.Target.Name
            let label, time = 
                match test.Time with 
                | TimedOut -> "Timed out", 0.0 
                | Time t -> t.ToString(), t 
                | Error s -> "Error", 0.0 
                | Unknown -> "?", 0.0
            labels <- label::labels
            points <- (name, time)::points
            cls <- test.Test.Group
            iters <- test.Test.Iters
        let labels, points = List.zip labels points |> List.sortBy (snd >> snd) |> List.unzip
        let points = points |> List.sortBy (fun (name,v) -> v : float) //|> List.map snd
        let title = name
        let chart = Chart.Bar(points, Title=title,Labels= labels, XTitle = "Collection", YTitle = "Time (in ms)")
        charts <- (kind, cls, chart)::charts

    charts |> Seq.groupBy (fun (a,b,_) -> a,b) |> Seq.map (fun ((k,cls), tests) -> tests |> Seq.map thd |> Chart.Rows |> Chart.WithTitle(Text=cls), k, cls)


    
    


    
    


