using Impostor.Api.Innersloth;
using Impostor.Api.Net;

namespace Impostor.Server.Net.Factories
{
    internal interface IClientFactory
    {
        ClientBase Create(IHazelConnection connection, string name, GameVersion clientVersion, Language language, QuickChatModes chatMode, PlatformSpecificData platformSpecificData);
    }
}
