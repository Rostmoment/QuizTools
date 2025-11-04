using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizTools.GeneralUtils
{
    public static class DatetimeUtils
    {
        public static DateTime FromUnixTime(long time) => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(time).ToLocalTime();

        public static long ToUnixTimeMilliseconds(this DateTime time) => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
        
    }
}
