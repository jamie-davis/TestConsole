using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace TestConsoleLib.Testing
{
    public class CompareUtil
    {
        private static List<Assembly> _reporterAssemblies = new List<Assembly> { typeof(CompareUtil).Assembly };
        private static Dictionary<string, IApprovalsReporter> _reporters; 
        
        public static void RegisterReporters(Assembly assembly)
        {
            _reporterAssemblies.Add(assembly);
            RefreshReporters();
        }

        private static void RefreshReporters()
        {
            
        }

        public static void CompareFiles(string receivedFile, string approvedFile)
        {
            var reporter = Environment.GetEnvironmentVariable("ApprovalsReporter");
            var reporters =  AssemblyLoadContext.Default.
            
            var startInfo = new ProcessStartInfo
            {
                FileName = "kompare",
                Arguments = $"\"{receivedFile}\" \"{approvedFile}\""
            };
            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}