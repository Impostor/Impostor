using System;

namespace Impostor.Api.Innersloth.GameFilters
{
    [Serializable]
    public class ChatModeGameFilter : ISubFilter
    {
        public string FilterType { get; } = "chat";

        public byte AcceptedValues;
    }
}
