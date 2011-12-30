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
using Gibbed.IO;

namespace Gibbed.TheOldRepublic.FileFormats
{
    public class BucketFile
    {
        public void Deserialize(Stream input)
        {
            var magic = input.ReadValueU32(Endian.Little);
            if (magic != 0x4B554250 &&
                magic.Swap() != 0x4B554250)
            {
                throw new FormatException();
            }
            var endian = magic == 0x4B554250 ? Endian.Little : Endian.Big;

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

            var headerLength = input.ReadValueU32(endian);
            using (var data = input.ReadToMemoryStream(headerLength))
            {
                var blobFile = new DataBlobs();
                blobFile.Deserialize(data);
            }

            var dataLength = input.ReadValueU32(endian);
            using (var data = input.ReadToMemoryStream(dataLength))
            {
                var blobFile = new DataBlobs();
                blobFile.Deserialize(data);
            }
        }
    }
}
