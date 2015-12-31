 ///A module that contains helper functions and methods.
[<AutoOpen>]
module ExtraFunctional.Misc
open System.CodeDom.Compiler
open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.ComponentModel
open System.Diagnostics

type ObservableList<'v> = ObservableCollection<'v>
type INotifyChanged = INotifyPropertyChanged
type INotifyChanging = INotifyPropertyChanging


type ``type``<'t> internal () = class end

let Type<'t> = ``type``<'t>()


type IndentedTextWriter with
    member x.Push() = x.Indent <- x.Indent + 1
    member x.Pop() = x.Indent <- x.Indent - 1   
     
type Ref<'v> with
    member x.On f = x.Value <- f(x.Value)
      
let (|Ref|) (r : _ ref) = !r

module Math = 
    let root n x = Math.Pow(float x, 1./(float n))
    let intRoot n x = x |> root n |> int
    let intSqrt x = x |> intRoot 2
    let variance = fun lst -> Seq.average(lst |> Seq.map (float >> ( ** ) @? 2.)) - (Seq.averageBy (float) lst ** 2.)
     
module Chars = 
    let alphanumeric = ['A' .. 'Z'] @ ['a' .. 'z'] @ ['0' .. '9']

[<AutoOpen>]
module Type = 
    type Type with 
        member x.JustTypeName() =
            let name = x.Name
            let indexOf = name.IndexOf("`", StringComparison.InvariantCulture)
            if indexOf < 0 then name else name.Substring(0, indexOf)
        member x.PrettyName() = 
            if x.GetGenericArguments().Length = 0 then
                x.Name
            else
                let args = x.GetGenericArguments()
                let name = x.JustTypeName()
                sprintf "%s<%s>" name (args |> Seq.map (fun x -> x.PrettyName()) |> fun strs -> String.Join(",", strs))
    
