using System.Threading.Tasks;

namespace Impostor.Server.Controllers;

public interface IExtensionConnection
{
    public ValueTask SendAsync<T>(T content) where T : class;
        
    public ValueTask SendAsync(string message);
}

public class WebSocketConnection(string ConnectionId) : IExtensionConnection
{
    public ValueTask SendAsync<T>(T content) where T : class
    {
        throw new System.NotImplementedException();
    }

    public ValueTask SendAsync(string message)
    {
        throw new System.NotImplementedException();
    }

}

public class HttpConnection(string ConnectionId) : IExtensionConnection
{
    public ValueTask SendAsync<T>(T content) where T : class
    {
        throw new System.NotImplementedException();
    }

    public ValueTask SendAsync(string message)
    {
        throw new System.NotImplementedException();
    }
}
