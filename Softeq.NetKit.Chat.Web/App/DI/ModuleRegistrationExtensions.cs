// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Autofac;

namespace Softeq.NetKit.Chat.Web.App.DI
{
    public static class ModuleRegistrationExtensions
    {
        public static void RegisterSolutionModules(this ContainerBuilder builder)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var rootNamespace = GetNamespaceMask();
            const string assemblyExtension = "dll";
            var assemblyLookupPattern = $"{rootNamespace}.*.{assemblyExtension}";
            var assemblies = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), assemblyLookupPattern, SearchOption.AllDirectories)
                .Select(Assembly.LoadFrom)
                .ToArray();

            builder.RegisterAssemblyModules(assemblies);
        }

        private static string GetNamespaceMask()
        {
            var currentNamespace = typeof(ModuleRegistrationExtensions).Namespace;
            const string rootNamespaceMatchName = "rootNamespace";
            var matches = Regex.Match(currentNamespace, $"(^(?<{rootNamespaceMatchName}>\\w+)\\.)");
            return matches.Groups[rootNamespaceMatchName].Value;
        }
    }
}