namespace Funq.Tests.Performance.Data
open Funq.Tests.Performance

module Basic = 
    let Array count generator  = DataStructure.Create("Array", count, generator, Seq.toArray)
    let List count generator   = DataStructure.Create("FSharpList", count, generator, Seq.toList)

