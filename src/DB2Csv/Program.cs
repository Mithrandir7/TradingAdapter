using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;

namespace DB2Csv
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: DB2Csv <symbol> <YYYYMMDD>");
                return;
            }

            string symbol = args[0].Trim();
            string yyyymmdd = args[1].Trim();

            RedisClient redisClient = new RedisClient();

            string keyPattern = symbol.Trim() + ":" + yyyymmdd.Trim() + "*";

            List<string> lkeyrtn = redisClient.SearchKeys(keyPattern);
            
            List<int> hhmmssList = new List<int>();
            Dictionary<int, string> hiD = new Dictionary<int, string>();
            Dictionary<int, string> loD = new Dictionary<int, string>();
            Dictionary<int, string> vD = new Dictionary<int, string>();
            Dictionary<int, string> oiD = new Dictionary<int, string>();

            foreach (string lstr in lkeyrtn)
            {
                string[] larr = lstr.Split(':');


                int hhmmss = Convert.ToInt32(larr[2]);
                hhmmssList.Add(hhmmss);

                string lhi = redisClient.GetValueFromHash(lstr, "h");
                hiD.Add(hhmmss, lhi);

                string llo = redisClient.GetValueFromHash(lstr, "l");
                loD.Add(hhmmss, llo);

                string lv = redisClient.GetValueFromHash(lstr, "v");
                vD.Add(hhmmss, lv);

                string loi = redisClient.GetValueFromHash(lstr, "oi");
                oiD.Add(hhmmss, loi);

                //Console.WriteLine("{0},{1},{2},{3},{4},{5}",yyyymmdd,hhmmss,lhi,llo,lv,loi);
            }

            hhmmssList.Sort();
            foreach (int lhhmmss in hhmmssList)
            {
                string lhi = hiD[lhhmmss];
                string llo = loD[lhhmmss];
                string lv = vD[lhhmmss];
                string loi = oiD[lhhmmss];

                Console.WriteLine("{0},{1},{2},{3},{4},{5}", yyyymmdd, lhhmmss, lhi, llo, lv, loi);
            }

            //Console.WriteLine("Press any key too continue...");
            //Console.ReadLine();

        }

        private static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private static string ByteArrayToString(byte[] dBytes)
        {
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            return enc.GetString(dBytes);
        }
    }
}
