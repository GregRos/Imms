using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Funq Collections")]
[assembly: AssemblyDescription(
	@"High performance persistent and immutable collections for .NET, supporting many operations efficiently. 
Includes a catenable/indexed deque, a high performance vector, sets, and maps/dictionaries. 
Strictly better than anything released for .NET previously. Check out the comparisons in the project website.
Currently not fully tested (except for core operations). Not related to the IoC/DI library also called Funq"
	)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Gregory Rosenbaum")]
[assembly: AssemblyCopyright("Gregory Rosenbaum")]
[assembly: AssemblyProduct("Funq")]
[assembly: InternalsVisibleTo("Funq.FSharp")]
[assembly: InternalsVisibleTo("Funq.Tests.Performance")]
[assembly: InternalsVisibleTo("Funq.Tests.Integrity")]
[assembly: InternalsVisibleTo("Funq.Tests.Unit")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("0.3.1")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("efa3e5dc-5425-4567-a4a3-2a6db77584c1")]