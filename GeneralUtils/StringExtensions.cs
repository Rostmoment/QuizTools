using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    public static class StringExtensions
    {
        public static string MakeHyperlink(this string text, string link)
        {
            const string ESC = "\u001B";
            return $"{ESC}]8;;{link}{ESC}\\{text}{ESC}]8;;{ESC}\\";
        }

    }
}
