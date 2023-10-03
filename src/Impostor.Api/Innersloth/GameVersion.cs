using System;
using System.Numerics;

namespace Impostor.Api.Innersloth
{
    public readonly struct GameVersion : IEquatable<GameVersion>, IComparable<GameVersion>, IComparisonOperators<GameVersion, GameVersion, bool>
    {
        private const int YearMask = 25000;
        private const int MonthMask = 1800;
        private const int DayMask = 50;

        public GameVersion(int value)
        {
            Value = value;
        }

        public GameVersion(int year, int month, int day, int revision = 0)
        {
            Value = (year * YearMask) + (month * MonthMask) + (day * DayMask) + revision;
        }

        public int Value { get; }

        public int Year => Value / YearMask;

        public int Month => (Value % YearMask) / MonthMask;

        public int Day => ((Value % YearMask) % MonthMask) / DayMask;

        public int Revision => Value % DayMask;

        public static bool operator ==(GameVersion left, GameVersion right) => left.Value == right.Value;

        public static bool operator !=(GameVersion left, GameVersion right) => left.Value != right.Value;

        public static bool operator >(GameVersion left, GameVersion right) => left.Value > right.Value;

        public static bool operator >=(GameVersion left, GameVersion right) => left.Value >= right.Value;

        public static bool operator <(GameVersion left, GameVersion right) => left.Value < right.Value;

        public static bool operator <=(GameVersion left, GameVersion right) => left.Value <= right.Value;

        public void GetComponents(out int year, out int month, out int day, out int revision)
        {
            var value = Value;
            year = value / YearMask;
            value %= YearMask;
            month = value / MonthMask;
            value %= MonthMask;
            day = value / DayMask;
            revision = value % DayMask;
        }

        public GameVersion ExtractDisableServerAuthority(out bool disableServerAuthority)
        {
            const int DisableServerAuthorityFlag = 25;

            var revision = Value % DayMask;
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
