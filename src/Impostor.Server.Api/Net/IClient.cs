using System.Collections.Generic;

namespace Impostor.Server.Net
{
    public interface IClient
    {
        int Id { get; }
        
        string Name { get; }
        
        IConnection Connection { get; }
        
        IDictionary<object,object> Items { get; }
    }
}