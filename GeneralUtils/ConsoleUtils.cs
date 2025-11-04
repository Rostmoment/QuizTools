using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    class ConsoleUtils
    {
        public static ConsoleColor SavedForeground
        {
            get => saved.fg;
            set => saved.fg = value;
        }
        public static ConsoleColor SavedBackground
        {
            get => saved.bg;
            set => saved.bg = value;
        }
        private static (ConsoleColor fg, ConsoleColor bg) saved;

        public static void WriteInCenter(string text) =>
            Console.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (text.Length / 2)) + "}", text));

        public static void DeleteLastSymbols(int toDelete)
        {
            if (toDelete <= 0) return;

            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;

            if (currentLeft >= toDelete)
            {
                Console.SetCursorPosition(currentLeft - toDelete, currentTop);
                Console.Write(new string(' ', toDelete));
                Console.SetCursorPosition(currentLeft - toDelete, currentTop);
            }
            else
            {
                int remainingDelete = toDelete;
                int targetLeft = currentLeft;
                int targetTop = currentTop;

                while (remainingDelete > 0 && targetTop >= 0)
                {
                    if (targetLeft >= remainingDelete)
                    {
                        targetLeft -= remainingDelete;
                        remainingDelete = 0;
                    }
                    else
                    {
                        remainingDelete -= targetLeft;
                        targetTop--;
                        targetLeft = Console.BufferWidth - 1;
                    }
                }

                if (targetTop >= 0)
                {
                    Console.SetCursorPosition(targetLeft, targetTop);
                    Console.Write(new string(' ', toDelete));
                    Console.SetCursorPosition(targetLeft, targetTop);
                }
            }
        }
        public static void DeleteLastCharacter()
        {
            if (Console.CursorLeft > 0)
            {
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(' ');
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }
        public static void Center2Strings(string str1, string str2, string divider = "||")
        {
            int padding = (Console.WindowWidth / 2) - str1.Length;
            if (padding < 0) padding = 0; 
            Console.WriteLine(str1 + new string(' ', padding) + divider + " " + str2);
        }
        public static void WriteToEndOfLine(string text, string prefix = null, string postfix = null)
        {
            StringBuilder b = new StringBuilder();
            while (true)
            {
                b.Append(text);
                if (b.Length >= Console.WindowWidth)
                    break;
            }
            if (b.Length > Console.WindowWidth)
                b = b.Remove(Console.WindowWidth, b.Length - Console.WindowWidth);

            if (prefix != null)
            {
                b = b.Remove(0, prefix.Length);
                b.Insert(0, prefix);
            }

            if (postfix != null)
            {
                b = b.Remove(b.Length - postfix.Length, postfix.Length);
                b.Append(postfix);
            }

            Console.WriteLine(b.ToString());
        }

        public static void SaveColors()
        {
            SavedForeground = Console.ForegroundColor;
            SavedBackground = Console.BackgroundColor;
        }
        public static void RestoreColors()
        {
            Console.ForegroundColor = SavedForeground;
            Console.BackgroundColor = SavedBackground;
        }
    }
}
