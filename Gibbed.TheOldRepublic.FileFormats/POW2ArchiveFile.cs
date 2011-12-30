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
    public class POW2ArchiveFile
    {
        public Endian Endian;

        public void Serialize(Stream output)
        {
            throw new FormatException();
        }

        public void Deserialize(Stream input)
        {
            var magic = input.ReadValueU32(Endian.Little);
            if (magic != 0x32574F50 &&
                magic.Swap() != 0x32574F50) // 'POW2'
            {
                throw new FormatException("bad magic");
            }
            var endian = magic == 0x32574F50 ? Endian.Little : Endian.Big;

            var headerSize = input.ReadValueU32(endian);
            if (headerSize != 72)
            {
                throw new FormatException();
            }

            throw new NotImplementedException();
        }
    }
}
