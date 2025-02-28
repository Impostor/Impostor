namespace Impostor.Api.Events;

public interface IEventResult;

public enum EventResultType
{
    Default,
    Cancelled,
    Error,
    Success,
}

public class NumberEventOutcome<T>(T result) : IEventResult where T : unmanaged
{
    public T Result { get; init; } = result;
    
    public static implicit operator T (NumberEventOutcome<T> result) => result.Result;
}

public class EventOutcome<T>(T result) : IEventResult
{
    public T Result { get; init; } = result;
    
    public static implicit operator T (EventOutcome<T> result) => result.Result;
}

public class TypeResult(EventResultType resultType) : IEventResult
{
    public EventResultType Type { get; init; } = resultType;
    
    public static implicit operator EventResultType (TypeResult result) => result.Type;
}
