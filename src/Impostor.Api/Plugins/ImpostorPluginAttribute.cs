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
            string[]? dependencies = null,
            string[]? softDependencies = null,
            string[]? loadBefore = null)
        {
            Package = package;
            Name = name;
            Author = author;
            Version = version;
            Dependencies = dependencies ?? new string[0];
            SoftDependencies = softDependencies ?? new string[0];
            LoadBefore = loadBefore ?? new string[0];
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
