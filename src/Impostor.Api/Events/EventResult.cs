namespace Impostor.Api.Events;

public interface IEventResult;

public enum EventResultType
{
    Default,
    Cancelled,
    Error,
    Success,
}

public class TypeResult(EventResultType resultType) : IEventResult
{
    public EventResultType Type { get; init; } = resultType;
}
