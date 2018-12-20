// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;

namespace BITSManager
{
    public class TabifyHttpHeaders
    {
        /// <summary>
        /// Header:Value prettifier; converts HEADER:VALUE into \tHEADER:\t\t\tVALUE.
        /// </summary>
        /// <param name="str">Input HTTP headers, possibly split on multiple lines with \r\n</param>
        /// <returns>String with all the headers and value but separated with tabs</returns>
        public static string AddTabs(string str)
        {
            var sb = new StringBuilder();
            int colonFoundCount = 0;
            int charsBeforeFirstColon = 0;
            foreach (var ch in str)
            {
                if (ch == '\n')
                {
                    sb.Append(ch);
                    sb.Append('\t');
                    colonFoundCount = 0;
                    charsBeforeFirstColon = 0;
                }
                else if (ch == ':')
                {
                    sb.Append(ch);

                    if (colonFoundCount == 0)
                    {
                        var tabs = GetAlignedSpaces(charsBeforeFirstColon + 1); // Include the colon
                        sb.Append(tabs);
                    }
                    colonFoundCount++;
                }
                else
                {
                    sb.Append(ch);
                    charsBeforeFirstColon++;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Given a string, return the right number of spaces needed after the string
        /// so that a series of strings is neatly aligned.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetAlignedSpaces(int length)
        {
            const int TabSize = 8;
            const int TargetLength = 2 * TabSize;

            // Handle the case of length being larger than TargetLength.
            // We want some N such that (length + N) % TabSize == 0
            // Otherwise we just want to know how many spaces it takes
            // to get to the desired size.
            // What makes this a little more complex is that if we're
            // bigger than the target size then we still want things
            // to line up a little bit neatly.
            var missingLength = length < 0
                ? 1
                : (length >= TargetLength)
                    ? TabSize - (length % TabSize)
                    : TargetLength - length;

            string tabs = "";
            for (int i = 0; i < missingLength; i++)
            {
                tabs = tabs + " ";
            }
            return tabs;
        }
    }
}
