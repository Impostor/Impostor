using System.Threading.Tasks;

namespace Impostor.Api.Net.Inner.Objects
{
    public interface IInnerPlayerControl
    {
        IInnerPlayerInfo PlayerInfo { get; }

        ValueTask SetNameAsync(string name);

        ValueTask SendChatAsync(string text);
    }
}