using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClient.Interop.Helpers
{
    public static class DateTimeConversion
    {
        public static DateTime UnixTimeToDateTime(long unixTime)
        {
            return UnixStartTime.AddSeconds(Convert.ToDouble(unixTime));
        }
        public static long DateTimeToUnixTime(DateTime date)
        {
            var timeSpan = date - UnixStartTime;
            return Convert.ToInt64(timeSpan.TotalSeconds);
        }
        private static readonly DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }
}
