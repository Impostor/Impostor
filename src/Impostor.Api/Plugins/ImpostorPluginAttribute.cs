using System;

namespace Impostor.Api.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImpostorPluginAttribute : Attribute
    {
        public ImpostorPluginAttribute(string package, string name, string author, string version)
        {
            Package = package;
            Name = name;
            Author = author;
            Version = version;
        }

        public string Package { get; }

        public string Name { get; }

        public string Author { get; }

        public string Version { get; }
    }
}
