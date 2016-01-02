///Contains short aliases for commonly used types. Auto-opened.
[<AutoOpen>]
module Imms.FSharp.Aliases

///Alias for IEqualityComparer<'a>.
type IEq<'a> = System.Collections.Generic.IEqualityComparer<'a>
///Alias for EqualityComparer<'a>.
type Eq<'a> = System.Collections.Generic.EqualityComparer<'a>
///Alias for IComparer<'a>.
type ICmp<'a> = System.Collections.Generic.IComparer<'a>
///Alias for Comparer<'a>.
type Cmp<'a> = System.Collections.Generic.Comparer<'a>
///Alias for KeyValuePair<'key, 'value>
type Kvp<'key, 'value> = System.Collections.Generic.KeyValuePair<'key, 'value>
///Alias for IEnumerator<'a>
type Iter<'a> = System.Collections.Generic.IEnumerator<'a>