using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizTools.GeneralUtils;

namespace QuizTools.Categories
{
    class Category
    {
        public static List<Category> all = new List<Category>();
        public string Name { get; }
        public List<CategoryOption> options;
        
        public Category(string name)
        {
            if (all.Exists(x => x.Name == name))
                throw new ArgumentException($"Kahoot with name {name} already exists!");
            Name = name;
            options = new List<CategoryOption>();
            all.Add(this);
        }
        public void WriteName()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(Name);
            Console.ResetColor();
            Console.Write(string.Concat(Enumerable.Repeat("=", Console.WindowWidth - Name.Length)));
        }

        public Category AddOption(string name, string desctiption, Action action) => AddOption(new CategoryOption(name, desctiption, action));
        public Category AddOption(CategoryOption option)
        {
            options.Add(option);
            return this;
        }
    }
}
