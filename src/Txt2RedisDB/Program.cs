using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;

namespace Txt2RedisDB
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Tick2RedisDB <symbol> <filename>");
                return;
            }

            string symbol = args[0].Trim();
            string filename = args[1].Trim();
            //string filename = @"D:\WorkPlace\data_recent_oi\cu.txt";
            if (!File.Exists(filename))
            {
                Console.WriteLine("File "+filename+" not exist!");
                return;
            }

            string line = "";

            int lyyyymmdd = 0;
            int lhhmmss = 0;
            double lpz = -1;
            double hi = -9999;
            double lo = 9999;
            double lvol = 0;
            double loi = 0;

            RedisClient redisClient = new RedisClient();

            StreamReader file = null;
            try
            {
                file = new StreamReader(filename);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Length <= 0)
                    {
                        continue;
                    }
                    if (line.IndexOf("DATE") < 0)
                    {
                        string[] larr = line.Split(',');
                        //Console.WriteLine(larr[0].Trim());  
                        if (larr.Length == 7)
                        {
                            int lday = Convert.ToInt32(larr[0].Trim());
                            int ltime = Convert.ToInt32(larr[1].Trim());
                            lpz = Convert.ToDouble(larr[2].Trim());
                            int lvolume = Convert.ToInt32(larr[3].Trim());
                            int loit = Convert.ToInt32(larr[6].Trim());

                            if (lyyyymmdd == lday & lhhmmss == ltime)
                            {
                                hi = Math.Max(hi, lpz);
                                lo = Math.Min(lo, lpz);
                                lvol = lvol + lvolume;
                                loi = loit;
                            }
                            else
                            {
                                bool isValid = lyyyymmdd > 0 & lvol != 0;
                                isValid = isValid & hi>0 & hi<9999999;
                                isValid = isValid & lo>0 & lo < 9999999;
                                isValid = isValid & lvol>0 & lvol<999999999;
                                isValid = isValid & loi>0 & loi<999999999;

                                if (isValid)
                                {
                                    
                                    string key = symbol + ":" + lyyyymmdd.ToString().Trim() + ":" + lhhmmss.ToString().Trim();
                                    Console.WriteLine("key insert "+key);
                                    redisClient.HSet(key, StrToByteArray("h"), StrToByteArray(String.Format("{0:F2}", hi)));
                                    redisClient.HSet(key, StrToByteArray("l"), StrToByteArray(String.Format("{0:F2}", lo)));
                                    redisClient.HSet(key, StrToByteArray("v"), StrToByteArray(String.Format("{0}", lvol)));
                                    redisClient.HSet(key, StrToByteArray("oi"), StrToByteArray(String.Format("{0}", loi)));
                                }
                                lyyyymmdd = lday;
                                lhhmmss = ltime;
                                hi = lpz;
                                lo = lpz;
                                lvol = lvolume;
                                loi = loit;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            


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
