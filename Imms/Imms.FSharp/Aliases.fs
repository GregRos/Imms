[<AutoOpen>]
module Imms.FSharp.Aliases

type IEq<'a> = System.Collections.Generic.IEqualityComparer<'a>
type Eq<'a> = System.Collections.Generic.EqualityComparer<'a>
type ICmp<'a> = System.Collections.Generic.IComparer<'a>
type Cmp<'a> = System.Collections.Generic.Comparer<'a>
type Kvp<'key, 'value> = System.Collections.Generic.KeyValuePair<'key, 'value>
type Iter<'a> = System.Collections.Generic.IEnumerator<'a>