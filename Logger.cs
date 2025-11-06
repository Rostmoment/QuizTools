using QuizTools.GeneralUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools
{
    public static class Logger
    {
        public static void WriteInfo(object message, object prefix = null)
        {
            
            ConsoleUtils.SaveColors();
            Console.ResetColor();
            Console.Write($"{(prefix != null ? prefix.ToString() : "")}[Info] {message}");
            ConsoleUtils.RestoreColors();

        }
        public static void WriteInfoLine(object message, object prefix = null)
        {
            ConsoleUtils.SaveColors();
            Console.ResetColor();
            Console.WriteLine($"{(prefix != null ? prefix.ToString() : "")}[Info] {message}");
            ConsoleUtils.RestoreColors();
        }
        public static void WriteWarning(object message, object prefix = null)
        {
            ConsoleUtils.SaveColors();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{(prefix != null ? prefix.ToString(): "")}[Warning] {message}");
            ConsoleUtils.RestoreColors();
        }
        public static void WriteWarningLine(object message, object prefix = null)
        {
            ConsoleUtils.SaveColors();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{(prefix != null ? prefix.ToString() : "")}[Warning] {message}");
            ConsoleUtils.RestoreColors();
        }
        public static void WriteError(object message, object prefix = null)
        {
            ConsoleUtils.SaveColors();
            Console.ForegroundColor= ConsoleColor.DarkRed;
            Console.Write($"{(prefix != null ? prefix.ToString() : "")}[Error] {message}");
            ConsoleUtils.RestoreColors();
        }
        public static void WriteErrorLine(object message, object prefix = null)
        {
            ConsoleUtils.SaveColors();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{(prefix != null ? prefix.ToString() : "")}[Error] {message}");
            ConsoleUtils.RestoreColors();
        }
    }
}
