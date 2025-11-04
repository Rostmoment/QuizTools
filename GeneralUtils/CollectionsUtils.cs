using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    public static class CollectionsUtils
    {
        public static string ToSeperateString<T>(this ICollection<T> values, string seperator = null)
        {
            if (values == null || values.Count == 0)
                return string.Empty;

            if (seperator == null)
                seperator = ".";

            StringBuilder sb = new StringBuilder();
            foreach (T value in values)
            {
                if (value != null)
                    sb.Append(value.ToString());
                sb.Append(seperator);
            }
            sb.Remove(sb.Length - seperator.Length, seperator.Length);
            return sb.ToString();
        }
        public static int[] IndexesOf<T>(this ICollection<T> values, Func<T, bool> func)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < values.Count; i++)
            {
                if (func(values.ElementAt(i)))
                    list.Add(i);
            }
            return list.ToArray();
        }
        public static int IndexOf<T>(this ICollection<T> values, Func<T, bool> func)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (func(values.ElementAt(i)))
                    return i;
            }
            return -1;
        }
    }
}
