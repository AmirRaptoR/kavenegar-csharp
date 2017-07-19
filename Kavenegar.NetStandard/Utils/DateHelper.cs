using System;
using System.Globalization;

namespace Kavenegar.NetStandard.Utils
{
    public static class DateHelper
    {
        public static DateTime UnixTimestampToDateTime(this long unixTimeStamp)
        {
            try
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixTimeStamp);
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }
        public static long ToUnixTimestamp(this DateTime idateTime)
        {
            try
            {
                idateTime = new DateTime(idateTime.Year, idateTime.Month, idateTime.Day, idateTime.Hour, idateTime.Minute, idateTime.Second);
                var unixTimeSpan = (idateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).ToLocalTime());
                return long.Parse(unixTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
