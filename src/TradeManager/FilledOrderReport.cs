using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeManager
{
    public class FilledOrderReport
    {
        public string abbrname;
        public string time;
        public string orderNumber;
        public string account;
        public string buySellStr;
        public double avgPrice;
        public string status;
        public string iceid;
        public double filledNumber;
        public string memo;
        public string userkey;


        public string info()
        {
            string rtnStr = abbrname + "/" + time + "/" + orderNumber + "/" + account + "/" + buySellStr + "/" +
                Convert.ToString(avgPrice) + "/ Lots: " + "/" + status + "/" + iceid + "/" + Convert.ToString(filledNumber) + "/" +
                memo + "/" + userkey;
            return (rtnStr);
        }

        public int getHHMMSS()
        {
            string[] sp = time.Split(':');
            if (sp.Length == 3)
            {
                return (10000 * Convert.ToInt32(sp[0]) + 100 * Convert.ToInt32(sp[1]) + Convert.ToInt32(sp[2]));
            }
            else
            {
                return (999999);
            }

        }
    }
}
