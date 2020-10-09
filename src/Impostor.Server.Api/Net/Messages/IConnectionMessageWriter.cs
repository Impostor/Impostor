using System.Threading.Tasks;

namespace Impostor.Server.Net
{
    public interface IConnectionMessageWriter : IMessageWriter
    {
        ValueTask SendAsync();
    }
}