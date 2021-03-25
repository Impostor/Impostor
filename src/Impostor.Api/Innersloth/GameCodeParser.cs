using System;
using System.Buffers.Binary;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Impostor.Api.Innersloth
{
    public static class GameCodeParser
    {
        private const string V2 = "QWXRTYLPESDFGHUJKZOCVBINMA";

        private static readonly int[] V2Map = Enumerable.Range(65, 26).Select(v => V2.IndexOf((char)v)).ToArray();

        private static readonly RNGCryptoServiceProvider Random = new RNGCryptoServiceProvider();

        public static string IntToGameName(int input)
        {
            // V2.
            if (input < -1)
            {
                return IntToGameNameV2(input);
            }

            // V1.
            Span<byte> code = stackalloc byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(code, input);
#if NETSTANDARD2_0
            return Encoding.UTF8.GetString(code.Slice(0, 4).ToArray());
#else
            return Encoding.UTF8.GetString(code.Slice(0, 4));
#endif
        }

        public static int GameNameToInt(string code)
        {
            var upper = code.ToUpperInvariant();
            if (upper.Any(x => !char.IsLetter(x)))
            {
                return -1;
            }

            var len = code.Length;
            if (len == 6)
            {
                return GameNameToIntV2(upper);
            }

            if (len == 4)
            {
                return code[0] | ((code[1] | ((code[2] | (code[3] << 8)) << 8)) << 8);
            }

            return -1;
        }

        public static int GenerateCode(int len)
        {
            if (len != 4 && len != 6)
            {
                throw new ArgumentException("should be 4 or 6", nameof(len));
            }

            // Generate random bytes.
#if NETSTANDARD2_0
            var data = new byte[len];
#else
            Span<byte> data = stackalloc byte[len];
#endif
            Random.GetBytes(data);

            // Convert to their char representation.
            Span<char> dataChar = stackalloc char[len];
            for (var i = 0; i < len; i++)
            {
                dataChar[i] = V2[V2Map[data[i] % 26]];
            }

#if NETSTANDARD2_0
            return GameNameToInt(new string(dataChar.ToArray()));
#else
            return GameNameToInt(new string(dataChar));
#endif
        }

        private static string IntToGameNameV2(int input)
        {
            var a = input & 0x3FF;
            var b = (input >> 10) & 0xFFFFF;

            return new string(new[]
            {
                V2[a % 26],
                V2[a / 26],
                V2[b % 26],
                V2[(b / 26) % 26],
                V2[(b / (26 * 26)) % 26],
                V2[(b / (26 * 26 * 26)) % 26],
            });
        }

        private static int GameNameToIntV2(string code)
        {
            var a = V2Map[code[0] - 65];
            var b = V2Map[code[1] - 65];
            var c = V2Map[code[2] - 65];
            var d = V2Map[code[3] - 65];
            var e = V2Map[code[4] - 65];
            var f = V2Map[code[5] - 65];

            var one = (a + (26 * b)) & 0x3FF;
            var two = c + (26 * (d + (26 * (e + (26 * f)))));

            return (int)(one | ((two << 10) & 0x3FFFFC00) | 0x80000000);
        }
    }
}
