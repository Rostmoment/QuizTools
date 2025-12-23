using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    class ColorHelper
    {
        public static List<string> GetGradient(string startHex, string endHex, int steps)
        {
            if (startHex[0] != '#')
                startHex = "#" + startHex;

            if (endHex[0] != '#')
                endHex = "#" + endHex;

            Color start = ColorTranslator.FromHtml(startHex);
            Color end = ColorTranslator.FromHtml(endHex);
            List<string> result = new List<string>();

            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / (steps - 1);

                int r = (int)(start.R + (end.R - start.R) * t);
                int g = (int)(start.G + (end.G - start.G) * t);
                int b = (int)(start.B + (end.B - start.B) * t);

                Color current = Color.FromArgb(r, g, b);
                result.Add($"{current.R:x2}{current.G:x2}{current.B:x2}");
            }

            return result;
        }
        public static bool IsValidHex(string hex)
        {
            if (hex.IsNullOrEmpty()) 
                return false;

            if (hex[0] != '#')
                hex = "#" + hex;
            try
            {
                Color color = ColorTranslator.FromHtml(hex);
                return true;
            }
            catch (Exception e) when (e is FormatException || e is ArgumentException)
            {
                return false;
            }
        }
        public static (byte r, byte g, byte b) HexToRgb(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = (byte)Convert.ToInt32(hex.Substring(0, 2), 16);
            byte g = (byte)Convert.ToInt32(hex.Substring(2, 2), 16);
            byte b = (byte)Convert.ToInt32(hex.Substring(4, 2), 16);
            return (r, g, b);
        }
        public static string RgbToHex(byte r, byte g, byte b) => $"{r:x2}{g:x2}{b:x2}";
    }
}
