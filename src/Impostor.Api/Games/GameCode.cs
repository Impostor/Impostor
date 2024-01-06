using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Games;

public readonly struct GameCode : IEquatable<GameCode>
{
    public GameCode(int value)
    {
        Value = value;
        Code = GameCodeParser.IntToGameName(value);
    }

    public GameCode(string code)
    {
        Value = GameCodeParser.GameNameToInt(code);
        Code = code.ToUpperInvariant();
    }

    public string Code { get; }

    public int Value { get; }

    public bool IsInvalid => Value == -1;

    public static implicit operator string(GameCode code)
    {
        return code.Code;
    }

    public static implicit operator int(GameCode code)
    {
        return code.Value;
    }

    public static implicit operator GameCode(string code)
    {
        return From(code);
    }

    public static implicit operator GameCode(int value)
    {
        return From(value);
    }

    public static bool operator ==(GameCode left, GameCode right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GameCode left, GameCode right)
    {
        return !left.Equals(right);
    }

    public static GameCode Create()
    {
        return new GameCode(GameCodeParser.GenerateCode(6));
    }

    public static GameCode From(int value)
    {
        return new GameCode(value);
    }

    public static GameCode From(string value)
    {
        return new GameCode(value);
    }

    /// <inheritdoc />
    public bool Equals(GameCode other)
    {
        return Code == other.Code && Value == other.Value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is GameCode other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Value);
    }

    public override string ToString()
    {
        return Code;
    }
}
