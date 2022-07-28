using System;
using System.Text.RegularExpressions;

namespace Console_Launcher
{
    class log4j
    {

        public static void Print(string str)
        {
            string timestamp = Regex.Match(str, ".*timestamp=\"(.+?)\"").Groups[1].Value;
            if (string.IsNullOrEmpty(timestamp))
            {
                var msg = Regex.Match(str, ".*<log4j:Message><!\\[CDATA\\[(.+?)]").Groups[1].Value;
                if (string.IsNullOrEmpty(msg)) return;
                Console.WriteLine( msg );
                return;
            }

            string level = Regex.Match(str, ".*level=\"(.+?)\"").Groups[1].Value;
            string thread = Regex.Match(str, ".*thread=\"(.+?)\"").Groups[1].Value;

            Console.Write( "[{0}] {1}: ", level, thread );
        }

    }
}
