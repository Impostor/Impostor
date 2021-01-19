namespace Impostor.Api.Reactor
{
    public readonly struct Mod
    {
        public readonly string Id;
        public readonly string Version;
        public readonly PluginSide Side;

        public Mod(string id, string version, PluginSide side)
        {
            Id = id;
            Version = version;
            Side = side;
        }

        public override string ToString()
        {
            return $"{Id} ({Version})";
        }
    }
}
