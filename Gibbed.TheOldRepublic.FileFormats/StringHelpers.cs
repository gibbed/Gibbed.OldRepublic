/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Text;

namespace Gibbed.TheOldRepublic.FileFormats
{
    public static class StringHelpers
    {
        #region Jenkins
        private static void HashJenkins(byte[] data, int index, int length, uint seed, out uint pc, out uint pb)
        {
            uint a, b, c;
            a = b = c = 0xDEADBEEF + (uint)length + seed;
            c += 0; //pb;

            int i = index;
            while (i + 12 < length)
            {
                a += (uint)data[i++] |
                    ((uint)data[i++] << 8) |
                    ((uint)data[i++] << 16) |
                    ((uint)data[i++] << 24);
                b += (uint)data[i++] |
                    ((uint)data[i++] << 8) |
                    ((uint)data[i++] << 16) |
                    ((uint)data[i++] << 24);
                c += (uint)data[i++] |
                    ((uint)data[i++] << 8) |
                    ((uint)data[i++] << 16) |
                    ((uint)data[i++] << 24);

                a -= c;
                a ^= (c << 4) | (c >> (32 - 4));
                c += b;
                b -= a;
                b ^= (a << 6) | (a >> (32 - 6));
                a += c;
                c -= b;
                c ^= (b << 8) | (b >> (32 - 8));
                b += a;
                a -= c;
                a ^= (c << 16) | (c >> (32 - 16));
                c += b;
                b -= a;
                b ^= (a << 19) | (a >> (32 - 19));
                a += c;
                c -= b;
                c ^= (b << 4) | (b >> (32 - 4));
                b += a;
            }

            if (i < length)
            {
                a += data[i++];
            }

            if (i < length)
            {
                a += (uint)data[i++] << 8;
            }

            if (i < length)
            {
                a += (uint)data[i++] << 16;
            }

            if (i < length)
            {
                a += (uint)data[i++] << 24;
            }

            if (i < length)
            {
                b += (uint)data[i++];
            }

            if (i < length)
            {
                b += (uint)data[i++] << 8;
            }

            if (i < length)
            {
                b += (uint)data[i++] << 16;
            }

            if (i < length)
            {
                b += (uint)data[i++] << 24;
            }

            if (i < length)
            {
                c += (uint)data[i++];
            }

            if (i < length)
            {
                c += (uint)data[i++] << 8;
            }

            if (i < length)
            {
                c += (uint)data[i++] << 16;
            }

            if (i < length)
            {
                c += (uint)data[i++] << 24;
            }

            c ^= b;
            c -= (b << 14) | (b >> (32 - 14));
            a ^= c;
            a -= (c << 11) | (c >> (32 - 11));
            b ^= a;
            b -= (a << 25) | (a >> (32 - 25));
            c ^= b;
            c -= (b << 16) | (b >> (32 - 16));
            a ^= c;
            a -= (c << 4) | (c >> (32 - 4));
            b ^= a;
            b -= (a << 14) | (a >> (32 - 14));
            c ^= b;
            c -= (b << 24) | (b >> (32 - 24));

            pc = c;
            pb = b;
        }

        private static void HashJenkins(this string input, out uint pc, out uint pb)
        {
            var data = Encoding.ASCII.GetBytes(input);
            HashJenkins(data, 0, data.Length, 0, out pc, out pb);
        }

        public static uint HashJenkins32(this string input)
        {
            var data = Encoding.ASCII.GetBytes(input);
            uint pc, pb;
            input.HashJenkins(out pc, out pb);
            return pc;
        }

        public static ulong HashJenkins64(this string input)
        {
            var data = Encoding.ASCII.GetBytes(input);
            uint pc, pb;
            input.HashJenkins(out pc, out pb);
            return ((ulong)pc << 0) | ((ulong)pb << 32);
        }
        #endregion
    }
}
