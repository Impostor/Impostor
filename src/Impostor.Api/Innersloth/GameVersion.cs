namespace Impostor.Api.Innersloth
{
    public static class GameVersion
    {
        public static int GetVersion(int year, int month, int day, int revision = 0)
        {
            return (year * 25000) + (month * 1800) + (day * 50) + revision;
        }

        public static void ParseVersion(int version, out int year, out int month, out int day, out int revision)
        {
            year = version / 25000;
            version %= 25000;
            month = version / 1800;
            version %= 1800;
            day = version / 50;
            revision = version % 50;
        }
    }
}
