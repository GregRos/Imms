# Funq.Tests.Integrity
This is a test assembly, specifically doing unit and integration testing. 

There are a lot of frameworks for this sort of thing, but I felt none of them quite fit my requirements, so I decided to develop everything myself, just like I did with the performance testing assembly.

The structure is similar to the performance testing assembly, though few of the types are actually identical, and there are different requirements.

1. Everything should be debuggable. That means very little `inline` code.
2. I should be able to group all the tests together to be run easily, but also be able to filter specific tests, since some tests are somewhat time consuming.
3. 


