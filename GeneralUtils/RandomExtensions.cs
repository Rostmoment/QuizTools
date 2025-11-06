using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    public static class RandomExtensions
    {
        public static int RandomSign(this Random random)
        {
            if (random.Next(0, 2) == 0)
                return -1;
            else
                return 1;
        
        }
        public static bool RandomBool(this Random random) => random.Next(0, 2) == 0;
        public static T ChoseRandom<T>(this Random random, params IEnumerable<T> options)
        {
            if (options == null || !options.Any())
                return default;

            int index = random.Next(0, options.Count());
            return options.ElementAt(index);
        }
        
    }
}
