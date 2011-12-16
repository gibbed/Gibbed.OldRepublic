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
    public class DataBlobs
    {
        public List<byte[]> Entries
            = new List<byte[]>();

        public void Serialize(MemoryStream output)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(MemoryStream input)
        {
            var magic = input.ReadValueU32(Endian.Little);
            if (magic != 0x424C4244 &&
                magic.Swap() != 0x424C4244)
            {
                throw new FormatException();
            }
            var endian = magic == 0x424C4244 ? Endian.Little : Endian.Big;

            var version = input.ReadValueU32(endian);
            if (version != 2)
            {
                throw new FormatException();
            }

            this.Entries.Clear();
            while (input.Position < input.Length)
            {
                // aligned to 8 bytes
                var padding = ((input.Position + 7) & ~7) - input.Position;
                if (padding > 0)
                {
                    input.Seek(padding, SeekOrigin.Current);
                }

                var length = input.ReadValueU32(endian);
                if (length == 0)
                {
                    break;
                }

                input.Seek(-4, SeekOrigin.Current);
                if (input.Position + length > input.Length)
                {
                    throw new FormatException();
                }

                using (var data = input.ReadToMemoryStream(length))
                {
                    var blob = new DataBlob();
                    blob.Deserialize(input, endian);
                }
            }
        }
    }
}
