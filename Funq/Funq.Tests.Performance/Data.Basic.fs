namespace Funq.Tests.Performance.Data
open Funq.Tests.Performance

module Basic = 
    let Array(count, generator)  = DataStructure.Create("Array", Core, count,"System", generator, Seq.toArray)
    let List(count, generator)   = DataStructure.Create("FSharpList", Core, count, "FSharp", generator, Seq.toList)

