using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Categories
{
    class BrowserOption : CategoryOption
    {
        public override string Description => string.Format(description, Settings.Current.Browser.ToString());
        public BrowserOption(string name, string description, Action action) : base(name, description, action)
        {
        }
    }
}
