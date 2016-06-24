module Imms.Tests.Performance.Report
open System.IO
open Imms.FSharp.Implementation
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
        Ratio : float option
    }


let private toFullRecord (entry : TestInstanceMeta) = 
    {
        FullRecord.Test = 
            if entry.Test.TargetType.Length > 0 then
                sprintf "%s-%s" entry.Test.TargetType entry.Test.Name
            else
                entry.Test.Name
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
    }

open Newtonsoft.Json
let toJSON (entries : TestInstanceMeta list) = 
    let ys = JsonSerializer()
    ys.Formatting <- Formatting.Indented 
    use stream = new StringWriter()    
    let entries = entries |> Seq.map (toFullRecord)
    ys.Serialize(stream, entries)
    stream.Flush()
    stream.ToString()
    

