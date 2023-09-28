using System;
using System.Numerics;

namespace Impostor.Api.Innersloth
{
    public readonly struct GameVersion : IEquatable<GameVersion>, IComparable<GameVersion>, IComparisonOperators<GameVersion, GameVersion, bool>
    {
        public GameVersion(int value)
        {
            Value = value;
        }

        public GameVersion(int year, int month, int day, int revision = 0)
        {
            Value = (year * 25000) + (month * 1800) + (day * 50) + revision;
        }

        public int Value { get; }

        public static bool operator ==(GameVersion left, GameVersion right) => left.Value == right.Value;

        public static bool operator !=(GameVersion left, GameVersion right) => left.Value != right.Value;

        public static bool operator >(GameVersion left, GameVersion right) => left.Value > right.Value;

        public static bool operator >=(GameVersion left, GameVersion right) => left.Value >= right.Value;

        public static bool operator <(GameVersion left, GameVersion right) => left.Value < right.Value;

        public static bool operator <=(GameVersion left, GameVersion right) => left.Value <= right.Value;

        public void GetComponents(out int year, out int month, out int day, out int revision)
        {
            var value = Value;
            year = value / 25000;
            value %= 25000;
            month = value / 1800;
            value %= 1800;
            day = value / 50;
            revision = value % 50;
        }

        public GameVersion ExtractDisableServerAuthority(out bool disableServerAuthority)
        {
            const int DisableServerAuthorityFlag = 25;

            var revision = Value % 50;
            if (revision >= DisableServerAuthorityFlag)
            {
                disableServerAuthority = true;
                return new GameVersion(Value - DisableServerAuthorityFlag);
            }

            disableServerAuthority = false;
            return this;
        }

        public override string ToString()
        {
            GetComponents(out var year, out var month, out var day, out var revision);
            return $"{year}.{month}.{day}{(revision == 0 ? string.Empty : "." + revision)}";
        }

        public int CompareTo(GameVersion other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(GameVersion other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is GameVersion other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
