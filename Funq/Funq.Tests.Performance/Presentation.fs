[<AutoOpen>]
module Funq.Tests.Performance.Presentation
open Funq.Tests.Performance
open Funq.FSharp.Implementation
open Funq
open System.Drawing
open System.Windows.Forms.DataVisualization.Charting
open Funq.Tests
open System
open System.IO
open LINQtoCSV
[<AutoOpen>]
module Exts = 
    type Meta with
        member x.Name = x.GetType().JustTypeName()
    


let getValue (meta : Meta) = meta.Value
open FSharp.Charting

let present (tests : MetaContainer list) = 
    let mutable charts = []
    let instances = tests |> Seq.sortBy (fun test -> test?Name : string)
    let instances = 
        tests |> Seq.sortBy(fun test -> test?Target?Name)
        |> Seq.groupBy(fun test -> test?Test?Name)
       
    for name, tests in instances do
        let mutable labels = []
        let mutable points = []
        let mutable cls = ""
        let mutable iters = 0
        for test in tests do
            let name = test?Target?Name
            let label, time = match test?Time with | TimedOut -> "Timed out", 0.0 | Time t -> t.ToString(), t
            labels <- label::labels
            points <- (name, time)::points
            cls <- test?Test?Class
            iters <- test?Test?Iterations
        let labels, points = List.zip labels points |> List.sortBy (snd >> snd) |> List.unzip
        let points = points |> List.sortBy (fun (name,v) -> v : float) //|> List.map snd
        let title = sprintf "%s (Iterations: %d)" (string name) iters
        let chart = Chart.Bar(points, Title=title,Labels= labels, XTitle = "Collection", YTitle = "Time (in ms)")
            //|> Chart.WithStyling(Margin=(0.,0.,0.,0.))
        charts <- (cls, chart)::charts

    charts |> Seq.groupBy fst |> Seq.map (fun (cls, tests) -> tests |> Seq.map snd |> Chart.Rows |> Chart.WithTitle(Text=cls))


type Record = 
    {
        Name : string
        Target : string
        Target_InitialSize : int
        Iterations : int
        DataSource_Size : int
        DataSource_Type : string
        Time : Time
        Target_InitialGenerator : string
        DataGenerator : string
        Description : string
        Class : string
    }


let private meta_to_record (entry : MetaContainer) = 
    match entry?Test?Kind with
    | "Simple" -> 
        {
            Record.Name = entry?Test?Name
            Description = entry?Test |> Meta.tryGetOr "Description" ""
            Target = entry?Target?Name
            Target_InitialSize = entry?Target?Count
            Iterations = entry?Test?Iterations
            Time = entry?Time
            DataSource_Size = 0
            DataSource_Type = ""
            DataGenerator = ""
            Target_InitialGenerator = entry?Target?Generator.ToString()
            Class = entry?Test?Class
        }
    | "Data Source" ->
        {
            Record.Name = entry?Test?Name
            Description = entry?Test |> Meta.tryGetOr "Description" ""
            Target = entry?Target?Name
            Target_InitialSize = entry?Target?Count
            Iterations = entry?Test?Iterations
            Time = entry?Time
            DataSource_Size = entry?Test?Source?Count
            DataSource_Type = entry?Test?Source?Name
            DataGenerator = entry?Test?Source?Generator.ToString()
            Target_InitialGenerator = entry?Target?Generator.ToString()
            Class = entry?Test?Class
        }
 
    | _ -> failwith "Unknown kind"
let printToFile path filename ext (entries : MetaContainer list) = 
    let rec getFreeName n =
        let tryName = sprintf "%s\%s.%03d.%s" path filename n ext
        if File.Exists(tryName) then
            getFreeName (n+1)
        else
            tryName
    let filename = getFreeName 0
    use file = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
    let context = CsvContext()
    use stream = new StreamWriter(file)
    stream.AutoFlush<- true
    let entries = entries |> Seq.map (meta_to_record)
    context.Write(entries, stream)

    stream.Flush()
    
    


    
    


