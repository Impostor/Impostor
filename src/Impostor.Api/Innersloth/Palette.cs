using System.Collections.Generic;
using System.Drawing;
using Impostor.Api.Innersloth.Customization;

namespace Impostor.Api.Innersloth
{
    public static class Palette
    {
        public static readonly Color DisabledGrey = Color.FromArgb(76, 76, 76);
        public static readonly Color DisabledColor = Color.FromArgb(76, 255, 255, 255);
        public static readonly Color EnabledColor = Color.FromArgb(255, 255, 255);
        public static readonly Color Black = Color.FromArgb(0, 0, 0);
        public static readonly Color ClearWhite = Color.FromArgb(0, 255, 255, 255);
        public static readonly Color HalfWhite = Color.FromArgb(128, 255, 255, 255);
        public static readonly Color White = Color.FromArgb(255, 255, 255);
        public static readonly Color LightBlue = Color.FromArgb(128, 128, 255);
        public static readonly Color Blue = Color.FromArgb(51, 51, 255);
        public static readonly Color Orange = Color.FromArgb(255, 153, 1);
        public static readonly Color Purple = Color.FromArgb(153, 26, 153);
        public static readonly Color Brown = Color.FromArgb(184, 110, 28);

        public static readonly Color CrewmateBlue = Color.FromArgb(140, 255, 255);
        public static readonly Color ImpostorRed = Color.FromArgb(255, 25, 25);

        public static readonly Dictionary<ColorType, Color> PlayerColors = new Dictionary<ColorType, Color>
        {
            [ColorType.Red] = Color.FromArgb(198, 17, 17),
            [ColorType.Blue] = Color.FromArgb(19, 46, 210),
            [ColorType.Green] = Color.FromArgb(17, 128, 45),
            [ColorType.Pink] = Color.FromArgb(238, 84, 187),
            [ColorType.Orange] = Color.FromArgb(240, 125, 13),
            [ColorType.Yellow] = Color.FromArgb(246, 246, 87),
            [ColorType.Black] = Color.FromArgb(63, 71, 78),
            [ColorType.White] = Color.FromArgb(215, 225, 241),
            [ColorType.Purple] = Color.FromArgb(107, 47, 188),
            [ColorType.Brown] = Color.FromArgb(113, 73, 30),
            [ColorType.Cyan] = Color.FromArgb(56, 255, 221),
            [ColorType.Lime] = Color.FromArgb(80, 240, 57),
        };

        public static readonly Dictionary<ColorType, Color> ShadowColors = new Dictionary<ColorType, Color>
        {
            [ColorType.Red] = Color.FromArgb(122, 8, 56),
            [ColorType.Blue] = Color.FromArgb(9, 21, 142),
            [ColorType.Green] = Color.FromArgb(10, 77, 46),
            [ColorType.Pink] = Color.FromArgb(172, 43, 174),
            [ColorType.Orange] = Color.FromArgb(180, 62, 21),
            [ColorType.Yellow] = Color.FromArgb(195, 136, 34),
            [ColorType.Black] = Color.FromArgb(30, 31, 38),
            [ColorType.White] = Color.FromArgb(132, 149, 192),
            [ColorType.Purple] = Color.FromArgb(59, 23, 124),
            [ColorType.Brown] = Color.FromArgb(94, 38, 21),
            [ColorType.Cyan] = Color.FromArgb(36, 169, 191),
            [ColorType.Lime] = Color.FromArgb(21, 168, 66),
        };

        public static readonly Color VisorColor = Color.FromArgb(149, 202, 220);
    }
}
