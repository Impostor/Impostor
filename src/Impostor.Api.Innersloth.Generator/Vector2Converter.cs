using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Impostor.Api.Innersloth.Generator;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        float x = 0;
        float y = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) break;

            switch (reader.GetString())
            {
                case "x":
                    reader.Read();
                    x = reader.GetSingle();
                    break;

                case "y":
                    reader.Read();
                    y = reader.GetSingle();
                    break;
            }
        }

        return new Vector2(x, y);
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}
