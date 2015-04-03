namespace Funq.Tests.Performance
open Funq.FSharp.Implementation
open Funq.Tests.Performance
open Funq.Tests
///An object that binds typed tests to concrete targets and collects fully bound tests ready for execution.
type internal Builder<'s>(bound : ErasedTest list, targets : DataStructure<'s> list, tests : Test<'s> list) = 
    
    ///All the tests bound by this builder.
    member val Bound = bound
    
    ///All the concrete targets of the inferred type cached by the builder.
    member val Targets = targets
    
    ///All the unbound tests for the current inferred type cached by the builder.
    member val Tests = tests
    
    ///Adds an unbound test.
    member inline x.AddTest test = Builder(x.Bound, x.Targets, test :: x.Tests)
    
    ///Adds a concrete target binding for the current collection type.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    member inline x.AddTarget targ = 
        Builder(x.Bound, targ :: x.Targets, x.Tests)
    
    ///Adds a list of concrete target bindings.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    member x.AddTargets targets2 = 
        let targets2 = targets |> Seq.toList
        Builder(x.Bound, x.Targets @ targets2, x.Tests)
    
    ///Adds a list of unbound tests.
    member inline x.AddTests tests2 = 
        let tests2 = tests2 |> Seq.toList
        Builder(x.Bound, x.Targets, x.Tests @ tests2)
    
    ///Uses a cross product to bind each unbound test to each target binding. 
    ///Returns a new Builder object with an unbound type.
    member inline x.Next<'next>() = 
        let erasedTests = 
            x.Targets
            |> List.cross x.Tests
            |> List.mapPairs (fun test targ -> test.Bind targ)
        Builder<'next>(x.Bound @ erasedTests, [], [])
    
    ///Returns a new builder initialized with blank caches.
    static member inline New<'s>() = Builder<'s>([], [], [])


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
    
    ///Uses a cross product to bind each unbound test to each target binding. 
    ///Returns a new Builder object with an unbound type.
    let inline next<'p, 'n> (builder : Builder<'p>) = builder.Next<'n>()
    
    ///Returns a new builder initialized with blank caches.
    let inline blank<'t> = Builder<'t>.New()
    
    ///Returns all the bound tests cached by the builder.
    ///Doesn't bind the unbound tests/targets currently cached by the builder, if such exist.
    let inline finish (builder : _ Builder) = builder.Next().Bound

    let inline cross partials args = 
        Seq.cross partials args |> Seq.map (fun (test, args) -> test args) |> Seq.toList

[<AutoOpen>]
module internal BuilderOperators = 
    let inline (<<+) builder test = builder |> Builder.addTest test
    let (<<++) builder testGroup =
        builder |> Builder.addTests testGroup

    let inline ( *<< ) partials args = 
        List.cross partials args |> List.mapPairs (fun f args -> f args)