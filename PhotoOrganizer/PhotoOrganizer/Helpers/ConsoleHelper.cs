using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.Helpers
{
    public class ConsoleHelper
    {
        private static int _progressLineTop;

        public static void WriteProgress(string text, bool isFirstLine = false)
        {
            if (!isFirstLine)
            {
                Console.SetCursorPosition(0, _progressLineTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, _progressLineTop);
            }
            else
            {
                _progressLineTop = Console.CursorTop;
            }
            Console.Write(text);
        }

        public static void WriteError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: {text}");
            Console.ResetColor();
        }
    }
}
