using System.Collections.Generic;

namespace Impostor.Api.Innersloth;

public class GameFilterOptions
{
    public HashSet<string> FilterTags { get; } = new HashSet<string>();

    public static GameFilterOptions CreateDefault()
    {
        return new GameFilterOptions
        {
            FilterTags =
            {
                "Beginner",
            },
        };
    }

    public static GameFilterOptions Deserialize(IMessageReader reader)
    {
        var options = new GameFilterOptions();

        var count = reader.ReadPackedInt32();
        for (var i = 0; i < count; i++)
        {
            options.FilterTags.Add(reader.ReadString());
        }

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.WritePacked(FilterTags.Count);
        foreach (var filterTag in FilterTags)
        {
            writer.Write(filterTag);
        }
    }
}
