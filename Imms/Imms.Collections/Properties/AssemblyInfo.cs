using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Imms;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Imms")]
[assembly: AssemblyDescription(
	@"High performance persistent and immutable collections for .NET, supporting many operations efficiently.
 
Includes a catenable/indexed deque, a high performance vector, sets, and maps/dictionaries. 

Better than anything released for .NET previously (as of this writing, and that I'm aware of). Check out the comparisons in the project website."
	)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ImmsInfo.Author)]
[assembly: AssemblyCopyright(ImmsInfo.Copyright)]
[assembly: AssemblyProduct("Imms")]
[assembly: InternalsVisibleTo("Imms.FSharp")]
[assembly: InternalsVisibleTo("Imms.Tests.Performance")]
[assembly: InternalsVisibleTo("Imms.Tests.Integrity")]
[assembly: AssemblyVersion(ImmsInfo.Version)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("efa3e5dc-5425-4567-a4a3-2a6db77584c1")]