using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Categories
{
    class LogoGradientOption : CategoryOption
    {
        public override string Description => string.Format(description, Settings.LogoGradientFrom, Settings.LogoGradientTo);
        public LogoGradientOption(string name, string description, Action action) : base(name, description, action)
        {
        }
    }
}
