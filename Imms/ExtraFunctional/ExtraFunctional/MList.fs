namespace ExtraFunctional
open System.Collections.Generic
type MutableList<'v> = System.Collections.Generic.List<'v>
module MutableList = 
    let empty<'v> : MutableList<'v>  = MutableList<'v>()
    let emptyOf<'v> (n : int) : MutableList<'v> = MutableList<'v>(n)
    let ofSeq (sq : _ seq) : MutableList<'v> = MutableList<'v>(sq)
