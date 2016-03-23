module Imms.Tests.Performance.Report
open System.IO
open Imms.FSharp.Implementation
open LINQtoCSV
open CsvHelper
open ExtraFunctional
type FullRecord = 
    {
        Test : string
        Target : string
        Time : Time
        Target_InitialSize : int
        Iterations : int
        DataSource_Size : int
        DataSource_Type : string
        Target_InitialGenerator : string
        DataGenerator : string
        Description : string
        Class : string
        Ratio : float option
    }


let private toFullRecord (entry : TestInstanceMeta) = 
    {
        FullRecord.Test = entry.Test.Name
        Target = entry.Target.Name
        Target_InitialSize = entry.Target.Count
        Iterations = entry.Test.Iters
        DataSource_Size = entry.Test.DataSource |> Option.map (fun d -> d.Count) |> Option.orValue 0
        DataSource_Type = entry.Test.DataSource |> Option.map (fun d -> d.Name) |> Option.orValue ""
        Time = entry.Time
        Target_InitialGenerator = entry.Target.Generator.ToString()
        DataGenerator = entry.Test.DataSource |> Option.map (fun d -> d.Generator.ToString()) |> Option.orValue ""
        Description = entry.Test |> Meta.tryGetOr "Description" ""
        Ratio = entry.Test |> Meta.tryGet "Ratio"
        Class = entry.Test?Class
    }


let toTable (entries : TestInstanceMeta list) =
    use stringw = new StringWriter()
    use csv = new CsvWriter(stringw)
    let missingText = "X"
    for kind, tests in entries |> Seq.groupBy(fun x -> x.Target.Kind) do
        csv.WriteField("Tests For:")
        csv.WriteField(sprintf "%A" kind)
        csv.NextRecord()
        csv.WriteField("Collection/Test")
        let byName = tests |> Seq.map (fun x -> x.Test.Name) |> Seq.distinct |> Seq.sort |> Seq.toArray
        byName |> Seq.iter (csv.WriteField)
        csv.NextRecord()
        let byTarget = 
            tests 
            |> Seq.groupBy (fun x -> x.Target.Name) 
            |> Seq.sortBy (snd >> Seq.head >> (fun x -> x.Target.Library))

        for name,group in byTarget do
            csv.WriteField name
            for testName in byName do
                let myTest = group |> Seq.tryFind (fun x -> x.Test.Name = testName)
                match myTest with
                | Some test -> csv.WriteField(test.Time.ToString())
                | None -> csv.WriteField(missingText)
            csv.NextRecord()
        csv.NextRecord()
    stringw.Flush()
    stringw.ToString()                               
    
let toLog (entries : TestInstanceMeta list) = 
    let context = CsvContext()
    use stream = new StringWriter()
    let js = Newtonsoft.Json.JsonSerializer.Create()
    
    let entries = entries |> Seq.map (toFullRecord)
    js.Serialize(stream, entries |> Seq.toArray )
    stream.Flush()
    stream.ToString()

