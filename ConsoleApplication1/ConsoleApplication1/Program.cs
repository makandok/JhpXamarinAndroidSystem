using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var date2 = new DateTime(2016, 4, 7, 16, 1, 45, 1);
            var maxInt = int.MaxValue;
            var asInt = date2.toInt();

            var time = date2.TimeOfDay;
            var ts = time.ToString("hhmmss.fff");
            var g = date2.ToString("yyyyMMdd hhmmss.fff");
            var date = DateTime.Now.toSafeDate2();
            var t = new DateTime(2016, 4, 7).toSafeDate2();
            var n = DateTime.MinValue.toSafeDate2();
        }
    }
        public static class ExtensionsPCL { 
        public static long toSafeDate(this DateTime dateValue)
        {
            var timeofday = dateValue.TimeOfDay;
            return (dateValue.Year * 100 + dateValue.Month) * 100 + dateValue.Day+
                dateValue.Hour;
        }

        /// <summary>
        /// Converts date value to yyyMMdd format
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns>Returns as int represent date e.g. 20160130</returns>
        public static int toYMDInt(this DateTime dateValue)
        {
            var timeofday = dateValue.ToString("yyyyMMdd");
            return Convert.ToInt32(timeofday);
        }

        public static long toSafeDate2(this DateTime dateValue)
        {
            var timeofday = dateValue.TimeOfDay;
            var longDateValue =
                dateValue.ToString("yyyyMMdd") +
                timeofday.Hours.ToString().PadLeft(2, '0') +
                timeofday.Minutes.ToString().PadLeft(2, '0') +
                timeofday.Seconds.ToString().PadLeft(2, '0') +
                timeofday.Milliseconds.ToString().PadLeft(3, '0');
            return Convert.ToInt64(longDateValue);
        }

        public static DateTime fromSafeDate(this int safeDate)
        {
            var asString = safeDate.ToString();
            var day = Convert.ToInt16(asString.Substring(5, 2));
            var month = Convert.ToInt16(asString.Substring(3, 2));
            var year = Convert.ToInt16(asString.Substring(0, 4));
            return new DateTime(year, month, day);
        }
    }
}
