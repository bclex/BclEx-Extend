using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Contoso.VisualStudio;

namespace Contoso
{
    // register : regasm /codebase [AssemblyFullPath].dll
    // unregister : regasm /codebase [AssemblyFullPath].dll /u
    /// <summary>
    /// Bootstraper
    /// </summary>
    [ComVisible(true)]
    public class Bootstraper : BootstraperBase
    {
        /// <summary>
        /// Registers the class.
        /// </summary>
        /// <param name="t">The t.</param>
        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
#if VS10
            Register(new Version(10, 0), new Guid("{10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC8}"), "ExampleSingleFile", "Example of a single file generator", CSharpCategory, VBCategory);
            Register(new Version(10, 0), new Guid("{10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC9}"), "ExampleMultipleFile", "Example of a multiple file generator", CSharpCategory, VBCategory);
            //
            Register(new Version(10, 0), new Guid("{10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA0}"), "LALR", "LALR generator", CSharpCategory, VBCategory);
            Register(new Version(10, 0), new Guid("{10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA1}"), "LALRReprint", "Reprint LALR generator rules", CSharpCategory, VBCategory);
            Register(new Version(10, 0), new Guid("{10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA2}"), "LALRC", "LALR generator for C", CSharpCategory);
            Register(new Version(10, 0), new Guid("{10A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA3}"), "LALRJS", "LALR generator for JS", CSharpCategory);
#elif VS11
            Register(new Version(11, 0), new Guid("{11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC8}"), "ExampleSingleFile", "Example of a single file generator", CSharpCategory, VBCategory);
            Register(new Version(11, 0), new Guid("{11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDC9}"), "ExampleMultipleFile", "Example of a multiple file generator", CSharpCategory, VBCategory);
            //
            Register(new Version(11, 0), new Guid("{11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA0}"), "LALR", "LALR generator", CSharpCategory, VBCategory);
            Register(new Version(11, 0), new Guid("{11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA1}"), "LALRReprint", "Reprint LALR generator rules", CSharpCategory, VBCategory);
            Register(new Version(11, 0), new Guid("{11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA2}"), "LALRC", "LALR generator for C", CSharpCategory);
            Register(new Version(11, 0), new Guid("{11A7B6C6-E3DA-4bfa-A27C-8F1CEFA3DDA3}"), "LALRJS", "LALR generator for JS", CSharpCategory);

#endif
        }

        /// <summary>
        /// Unregisters the class.
        /// </summary>
        /// <param name="t">The t.</param>
        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
#if VS10
            Unregister(new Version(10, 0), "ExampleSingleFile", CSharpCategory, VBCategory);
            Unregister(new Version(10, 0), "ExampleMultipleFile", CSharpCategory, VBCategory);
            //
            Unregister(new Version(10, 0), "LALR", CSharpCategory);
            Unregister(new Version(10, 0), "LALRReprint", CSharpCategory);
            Unregister(new Version(10, 0), "LALRC", CSharpCategory);
            Unregister(new Version(10, 0), "LALRJS", CSharpCategory);
#elif VS11
            Unregister(new Version(11, 0), "ExampleSingleFile", CSharpCategory, VBCategory);
            Unregister(new Version(11, 0), "ExampleMultipleFile", CSharpCategory, VBCategory);
            //
            Unregister(new Version(11, 0), "LALR", CSharpCategory);
            Unregister(new Version(11, 0), "LALRReprint", CSharpCategory);
            Unregister(new Version(11, 0), "LALRC", CSharpCategory);
            Unregister(new Version(11, 0), "LALRJS", CSharpCategory);
#endif
        }
    }
}
