# Funq.Tests.Performance
This is the benchmarking library. It is really quite complex and, I think, rather well done. When creating this library, I had the following goals:

1. Reduce test infrastructure to a minimum. Basically, avoid virtual/interface method calls, object allocation, type checking, type conversion, and so forth.
2. Although each test works on a particular kind of collection, it should be possible to abstract over all the tests and run them one after the other as part of a single command. 
3. Each test should maintain a complete set of the parameters used, which can include initial target collection size, the data generator used to fill the data structure (e.g. random integers by length, random strings, etc.), and metadata unique to specific tests.
5. It should be possible to generate charts from the test data, as well as a CSV table for comparison.

Test infrastructure consists of several interrelated components:

## Metadata and Containers
Metadata objects are essentially key-value pairs that encode test metadata. There are two kinds of metadata objects: the simple `Meta(name,value)` which is an ordinary key-value pair, an a `ReflectionMeta(property,instace)` which gets its value from an actual property defined on a type. In order to mark a property as a metadata property, the `IsMeta` attribute is used. 

Metadata is stored in the base class `MetaContainer`. This class stores all the concrete `Meta` objects, and is initialized with all the metadata properties of the instance when it is constructed. This latter part is achieved by searching for properties marked with the attribute using reflection.

Metadata can be accessed using methods such as `GetMeta(name)`, and also with the dynamic `?` operator, such as `obj?MetaName`. Metadata properties can also be accessed as properties, of course. 

## 
There 