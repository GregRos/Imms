namespace Benchmarks

///An object that binds typed tests to concrete targets and collects fully bound tests ready for execution.
type Builder<'s>(bound : IErasedTest list, targets : ITarget<'s> list, tests : ITest<'s> list) = 
    ///All the tests bound by this builder.
    member val Bound = bound
    ///All the concrete targets of the inferred type cached by the builder.
    member val Targets = targets
    ///All the unbound tests for the current inferred type cached by the builder.
    member val Tests = tests
    ///Adds an unbound test.
    member inline x.AddTest test = Builder(x.Bound, x.Targets, test::x.Tests)
    ///Adds a concrete target binding for the current collection type.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    member inline x.AddTarget targ = Builder(x.Bound, targ::x.Targets, x.Tests)
    ///Adds a list of concrete target bindings.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    member inline x.AddTargets targets2 = Builder(x.Bound, x.Targets @ targets2, x.Tests)
    ///Adds a list of unbound tests.
    member inline x.AddTests tests2 = Builder(x.Bound, x.Targets, x.Tests @ tests2)
    ///Uses a cross product to bind each unbound test to each target binding. 
    ///Returns a new Builder object with an unbound type.
    member inline x.Next<'next>() =
        let erasedTests = x.Targets |> List.cross x.Tests |> List.mapPairs (fun test targ -> test.Bind targ)
        Builder<'next>(x.Bound @ erasedTests, [], [])
    ///Returns a new builder initialized with blank caches.
    static member inline New<'s>() = Builder<'s>([], [], [])

///A companion module for the Builder object. 
[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module Builder = 
    ///Adds an unbound test.
    let inline addTest test (builder: _ Builder) = builder.AddTest test
    ///Adds a list of unbound tests.
    let inline addTests tests (builder :_ Builder) = builder.AddTests tests
    ///Adds a concrete target binding for the current collection type.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    let inline addTarget targ (builder:_ Builder) = builder.AddTarget targ
    ///Adds a list of concrete target bindings.
    ///The first time a target is added to the builder will cause type inference
    ///To bind the builder to a matching type.
    let inline addTargets targs (builder:_ Builder) = builder.AddTargets targs
    ///Uses a cross product to bind each unbound test to each target binding. 
    ///Returns a new Builder object with an unbound type.
    let inline next<'s,'t> (builder :'s Builder) = builder.Next<'t>()
    ///Returns a new builder initialized with blank caches.
    let inline blank<'t> = Builder<'t>.New()
    ///Returns all the bound tests cached by the builder.
    ///Doesn't bind the unbound tests/targets currently cached by the builder, if such exist.
    let inline finish (builder :_ Builder) = builder.Bound