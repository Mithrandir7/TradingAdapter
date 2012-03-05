using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderInformation
{
    public class OrderInfo
    {
    

            public long orderId { get; set; }
            public string account { get; set; }
            public int position { get; set; }
            public string abbrName { get; set; }
            public string userkey { get; set; }
            public string closedUserkey { get; set; }
            public string ICEID { get; set; }

            public OrderInfo()
            {

            }

            //public string info()
            //{
            //    string infoStr = abbrName + "/" + userkey + "/" + account + "/" + ICEID + "/" + Convert.ToString(position);
            //    return (infoStr);
            //}

  

    }
}
