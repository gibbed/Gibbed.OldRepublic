﻿/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
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
using Gibbed.TheOldRepublic.FileFormats;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using NDesk.Options;

namespace Gibbed.TheOldRepublic.Unpack
{
    internal class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool showHelp = false;
            bool? extractUnknowns = null;
            bool overwriteFiles = false;
            bool verbose = true;
            string currentProject = null;

            var options = new OptionSet()
            {
                {
                    "o|overwrite",
                    "overwrite existing files",
                    v => overwriteFiles = v != null
                },
                {
                    "nu|no-unknowns",
                    "don't extract unknown files",
                    v => extractUnknowns = v != null ? false : extractUnknowns
                },
                {
                    "ou|only-unknowns",
                    "only extract unknown files",
                    v => extractUnknowns = v != null ? true : extractUnknowns
                },
                {
                    "v|verbose",
                    "be verbose",
                    v => verbose = v != null
                },
                {
                    "h|help",
                    "show this message and exit", 
                    v => showHelp = v != null
                },
                {
                    "p|project=",
                    "override current project",
                    v => currentProject = v
                },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extras.Count < 1 ||
                extras.Count > 2 ||
                showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_file.tor [output_dir]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string inputPath = extras[0];
            string outputPath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(inputPath, null) + "_unpack";

            var manager = ProjectData.Manager.Load(currentProject);
            if (manager.ActiveProject == null)
            {
                Console.WriteLine("Warning: no active project loaded.");
            }
            var hashes = manager.LoadListsFileNames();

            var archive = new MythicArchiveFile();
            using (var input = File.OpenRead(inputPath))
            {
                archive.Deserialize(input);

                long current = 0;
                long total = archive.Entries.Count;

                foreach (var entry in archive.Entries)//.OrderBy(e => e.Offset))
                {
                    current++;

                    string name = hashes[entry.NameHash];
                    if (name == null)
                    {
                        if (extractUnknowns.HasValue == true &&
                            extractUnknowns.Value == false)
                        {
                            continue;
                        }

                        KeyValuePair<string, string> detected;
                        // detect type
                        {
                            var guess = new byte[64];
                            int read = 0;

                            if (entry.Flags == 0)
                            {
                                input.Seek(entry.Offset + entry.HeaderSize, SeekOrigin.Begin);
                                read = input.Read(guess, 0, (int)Math.Min(
                                    entry.DataUncompressedSize, guess.Length));
                            }
                            else if (entry.Flags == 1)
                            {
                                input.Seek(entry.Offset + entry.HeaderSize, SeekOrigin.Begin);
                                var zlib = new InflaterInputStream(input);
                                read = zlib.Read(guess, 0, (int)Math.Min(
                                    entry.DataUncompressedSize, guess.Length));
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }

                            detected = FileExtensions.Detect(guess, Math.Min(guess.Length, read));
                        }

                        name = entry.NameHash.ToString("X16");
                        name = Path.ChangeExtension(name, "." + detected.Value);
                        name = Path.Combine(detected.Key, name);
                        name = Path.Combine("__UNKNOWN", name);
                    }
                    else
                    {
                        if (extractUnknowns.HasValue == true &&
                            extractUnknowns.Value == true)
                        {
                            continue;
                        }

                        name = name.Replace("/", "\\");
                        if (name.StartsWith("\\") == true)
                        {
                            name = name.Substring(1);
                        }
                    }

                    var entryPath = Path.Combine(outputPath, name);

                    if (overwriteFiles == false &&
                        File.Exists(entryPath) == true)
                    {
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                    if (verbose == true)
                    {
                        Console.WriteLine("[{0}/{1}] {2}",
                            current, total, name);
                    }

                    using (var output = File.Create(entryPath))
                    {
                        if (entry.Flags == 0)
                        {
                            input.Seek(entry.Offset + entry.HeaderSize, SeekOrigin.Begin);
                            output.WriteFromStream(input, entry.DataCompressedSize);
                        }
                        else if (entry.Flags == 1)
                        {
                            input.Seek(entry.Offset + entry.HeaderSize, SeekOrigin.Begin);
                            var zlib = new InflaterInputStream(input);
                            output.WriteFromStream(zlib, entry.DataUncompressedSize);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                }
            }
        }
    }
}
