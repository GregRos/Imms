namespace Imms.Tests.Performance
open Imms.FSharp.Implementation
open Imms.Tests.Performance
open Imms.Tests
open ExtraFunctional
///An object that binds typed tests to concrete targets and collects fully bound tests ready for execution.
type internal Builder<'s>(bound : ErasedTest MutableList, targets : DataStructure<'s> MutableList, tests : Test<'s> MutableList) = 
    
    ///All the tests bound by this builder.
    member val Bound = bound
    
    ///All the concrete targets of the inferred type cached by the builder.
    member val Targets = targets
    
    ///All the unbound tests for the current inferred type cached by the builder.
    member val Tests = tests
    
    ///Adds an unbound test.
    member inline x.AddTest test = 
        tests.Add(test)
        x
    
    ///Adds a concrete target binding for the current collection type.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    member inline x.AddTarget targ = 
        targets.Add(targ)
        x
    
    ///Adds a list of concrete target bindings.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    member x.AddTargets targets2 = 
        let targets2 = targets |> Seq.toList
        targets.AddRange(targets2)
        x

    ///Adds a list of unbound tests.
    member inline x.AddTests tests2 = 
        let tests2 = tests2 |> Seq.toList
        tests.AddRange(tests2)
        x
    
    member inline x.Done =
        let erasedTests = 
            x.Targets
            |> List.cross x.Tests
            |> List.map (fun (test,targ)-> test.Bind targ)

        x.Targets.Clear()
        x.Tests.Clear()
        bound.AddRange(erasedTests)

    ///Uses a cross product to bind each unbound test to each target binding. 
    ///Returns a new Builder object with an unbound type.
    member inline x.ForTarget<'next>(newTarget : DataStructure<'next>) = 
        x.Done
        Builder<'next>(bound, MutableList.empty, MutableList.empty).AddTarget(newTarget)
    
    ///Returns a new builder initialized with blank caches.
    static member inline New<'s>() = Builder<'s>(MutableList.empty, MutableList.empty, MutableList.empty)


///A companion module for the Builder object. 
[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Builder = 
    ///Adds an unbound test.
    let inline addTest test (builder : Builder<_>) = builder.AddTest test
    
    ///Adds a list of unbound tests.
    let inline addTests tests (builder : Builder<_>) = builder.AddTests tests
    
    let inline addTestsFunc tests args (builder : Builder<_>) = 
        List.cross tests args
        |> List.map (fun (f, arg) -> f arg)
        |> builder.AddTests

    let inline addTestsName tests args name (builder :_ Builder) = 
        let testList = List.cross tests args |> List.map (fun (f,arg) -> f arg)
        testList |>  List.iter (fun (test : Test<_>) -> test?Name <- test?Name + " " = name)
        builder.AddTests testList
        
    
    ///Adds a concrete target binding for the current collection type.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    let inline addTarget targ (builder : Builder<_>) = builder.AddTarget targ
    
    ///Adds a list of concrete target bindings.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    let addTargets targs (builder : Builder<_>) = builder.AddTargets targs
    
    ///Returns a new builder initialized with blank caches.
    let inline blank<'t> = Builder<'t>.New()

    let inline fin (builder : Builder<_>) = builder.Done
   

    let inline cross partials args = 
        Seq.cross partials args |> Seq.map (fun (test, args) -> test args) |> Seq.toList
