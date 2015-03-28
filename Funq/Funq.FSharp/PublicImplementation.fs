namespace Funq.FSharp.Implementation
open System
///The parent type of fake modules used to implement generic modules/inheritance.
type ModuleType() = 
    inherit obj()
    //Obsolete because we don't want them visible.
    [<Obsolete("This is a module type.")>]
    override x.Equals o = base.Equals o
    [<Obsolete("This is a module type.")>]
    override x.GetHashCode() = base.GetHashCode()
    [<Obsolete("This is a module type.")>]
    override x.ToString() = base.ToString()
    [<Obsolete("This is a module type.")>]
    member x.GetType() = base.GetType()
