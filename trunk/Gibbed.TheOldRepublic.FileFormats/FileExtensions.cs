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
using System.Text;
using Gibbed.IO;

namespace Gibbed.TheOldRepublic.FileFormats
{
    public static class FileExtensions
    {
        public static KeyValuePair<string, string> Detect(byte[] guess, int read)
        {
            if (read == 0)
            {
                return new KeyValuePair<string, string>("null", "null");
            }

            if (read >= 4 &&
                guess[0] == 'D' &&
                guess[1] == 'D' &&
                guess[2] == 'S' &&
                guess[3] == ' ')
            {
                return new KeyValuePair<string, string>("textures", "dds");
            }
            else if (
                read >= 4 &&
                guess[0] == 'F' &&
                guess[1] == 'A' &&
                guess[2] == 'C' &&
                guess[3] == 'E')
            {
                return new KeyValuePair<string, string>("facefx", "facefx");
            }
            else if (
                read >= 4 &&
                guess[0] == 'P' &&
                guess[1] == 'B' &&
                guess[2] == 'U' &&
                guess[3] == 'K')
            {
                return new KeyValuePair<string, string>("buckets", "bkt");
            }
            else if (
                read >= 4 &&
                guess[0] == 'P' &&
                guess[1] == 'R' &&
                guess[2] == 'O' &&
                guess[3] == 'T')
            {
                return new KeyValuePair<string, string>("prototypes", "node");
            }
            else if (
                read >= 3 &&
                guess[0] == 'G' &&
                guess[1] == 'F' &&
                guess[2] == 'X')
            {
                return new KeyValuePair<string, string>("scaleform", "gfx");
            }
            else if (
                read >= 3 &&
                guess[0] == 'C' &&
                guess[1] == 'F' &&
                guess[2] == 'X')
            {
                return new KeyValuePair<string, string>("scaleform", "gfx");
            }
            else if (
                read >= 3 &&
                guess[0] == 'F' &&
                guess[1] == 'W' &&
                guess[2] == 'S')
            {
                return new KeyValuePair<string, string>("scaleform", "swf");
            }
            else if (
                read >= 3 &&
                guess[0] == 'C' &&
                guess[1] == 'W' &&
                guess[2] == 'S')
            {
                return new KeyValuePair<string, string>("scaleform", "swf");
            }
            else if (
                read >= 4 &&
                guess[0] == 'G' &&
                guess[1] == 'A' &&
                guess[2] == 'W' &&
                guess[3] == 'B')
            {
                return new KeyValuePair<string, string>("granny", "gr2");
            }
            else if (
                read >= 5 &&
                guess[0] == '<' &&
                guess[1] == '?' &&
                guess[2] == 'x' &&
                guess[3] == 'm' &&
                guess[4] == 'l')
            {
                return new KeyValuePair<string, string>("xml", "xml");
            }
            else if (
                read >= 8 &&
                guess[0] == 0xEF &&
                guess[1] == 0xBB &&
                guess[2] == 0xBF &&
                guess[3] == '<' &&
                guess[4] == '?' &&
                guess[5] == 'x' &&
                guess[6] == 'm' &&
                guess[7] == 'l')
            {
                return new KeyValuePair<string, string>("xml", "xml");
            }
            else if (
                read >= 8 &&
                guess[0] == '<' &&
                guess[1] == 'b' &&
                guess[2] == 'l' &&
                guess[3] == 'o' &&
                guess[4] == 'c' &&
                guess[5] == 'k' &&
                guess[6] == 'e' &&
                guess[7] == 'd')
            {
                return new KeyValuePair<string, string>("blocked", "xml");
            }
            else if (
                read >= 11 &&
                guess[0] == 0xEF &&
                guess[1] == 0xBB &&
                guess[2] == 0xBF &&
                guess[3] == '<' &&
                guess[4] == 'b' &&
                guess[5] == 'l' &&
                guess[6] == 'o' &&
                guess[7] == 'c' &&
                guess[8] == 'k' &&
                guess[9] == 'e' &&
                guess[10] == 'd')
            {
                return new KeyValuePair<string, string>("blocked", "xml");
            }
            else if (read >= 23 &&
                guess[0] == '!' &&
                guess[1] == '\r' &&
                guess[2] == '\n' &&
                guess[3] == '!' &&
                guess[4] == ' ' &&
                Encoding.ASCII.GetString(guess, 5, 18) == "Area Specification")
            {
                return new KeyValuePair<string, string>("areas", "area");
            }
            else if (read >= 23 &&
                guess[0] == '!' &&
                guess[1] == '\r' &&
                guess[2] == '\n' &&
                guess[3] == '!' &&
                guess[4] == ' ' &&
                Encoding.ASCII.GetString(guess, 5, 18) == "Room Specification")
            {
                return new KeyValuePair<string, string>("areas", "room");
            }
            else if (
                read >= 24 &&
                guess[0] == 0xFF &&
                guess[1] == 0xFE &&
                Encoding.Unicode.GetString(guess, 2, 22) == "<Appearance")
            {
                return new KeyValuePair<string, string>("xml", "epp");
            }
            else if (
                read >= 22 &&
                guess[0] == 0xFF &&
                guess[1] == 0xFE &&
                Encoding.Unicode.GetString(guess, 2, 20) == "<DataTable")
            {
                return new KeyValuePair<string, string>("xml", "tbl");
            }
            else if (
                read >= 32 &&
                guess[0] == 0xFF &&
                guess[1] == 0xFE &&
                Encoding.Unicode.GetString(guess, 2, 30) == "<SurveyInstance")
            {
                return new KeyValuePair<string, string>("xml", "svy");
            }
            else if (
                read >= 22 &&
                Encoding.Unicode.GetString(guess, 0, 22) == "<Appearance")
            {
                return new KeyValuePair<string, string>("xml", "epp");
            }

            return new KeyValuePair<string, string>("unknown", "unknown");
        }
    }
}
