using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataManager
{
    public class TickQuote
    {
        public string abbrname;
        public double time;
        public double trade;
        public double volume;
        public double bid;
        public double ask;
        public double oi;


        public string info()
        {
            string lstr = "OI : " + abbrname + " : " + time.ToString() + "," +
                Convert.ToString(trade) + "," + Convert.ToString(volume) + "," + Convert.ToString(bid) + "," +
                Convert.ToString(ask) + "," + Convert.ToString(oi);
            return (lstr);
        }

        public TickQuote(string aAbbrname, double atime, double atrade, double avolume, double abid, double aask, double aoi)
        {
            abbrname = aAbbrname;
            time = atime;
            trade = atrade;
            volume = avolume;
            bid = abid;
            ask = aask;
            oi = aoi;
        }
    }
}
