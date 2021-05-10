using System;

namespace Impostor.Api.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImpostorPluginAttribute : Attribute
    {
        public ImpostorPluginAttribute(string id)
        {
            Id = id;
        }

        [Obsolete("Use (string id) constructor to avoid redundancy")]
        public ImpostorPluginAttribute(string id, string name, string author, string version)
        {
            Id = id;
            Name = name;
            Author = author;
            Version = version;
        }

        public string Id { get; }

        public string? Name { get; }

        public string? Author { get; }

        public string? Version { get; }
    }
}
