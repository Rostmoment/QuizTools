using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Categories
{
    class DelayOption : CategoryOption
    {
        public override string Description => string.Format(description, Settings.Current.MinDelay, Settings.Current.MaxDelay);
        public DelayOption(string name, string description, Action action) : base(name, description, action)
        {
        }
    }
}
