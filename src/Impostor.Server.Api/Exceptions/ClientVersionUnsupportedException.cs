namespace Impostor.Server
{
    public class ClientVersionUnsupportedException : ImpostorException
    {
        public ClientVersionUnsupportedException(int version)
            : base($"Version {version} is not supported by Impostor")
        {
            Version = version;
        }
        
        public int Version { get; }
    }
}