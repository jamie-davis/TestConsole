using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TestConsoleLib.Exceptions;

namespace TestConsoleLib.Testing
{
    public class CompareUtil
    {
        private static List<Assembly> _reporterAssemblies = new List<Assembly> { typeof(CompareUtil).Assembly };
        private static Dictionary<string, IApprovalsReporter> _reporters; 
        
        public static void RegisterReporters(Assembly assembly)
        {
            _reporterAssemblies.Insert(0, assembly);
            RefreshReporters();
        }

        private static void RefreshReporters()
        {
            var reporters = _reporterAssemblies
                .SelectMany(r => r.GetTypes().Where(t => typeof(IApprovalsReporter).IsAssignableFrom(t))).ToList();
            var constructable = reporters.Select(r => new { Type = r, Constructor = r.GetConstructor(Type.EmptyTypes)})
                .Where(r => r.Constructor != null);
            var instances = constructable.Select(r => Activator.CreateInstance(r.Type) as IApprovalsReporter).Where(i => i != null);
            var namedInstances = instances.Select(i => new {Attrib = i.GetType().GetCustomAttribute<ApprovalsReporterAttribute>(), Instance = i})
                .Where(i => i.Attrib != null)
                .Select(i => new {i.Attrib.Key, i.Instance, i.Instance.GetType().Assembly})
                .ToList();

            var dictionary = new Dictionary<string, IApprovalsReporter>();
            foreach (var distinctKey in namedInstances.Select(n => n.Key).Distinct())
            {
                IApprovalsReporter instance = null;
                var keyInstances = namedInstances.Where(i => i.Key == distinctKey).ToList();
                if (keyInstances.Count == 1)
                {
                    instance = keyInstances.First().Instance;
                }
                else
                {
                    foreach (var assembly in _reporterAssemblies)
                    {
                        var assemblyInstance = keyInstances.FirstOrDefault(i => i.Assembly == assembly);
                        if (assemblyInstance != null)
                        {
                            instance = assemblyInstance.Instance;
                            break;
                        }
                    }
                }

                if (instance != null)
                    dictionary[distinctKey] = instance;
            }

            _reporters = dictionary;
        }

        public static void CompareFiles(string receivedFile, string approvedFile)
        {
            try
            {
                if (_reporters == null)
                    RefreshReporters();

                var reporterName = Environment.GetEnvironmentVariable("TESTREPORTER") ?? string.Empty;
                if (!_reporters.TryGetValue(reporterName, out var reporter))
                    return;

                var arguments = reporter.Arguments == null
                    ? $"\"{receivedFile}\" \"{approvedFile}\""
                    : reporter.Arguments.Replace("$1", receivedFile).Replace("$2", approvedFile);

                var startInfo = new ProcessStartInfo
                {
                    FileName = reporter.FileName,
                    Arguments = arguments
                };
                var process = new Process();
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception e)
            {
                throw new UnableToInvokeFileCompare(e);
            }
        }
    }
}