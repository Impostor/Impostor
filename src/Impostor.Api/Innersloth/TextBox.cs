namespace Impostor.Api.Innersloth
{
    public static class TextBox
    {
        public static bool IsCharAllowed(char i)
        {
            return i == ' ' || (i >= 'A' && i <= 'Z') || (i >= 'a' && i <= 'z') || (i >= '0' && i <= '9') || (i >= 'À' && i <= 'ÿ') || (i >= 'Ѐ' && i <= 'џ') || (i >= 'ㄱ' && i <= 'ㆎ') || (i >= '가' && i <= '힣');
        }
    }
}
