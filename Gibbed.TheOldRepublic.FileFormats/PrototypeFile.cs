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

using System;
using System.IO;
using System.Text;
using Gibbed.IO;

namespace Gibbed.TheOldRepublic.FileFormats
{
    public class PrototypeFile
    {
        public ulong Id;
        public string Name;
        public string Description;
        public uint Unknown6;
        public uint Unknown7;
        public ulong BaseClass;
        public uint Unknown9;

        public byte[] Data;

        public void Deserialize(Stream input)
        {
            var magic = input.ReadValueU32(Endian.Little);
            if (magic != 0x544F5250 && // 'PROT'
                magic.Swap() != 0x544F5250)
            {
                throw new FormatException();
            }
            var endian = magic == 0x544F5250 ? Endian.Little : Endian.Big;

            var contentVersion = input.ReadValueU16(endian);
            if (contentVersion < 2 || contentVersion > 2)
            {
                throw new FormatException();
            }

            var transportVersion = input.ReadValueU16(endian);
            if (transportVersion < 1 || transportVersion > 5)
            {
                throw new FormatException();
            }

            this.Id = input.ReadValueU64(endian);
            this.Name = input.ReadString(endian, Encoding.UTF8);
            this.Description = input.ReadString(endian, Encoding.UTF8);
            this.Unknown6 = input.ReadValueU32(endian);
            this.Unknown7 = input.ReadValueU32(endian);
            this.BaseClass = input.ReadValueU64(endian);
            this.Unknown9 = transportVersion >= 5 ? 0u : input.ReadValueU32(endian);

            var unknown10 = input.ReadValueU32(endian);
            for (uint i = 0; i < unknown10; i++)
            {
                var unknown11 = input.ReadValueU64(endian);
            }

            var unknown12 = input.ReadValueU8();

            var unknown13 = input.ReadValueU16(endian);
            var unknown14 = transportVersion >= 3 ? input.ReadValueU8() : (byte)3;

            if (unknown13 >= 5)
            {
                var dataLength = input.ReadValueU32(endian);
                this.Data = input.ReadBytes(dataLength);
            }
            else
            {
                var unknown15 = input.ReadValueU32(endian);
                for (uint i = 0; i < unknown15; i++)
                {
                    var unknown16 = input.ReadValueU64(endian);
                    var unknown17 = input.ReadValueU32(endian);
                }

                var dataLength = input.ReadValueU32(endian);
                this.Data = input.ReadBytes(dataLength);
            }
        }
    }
}
