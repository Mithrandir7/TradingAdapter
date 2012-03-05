using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountClass
{
    public class Position
    {
        public string broker;
        public string account;
        public string symbolid;
        public string symbolname;
        public string month;
        public string buysell;
        public string qty;
        public string price;
        public string exchange;
        public string billtype;
        public string time;
        public string symbol;

        public string info()
        {
            string rtnStr = time + "/" + broker + "/" + account + "/" + symbolid + "/" + symbolname + "/" +
                month + "/" + buysell + "/" + qty + "/" + price + "/" +
                exchange + "/" + billtype + "/" + symbol + "\n";
            return (rtnStr);
        }

        public bool Equals(Position pos)
        {
            // If parameter is null return false:
            if ((object)pos == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.info().Equals(pos.info()));
        }


    }
}
