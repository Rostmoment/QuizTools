using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    public static class RandomUtils
    {
        public static int RandomSign(this Random random)
        {
            if (random.Next(0, 2) == 0)
                return -1;
            else
                return 1;
        }
        public static T ChoseRandom<T>(this Random random, params T[] options)
        {
            return options[random.Next(0, options.Length)];
        }
    }
}
