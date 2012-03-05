using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderClass
{
    public class OrderCmd
    {
        public int orderid;
        public string account;
        public string symbol;
        public int position = 0;
        public bool daytrade = false;
        public double profittakepercent = -9999;
        public double protectiontrigger = -9999;
        public double protection = -9999;
        public double hardstop = -9999;
        public bool closed = false;

        public string getInfo()
        {
            return "OrderID:" + orderid + " / " +
                "account:" + account + " / " +
                "symbol:" + symbol + " / " +
                "position:" + position + " / " +
                "daytrade:" + daytrade + " / " +
                "profittakepercent:" + profittakepercent + " / " +
                "protectiontrigger:" + protectiontrigger + " / " +
                "protection:" + protection + " / " +
                "hardstop:" + hardstop;
        }
    }

 
}
