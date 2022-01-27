namespace Impostor.Api.Innersloth
{
    public static class TextBox
    {
        public static bool IsCharAllowed(char i)
        {
            return i == ' ' ||
                (i >= 'A' && i <= 'Z') ||
                (i >= 'a' && i <= 'z') ||
                (i >= '0' && i <= '9') ||

                // U+00C0 to U+00FF (Latin-1 Supplement)
                (i >= 'À' && i <= 'ÿ') ||

                // U+0400 to U+045F (Cyrillic)
                (i >= 'Ѐ' && i <= 'џ') ||

                // U+2C61 to U+D7A3 (CJK)
                (i >= 'ⱡ' && i <= '힣');
        }
    }
}
