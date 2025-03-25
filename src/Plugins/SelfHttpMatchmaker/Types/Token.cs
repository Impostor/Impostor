using System.Text.Json.Serialization;

namespace SelfHttpMatchmaker.Types;

/// <summary>
///     Token that is returned to the user with a "signature".
/// </summary>
public sealed class Token
{
    // 定义一个名为Content的属性，类型为TokenPayload，使用JsonPropertyName特性指定序列化时的名称为"Content"
    [JsonPropertyName("Content")] public required TokenPayload Content { get; init; }

    // 定义一个名为Hash的属性，类型为string，使用JsonPropertyName特性指定序列化时的名称为"Hash"
    [JsonPropertyName("Hash")] public required string Hash { get; init; }
}
