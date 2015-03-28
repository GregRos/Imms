namespace Funq.FSharp



[<AutoOpen>]
module Modules = 
    open Funq.FSharp.Implementation.ModuleTypes
    let FunqList<'v> = FunqListModule<'v>.instance
    let FunqVector<'v> = FunqVectorModule<'v>.instance
    let FunqMap<'k,'v> = FunqMapModule<'k, 'v>()
    let FunqOrderedMap<'k,'v> = FunqOrderedMapModule<'k,'v>.instance
    let FunqSet<'v> = FunqSetModule<'v>.instance
    let FunqOrderedSet<'v> = FunqOrderedSetModule<'v>.instance

    
