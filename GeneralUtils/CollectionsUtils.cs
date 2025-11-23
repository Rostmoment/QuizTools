using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    public static class CollectionsUtils
    {
        public static bool ArrayIsNullOrEmpty(this Array array) => array == null || array.Length == 0;
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
        {
            if (values == null)
                return true;
            return values.Count() == 0;
        }

        public static string ToSeparatedString<T>(this IEnumerable<T> values) => values.ToSeparatedString(".");
        public static string ToSeparatedString<T>(this IEnumerable<T> values, string seperator)
        {
            if (values.IsNullOrEmpty())
                return string.Empty;

            ArgumentNullException.ThrowIfNull(seperator, nameof(seperator));

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

        public static int[] IndexesOf<T>(this IEnumerable<T> values, Func<T, bool> func)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < values.Count(); i++)
            {
                if (func(values.ElementAt(i)))
                    list.Add(i);
            }
            return list.ToArray();
        }
        public static int IndexOf<T>(this IEnumerable<T> values, Func<T, bool> func)
        {
            for (int i = 0; i < values.Count(); i++)
            {
                if (func(values.ElementAt(i)))
                    return i;
            }
            return -1;
        }
    }
}
