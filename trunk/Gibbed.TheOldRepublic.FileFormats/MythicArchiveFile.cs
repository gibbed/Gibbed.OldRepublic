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
    public class MythicArchiveFile
    {
        public Endian Endian;
        public uint Version;
        public uint DefaultFileTableSize;
        public uint Unknown18;
        public uint WriteSequence;
        public uint ConfirmSequence;
        public List<Archive.Entry> Entries
            = new List<Archive.Entry>();

        public void Serialize(Stream output)
        {
            throw new FormatException();
        }

        public void Deserialize(Stream input)
        {
            if (input.ReadValueU32(Endian.Little) != 0x0050594D) // 'MYP\0'
            {
                throw new FormatException("bad magic");
            }

            input.Seek(4, SeekOrigin.Current);
            
            var marker = input.ReadValueU32(Endian.Little);
            if (marker != 0xFD23EC43 &&
                marker.Swap() != 0xFD23EC43)
            {
                throw new FormatException("bad marker");
            }
            var endian = marker == 0xFD23EC43 ? Endian.Little : Endian.Big;

            input.Seek(-8, SeekOrigin.Current);
            var version = input.ReadValueU32(endian);
            //if (version < 4 || version > 5)
            if (version != 5)
            {
                throw new FormatException();
            }
            input.Seek(4, SeekOrigin.Current);

            this.Version = version;
            this.Endian = endian;

            this.Entries.Clear();

            var initialFileTableOffset = input.ReadValueS64(endian);
            this.DefaultFileTableSize = input.ReadValueU32(endian);
            this.Unknown18 = input.ReadValueU32(endian);
            this.WriteSequence = input.ReadValueU32(endian);
            this.ConfirmSequence = input.ReadValueU32(endian);

            var nextFileTableOffset = initialFileTableOffset;
            while (nextFileTableOffset != 0)
            {
                input.Seek(nextFileTableOffset, SeekOrigin.Begin);
                var fileTableSize = input.ReadValueU32(endian);
                nextFileTableOffset = input.ReadValueS64(endian);

                for (uint i = 0; i < fileTableSize; i++)
                {
                    var entry = new Archive.Entry();
                    entry.Offset = input.ReadValueS64(endian);
                    entry.HeaderSize = input.ReadValueU32(endian);
                    entry.DataCompressedSize = input.ReadValueU32(endian);
                    entry.DataUncompressedSize = input.ReadValueU32(endian);
                    entry.NameHash = input.ReadValueU64(endian);
                    entry.DataHash = input.ReadValueU32(endian);
                    entry.Flags = input.ReadValueU16(endian);

                    if (entry.Flags != 0 &&
                        entry.Flags != 1)
                    {
                        throw new FormatException();
                    }

                    this.Entries.Add(entry);
                }
            }
        }
    }
}
