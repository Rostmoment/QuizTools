using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Kahoot
{
    class KahootChoice(string text, bool correct, ImageMetaData image)
    {
        public string Text { get; } = text;
        public bool IsCorrect { get; } = correct;
        public ImageMetaData Image { get; } = image;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            if (!string.IsNullOrEmpty(Text))
                b.Append(Text);
            if (Image != null)
                b.Append($" [{Image}] ");
            return b.ToString();
        }
    }
}
