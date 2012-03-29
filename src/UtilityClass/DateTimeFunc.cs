using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityClass
{
    public class DateTimeFunc
    {
        public static int hhmmss2Secs(int hhmmss)
        {
            int lsec = 0;
            int ss = hhmmss % 100;
            int mm = ((hhmmss - ss) / 100) % 100;
            int hh = (hhmmss - 100 * mm - ss) / 10000;
            //logger.Info("hh,mm,ss" + hh + "," + mm + "," + ss);
            lsec = hh * 60 * 60 + mm * 60 + ss;
            return (lsec);
        }

        public static int nSecondsBefore(int ahhmmss, int nSecs)
        {
            return secs2hhmmss(hhmmss2Secs(ahhmmss) - nSecs);
        }

        public static int getYYYYMMDD()
        {
            DateTime dt = DateTime.Now;
            return (dt.Day + 100 * dt.Month + 10000 * dt.Year);
        }

        public static int getHHMMSS()
        {
            DateTime dt = DateTime.Now;
            return (dt.Second + 100 * dt.Minute + 10000 * dt.Hour);
        }

        public static int secs2hhmmss(int aSecs)
        {
            int hhmmss = 0;
            int hh = Convert.ToInt32(Math.Floor(aSecs / 3600.0));
            int mm = Convert.ToInt32(Math.Floor((aSecs - 3600 * hh) / 60.0));
            int ss = aSecs - 3600 * hh - 60 * mm;
            hhmmss = hh * 10000 + mm * 100 + ss;
            return (hhmmss);
        }

        public static String DateTimeToString(DateTime aDateTime)
        {
            return aDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime StringToDateTime(String aString)
        {
            return DateTime.ParseExact(aString, "yyyy-MM-dd HH:mm:ss", null);
        }

        public static int hhmmssDiff(int hhmmss, int hhmmssBase)
        {
            int lbegin = hhmmss2Secs(hhmmss);
            int lbase = hhmmss2Secs(hhmmssBase);
            return lbegin - lbase;
        }

    }
}
