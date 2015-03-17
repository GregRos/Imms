// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Funq.Tests.Performance.Main
open Funq.Tests.Performance
open Funq
open System
open System.Diagnostics
open LINQtoCSV
open System.Collections.Immutable
open System.IO
open System.Linq
open System.Windows.Forms
open System.Drawing
open System.Drawing.Imaging
open FSharp.Charting
open System.CodeDom.Compiler
open Funq.Tests
open Funq.FSharp.Implementation
///Power over integers.
let inline (^*) (a : int) (b : int) = pown a b
let inline (++) (a : _ list) (b : _ list) = b |> List.append a
let rec containsAny (lst) (trgt : string) =
    match lst with
    | [] -> false
    | h::t -> trgt.Contains(h) || trgt |> containsAny t
    
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
    context.Write(entries, stream)

    stream.Flush()
     
[<EntryPoint>]
let  main argv =
    //let res = Bench.invoke (Test.InsertRandom(1000000).Bind(Target.My.List(1000000)))

    let tests = Scripts.sequential(50000, 50000) //@ Scripts.mapLike(10000, 10000, Seqs.Numbers.length(5, 11)) @ Scripts.setLike(1000, 100, 2000, Seqs.Numbers.length(1, 10), Seqs.Numbers.length(1, 10))
   // let tests = Scripts.sequential(10, 10)
    //let tests = Scripts.setLike(1000, 100, 2000, Seqs.Numbers.length(1, 10), Seqs.Numbers.length(1, 10))
    //let tests = tests |> List.filter (fun x -> x.Entry.Test |> containsAny ["Add"; "Drop"; "Contains"])
    let filename = "results.csv"
    use writer = new IndentedTextWriter(Console.Out)
    let results = tests |> List.map (Bench.invoke writer)
    
    let charts = present results
    let form = new Form()
    let mutable i = 0;
    if Directory.Exists("Results\Logs") |> not then
        Directory.CreateDirectory("Results\Logs") |> ignore
    for chart in charts do
        chart.ShowChart()
        

    let charts = Application.OpenForms |> Seq.cast<Form>
    for chart in charts do
        chart.Width <- (chart.Width |> float) * 1.5 |> int
        chart.Height <- (chart.Height |> float) * 1.5 |> int
        use bmp = new Bitmap(chart.Width, chart.Height)
        chart.DrawToBitmap(bmp, Rectangle(Point.Empty, bmp.Size))
        bmp.Save(sprintf "Results\Charts\%d.png" i, ImageFormat.Png)
        i <- i + 1
    
    let copy = charts |> List.ofSeq
    copy |> List.iter (fun chart -> chart.Close())
    results |> Presentation.printToFile "Results\Logs" "benchmarks" "csv"
    //Application.Run(charts |> Seq.head)

    //Application.Run(form)
    
    
    0
        
        (*
        use file = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
        use stream = new StreamWriter(file)
        use csvWriter = new CsvWriter(stream)
        stream.AutoFlush<- true
        
        for test in tests do
            let result = Bench.invoke test
            csvWriter.WriteRecord(result)

            stream.Flush()
        stream.Flush()0*)
     // return an integer exit code
