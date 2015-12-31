module ExtraFunctional.Notification
open System
open System.ComponentModel
open System.Reflection
let (|PropertyChanged|) (e : PropertyChangedEventArgs) = e.PropertyName

[<AbstractClass>]
type ChangeNotification() = 
    let notifyChanged = Event<ChangeNotification * string>()
    let notifyChangedExternal = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()
    do notifyChanged.Publish.Add(fun (self,prop) -> notifyChangedExternal.Trigger(self, PropertyChangedEventArgs(prop)))
    interface INotifyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = notifyChangedExternal.Publish
   
    member x.PropertyChanged = notifyChanged.Publish

    member x.NotifyChange (propertyName : string) = 
        notifyChanged.Trigger(x, propertyName)
        

