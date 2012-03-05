using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityClass;
namespace DataManager
{
    public class RedisQuoteDataHandler
    {

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool isSaveQuoteData = true;

        private int yyyymmdd = DateTimeFunc.getYYYYMMDD();

        private RedisUtil redisUtility;

        public static RedisQuoteDataHandler Instance = new RedisQuoteDataHandler();

        private RedisQuoteDataHandler()
        {
            redisUtility = RedisUtil.getNewInstance();
            configuration();
        }

        private void configuration()
        {
            //String lsaveCfg = ConfigurationManager.AppSettings["isSaveQuote"].ToUpper().Trim();
            logger.Info("RedisQuoteDataHandler : isSaveQuote = " + isSaveQuoteData);
        }

        public void insertQuote(string symbol, int hhmmss, double hi, double lo, int vol, int oi)
        {
            if (!isSaveQuoteData)
            {
                return;
            }

            string key = symbol + ":" + yyyymmdd.ToString().Trim() + ":" + hhmmss.ToString().Trim();
            //Console.WriteLine("key insert " + key);
            redisUtility.Hset(key, Misc.StrToByteArray("h"), Misc.StrToByteArray(String.Format("{0:F2}", hi)));
            redisUtility.Hset(key, Misc.StrToByteArray("l"), Misc.StrToByteArray(String.Format("{0:F2}", lo)));
            redisUtility.Hset(key, Misc.StrToByteArray("v"), Misc.StrToByteArray(String.Format("{0}", vol)));
            redisUtility.Hset(key, Misc.StrToByteArray("oi"), Misc.StrToByteArray(String.Format("{0}", oi)));
        }

        public void save()
        {
            redisUtility.save();
        }
    }
}
