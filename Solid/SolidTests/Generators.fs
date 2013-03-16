module SolidTesting.Generators
open System
let chars = ['0' .. '9'] @ ['a' .. 'z'] @ ['A' .. 'Z'] |> List.toArray
let rnd = Random()
let anyInt32 () = rnd.Next(Int32.MinValue,Int32.MaxValue)
let anyDouble ()= rnd.NextDouble() * (System.Double.MaxValue) + rnd.NextDouble() * (System.Double.MinValue)

type IGenerator<'t> = 
    abstract member Generate : unit -> seq<'t> 

type IntegerGenerator = 
    { Count : int 
      Min : int
      Max : int
    }
    interface IGenerator<int> with
        member this.Generate() = 
            seq {for i in 0 .. this.Count -> rnd.Next(this.Min,this.Max)} |> Seq.distinct

type DoubleGenerator = 
    { Count : int }
    interface IGenerator<float> with
        member this.Generate() = 
            seq {for i in 0 .. this.Count -> anyDouble()}  |> Seq.distinct
            

type TupleGenerator = 
    {
      Count : int
    }
    interface IGenerator<int * int * int> with
        
        member this.Generate() = 
            seq {for i in 0 .. this.Count -> this.NextTuple()}  |> Seq.distinct
    member this.NextTuple() = 
            (anyInt32(),anyInt32(),anyInt32())

type StringGenerator = 
    { 
      Chars : char array
      Bounds : int * int;
      Count  : int;
    }
    interface IGenerator<string> with
        
        member this.Generate() = 
            seq {for i in 0 .. this.Count -> this.NextStr()}  |> Seq.distinct
    member this.NextStr() = 
        let mutable str = ""
        let min,max=this.Bounds
        let len = rnd.Next(min,max)
        for i = 0 to len do
            str <- str + string this.Chars.[rnd.Next(0,this.Chars.Length - 1)]
        str
   
let inline generate o = (^s : (member Generate : unit -> seq<'a>) o)
        





