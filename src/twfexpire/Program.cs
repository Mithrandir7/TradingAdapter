using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace twfexpire
{
    class Program
    {
        static private List<DateTime> expireDayList = new List<DateTime>();

        static void Main(string[] args)
        {

            expireDayList.Add(new DateTime(2011,12,21));

            DateTime lnow = DateTime.Now;
            DateTime lastExpire = lnow.AddHours(-24);
            foreach (DateTime dt in expireDayList)
            {
                if (dt > lnow)
                {
                    lastExpire = dt;
                    break;
                }
            }

            if (lastExpire == lnow.AddHours(-24))
            {
                Console.WriteLine("No Data");
                return;
            }

            Console.WriteLine("ExpireDay:"+lastExpire.ToShortDateString());
            Console.WriteLine("Left:" + lastExpire.Subtract(lnow).Days);

        }
    }
}
