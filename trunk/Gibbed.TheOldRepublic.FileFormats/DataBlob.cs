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
using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

namespace Gibbed.TheOldRepublic.FileFormats
{
    public class DataBlob
    {
        public ushort Flags;

        public byte UnknownSetting
        {
            get { return (byte)((this.Flags >> 3) & 0xF); }
            set
            {
                this.Flags &= 0xFF87; // ~(0xFu << 3)
                this.Flags |= (ushort)((value & 0xFu) << 3);
            }
        }

        public void Serialize(MemoryStream output, Endian endian)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(MemoryStream input, Endian endian)
        {
            var start = input.Position;
            var length = input.ReadValueU32(endian);
            var end = start + length;

            var unknown04 = input.ReadValueU32(endian);
            var unknown08 = input.ReadValueU64(endian);
            this.Flags = input.ReadValueU16(endian);

            var dataOffset = input.ReadValueU16(endian);
        }
    }
}
