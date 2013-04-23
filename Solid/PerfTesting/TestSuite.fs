module Benchmarks.TestSuite

//Here we define the computation expression that will allow us to declaratively construct a test suite.

//This is going to be a sort of workflow.
//1. 
//  * Add all the targets we want
//  * Add all the tests we want
//2.
//  Bind every target to every test (Using a × product)
//3.
//  Reset and move to the next batch
//


type Builder<'s>(bound : IErasedTest list, targets : ITarget<'s> list, tests : ITest<'s> list) = 
    member val Bound = bound
    member val Targets = targets
    member val Tests = tests
    member inline x.AddTest test = Builder(x.Bound, x.Targets, test::x.Tests)

    member inline x.AddTarget targ = Builder(x.Bound, targ::x.Targets, x.Tests)

    member inline x.AddTargets targets2 = Builder(x.Bound, x.Targets @ targets2, x.Tests)

    member inline x.AddTests tests2 = Builder(x.Bound, x.Targets, x.Tests @ tests2)

    member inline x.Next<'next>() =
        let erasedTests = x.Targets |> List.cross x.Tests |> List.mapPairs (fun test targ -> test.Bind targ)
        Builder<'next>(x.Bound @ erasedTests, [], [])

    static member inline New<'s>() = Builder<'s>([], [], [])

[<CompilationRepresentationAttribute(CompilationRepresentationFlags.ModuleSuffix)>]
module Builder = 
    let inline addTest test (builder: _ Builder) = builder.AddTest test
    let inline addTests tests (builder :_ Builder) = builder.AddTests tests
    let inline addTarget targ (builder:_ Builder) = builder.AddTarget targ
    let inline addTargets targs (builder:_ Builder) = builder.AddTargets targs
    let inline next<'s,'t> (builder :'s Builder) = builder.Next<'t>()
    let inline blank<'t> = Builder<'t>.New()
    let inline finish (builder :_ Builder) = builder.Bound