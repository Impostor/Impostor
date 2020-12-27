namespace Impostor.Api.Reactor
{
    public readonly struct Mod
    {
        public readonly string Id;
        public readonly string Version;

        public Mod(string id, string version)
        {
            Id = id;
            Version = version;
        }

        public override string ToString()
        {
            return $"{Id} ({Version})";
        }
    }
}
