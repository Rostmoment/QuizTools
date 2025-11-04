using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.Categories
{
    class CategoryOption
    {
        private static List<CategoryOption> _options = new List<CategoryOption>();
        public static int Count => _options.Count;

        protected string name;
        protected string description;
        protected Action action;

        public virtual string Name => name;
        public virtual string Description => description;
        public virtual Action Action => action;

        public CategoryOption(string name, string description, Action action)
        {
            this.name = name;
            this.description = description;
            this.action = action;
        }

        public void AddToList()
        {
            if (!_options.Contains(this))
                _options.Add(this);
        }
        public static void Invoke(int id)
        {
            _options[id-1].Action.Invoke();
        }
    }
}
