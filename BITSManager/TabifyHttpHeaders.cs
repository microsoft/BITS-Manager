using System;
using System.Text;

namespace BITSManager
{
    internal class TabifyHttpHeaders
    {
        /// <summary>
        /// Header:Value prettifier; converts HEADER:VALUE into \tHEADER:\t\t\tVALUE.
        /// </summary>
        /// <param name="str">Input HTTP headers, possibly split on multiple lines with \r\n</param>
        /// <returns>String with all the headers and value but seperated with tabs</returns>
        public static string AddTabs(string str)
        {
            var sb = new StringBuilder();
            int nColonFound = 1;
            int charsBeforeFirstColon = 0;
            foreach (var ch in str)
            {
                if (ch == '\n')
                {
                    sb.Append(ch);
                    sb.Append('\t');
                    nColonFound = 0;
                    charsBeforeFirstColon = 0;
                }
                else if (ch == ':')
                {
                    sb.Append(ch);

                    if (nColonFound == 0)
                    {
                        var tabs = GetTabs(charsBeforeFirstColon + 1); // Include the colon
                        sb.Append(tabs);
                    }
                    nColonFound++;
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
        /// Given a string, return the right number of tabs needed after the string
        /// so that a series of strings is neatly aligned.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static private string GetTabs(int length)
        {
            const double TabSize = 8.0;
            const char TabChar = '\t';
            const double TargetLength = 32.0;

            var missingLength = TargetLength - length;
            var ntab = Math.Ceiling(missingLength / TabSize);
            ntab = Math.Max(1, ntab); // Always show at least one tab

            string tabs = "";
            for (int i = 0; i < ntab; i++)
            {
                tabs = tabs + TabChar;
            }
            return tabs;
        }
    }
}