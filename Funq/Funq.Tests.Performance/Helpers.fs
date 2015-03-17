///A module that contains helper functions and methods.
[<AutoOpen>]
module Funq.Tests.Performance.Helper

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation
open System.Collections.Concurrent
let delay f arg = fun () -> f arg
type private Cache<'q>() = 
    static let inner = ConcurrentDictionary()
    static member Get (quote : 'q Expr) = 
        match inner.ContainsKey quote with
        | true -> inner.[quote]
        | false -> 
            let f = quote.Compile()
            let lz = lazy(f())
            inner.[quote] <- lz
            lz
let delay2 f arg1 arg2 = fun() -> f arg1 arg2

let memoize q = Cache.Get(q).Value

