using System.Collections.Generic;

namespace Impostor.Server.Net.Inner;

internal class GameObject
{
    protected List<object> Components { get; } = new();

    public List<T> GetComponentsInChildren<T>()
    {
        var result = new List<T>();

        foreach (var component in Components)
        {
            if (component is T c)
            {
                result.Add(c);
            }
        }

        return result;
    }
}
