using Impostor.Api.Innersloth;

namespace SelfHttpMatchmaker.Types;

public class FindGameByCodeResponse
{
    public List<MatchmakerError> Errors { get; set; } = [];
    
    public required GameListing Game { get; set; }

    public StringNames Region { get; set; }
    
    public required string UntranslatedRegion { get; set; }
    
    public class MatchmakerError
    {
        public DisconnectReason Reason { get; set; }
    }
}
