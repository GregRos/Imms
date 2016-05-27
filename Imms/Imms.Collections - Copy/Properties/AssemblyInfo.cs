using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Imms;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Imms")]
[assembly: AssemblyDescription(
	@"A powerful library of immutable and persistent data structures for the .NET platform."
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