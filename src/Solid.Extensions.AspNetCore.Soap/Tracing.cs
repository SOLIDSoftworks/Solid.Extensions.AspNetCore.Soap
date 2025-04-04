using System.Diagnostics;

namespace Solid.Extensions.AspNetCore.Soap
{
    public class Tracing
    {
        public static ActivitySource Soap = new ActivitySource(Names.Soap, GenerateAssemblyVersion()); 
        
        public static class Names
        {
            public const string Soap = "Solid.Extensions.AspNetCore.Soap";
        }
        

        private static string GenerateAssemblyVersion()
        {
            var version = typeof(Tracing).Assembly.GetName().Version;
            return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}