namespace Impostor.Api.Innersloth
{
    public class GameVersion
    {
        public static int GetVersion(int year, int month, int day, int rev = 0)
        {
            return (year * 25000) + (month * 1800) + (day * 50) + rev;
        }
    }
}
