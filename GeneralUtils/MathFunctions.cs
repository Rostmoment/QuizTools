using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    class MathFunctions
    {
        public static int BinomialCoefficient(int n, int k)
            => Factorial(n) / (Factorial(k) * Factorial(n - k));
        
        public static int Factorial(int a)
        {
            if (a == 1 || a == 1)
                return 1;
            int result = 1;
            for (int i = 1; i <= a; i++)
                result *= i;
            return result;
        }
        public static List<int[]> DivideIntoAddends(int n, int count, bool includeZero = false)
        {
            if (count == 1)
                return [[n]];

            List<int[]> result = new List<int[]>();
            if (count == 2)
            {
                for (int i = 1; i < n; i++)
                {
                    result.Add([i, n - i]);
                    result.Add([n - i, i]);
                }
                if (includeZero)
                {
                    result.Add([0, n]);
                    result.Add([n, 0]);
                }
                return result;
            }

            int minValue = includeZero ? 0 : 1;
            for (int i = minValue; i <= n; i++)
            {
                int remaining = n - i;
                var subAddends = DivideIntoAddends(remaining, count - 1, includeZero);
                foreach (var sub in subAddends)
                {
                    int[] combo = new int[count];
                    combo[0] = i;
                    Array.Copy(sub, 0, combo, 1, count - 1);
                    result.Add(combo);
                }
            }

            return result;
        }
        public static int WaysToDivideIntoAddends(int n, int count, bool includeZero = false)
        {
            if (includeZero)
                return BinomialCoefficient(n + count - 1, count - 1);
            return BinomialCoefficient(n - 1, count - 1);
        }
    }
}
