using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.Vseosvita
{
    class VseosvitaEndTestInformation
    {
        #region for date parsing
        private static string[] formats = {
            "d MMMM yyyy 'року о' HH:mm",
            "d MMMM yyyy 'року' HH:mm",
            "d MMMM yyyy HH:mm",
            "d MMMM yyyy 'at' HH:mm",
            "d MMMM yyyy 'года в' HH:mm"
        };

        private static CultureInfo[] cultures = {
            new CultureInfo("uk-UA"),
            new CultureInfo("ru-RU"),
            new CultureInfo("en-US"),
            new CultureInfo("pl-PL")
        };
        #endregion

        public int Grade { get; }
        public DateTime FinishedAt { get; }
        public int DurationSeconds { get; }

        public VseosvitaEndTestInformation(JsonElement root)
        {
            Grade = int.Parse(string.Concat(root.GetProperty("data")
                                            .GetProperty("mark_explain")
                                            .GetProperty("core_info")
                                            .GetProperty("line1")
                                            .GetString()
                                            .TakeWhile(char.IsDigit)));

            string finishedAtStr = root.GetProperty("data").GetProperty("finished_at_str").GetString();
            foreach (CultureInfo culture in cultures)
            {
                if (DateTime.TryParseExact(finishedAtStr, formats, culture, DateTimeStyles.None, out DateTime result))
                {
                    FinishedAt = result;
                    break;
                }
            }

            string[] time = root.GetProperty("data").GetProperty("duration_str").GetString().Split(' ');
            if (time.Length == 6)
                DurationSeconds = int.Parse(time[0]) * 3600 + int.Parse(time[2]) * 60 + int.Parse(time[4]);
            else if (time.Length == 4)
                DurationSeconds = int.Parse(time[0]) * 60 + int.Parse(time[2]);
            else
                DurationSeconds = int.Parse(time[0]);
        }
    }
}
