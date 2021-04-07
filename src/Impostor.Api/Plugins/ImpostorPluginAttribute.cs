using System;

namespace Impostor.Api.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImpostorPluginAttribute : Attribute
    {
        public ImpostorPluginAttribute(
            string package,
            string name,
            string author,
            string version,
            string[] dependencies,
            string[] softDependencies,
            string[] loadBefore)
        {
            Package = package;
            Name = name;
            Author = author;
            Version = version;
            Dependencies = dependencies;
            SoftDependencies = softDependencies;
            LoadBefore = loadBefore;
        }

        public string Package { get; }

        public string Name { get; }

        public string Author { get; }

        public string Version { get; }

        public string[] Dependencies { get;  }

        public string[] SoftDependencies { get; }

        public string[] LoadBefore { get; }
    }
}
