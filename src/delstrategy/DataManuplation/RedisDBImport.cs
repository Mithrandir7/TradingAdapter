using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Common.Extensions;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;

namespace delstrategy
{
    class RedisDBImport
    {
        private Paths paths;
        private string symbol;
        private RedisClient redisClient = new RedisClient();

        public RedisDBImport(string aSymbol, Paths aPaths)
        {
            symbol = aSymbol;
            paths = aPaths;           
        }

        public void importData()
        {
            List<Path> plist = paths.getPathList();
            foreach (Path lp in plist)
            {
                //showPathOutlier(lp);
                //showPathTimeSeqError(lp);
                importPath(lp);
            }
        }

        // C# to convert a string to a byte array.
        private byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding  encoding=new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private string ByteArrayToString(byte[] dBytes)
        {
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            return enc.GetString(dBytes);
        }

        private void importPath(Path aPath)
        {
            List<int> yyyymmdd = aPath.yyyymmdd;
            List<int> hhmmss = aPath.hhmmss;
            List<double> high = aPath.high;
            List<double> low = aPath.low;
            List<int> volume = aPath.volume;
            List<int> oi = aPath.oi;

            int[] lt = yyyymmdd.ToArray();
            for (int i = 0; i < aPath.high.Count; i++)
            {
                string key = symbol + ";" + yyyymmdd[i].ToString().Trim() + ":" + hhmmss[i].ToString().Trim();
                string value = String.Format("h {0:F2} l {1:F2} v {2} oi {3}", high[i],low[i],volume[i],oi[i]);
                redisClient.HSet(key, StrToByteArray("h"), StrToByteArray(String.Format("{0:F2}", high[i])));
                redisClient.HSet(key, StrToByteArray("l"), StrToByteArray(String.Format("{0:F2}", low[i])));
                redisClient.HSet(key, StrToByteArray("v"), StrToByteArray(String.Format("{0}", volume[i])));
                redisClient.HSet(key, StrToByteArray("oi"), StrToByteArray(String.Format("{0}", oi[i])));
            }
        }

        ~RedisDBImport()
        {
            redisClient.Save();            
        }
    }
}
