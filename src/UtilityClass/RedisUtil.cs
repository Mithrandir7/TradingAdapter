using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;
using System.Windows.Forms;


namespace UtilityClass
{
    public class RedisUtil
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        

        public static RedisUtil Instance = new RedisUtil();

        public static RedisUtil getNewInstance()
        {
            return new RedisUtil();
        }

        public static RedisClient getRedisClientInstance()
        {
            return new RedisClient(RedisConfig.Instance.host,RedisConfig.Instance.port);
        }

        
        private RedisClient redisClient;        

        private RedisUtil()
        {
            redisClient = new RedisClient(RedisConfig.Instance.host, RedisConfig.Instance.port);            
   
        }

        public List<String> search(String searchStr)
        {
            List<String> lrtn = null;

            try
            {
                lrtn = redisClient.SearchKeys(searchStr);
                
            }
            catch (RedisException e)
            {
                logger.Info(e.Message);
                MessageBox.Show(e.Message);
                Environment.Exit(1);
            }

            if (lrtn == null)
            {
                lrtn = new List<string>();
            }
            

            return lrtn;
        }

        public void set(String key, String value)
        {
            redisClient.Set(key, UtilityClass.Misc.StrToByteArray(value));
        }


        public String get(String key)
        {
            byte[] gb = redisClient.Get(key);
            String lrtn = UtilityClass.Misc.ByteArrayToString(gb);
            return lrtn;
        }

        public long getLong(String key)
        {
            byte[] gb = redisClient.Get(key);
            String lrtn = UtilityClass.Misc.ByteArrayToString(gb);
            return long.Parse(lrtn);
        }

        public int getInt(String key)
        {
            byte[] gb = redisClient.Get(key);
            String lrtn = UtilityClass.Misc.ByteArrayToString(gb);
            return int.Parse(lrtn);
        }

        public double getDouble(String key)
        {
            byte[] gb = redisClient.Get(key);
            String lrtn = UtilityClass.Misc.ByteArrayToString(gb);
            return double.Parse(lrtn);
        }

        public void save()
        {
            redisClient.Save();
        }

        public void Hset(string hashid,byte[] key, byte[] value)
        {
            redisClient.HSet(hashid, key, value);
        }

        public void del(string [] keys)
        {
            redisClient.Del(keys);
        }
    

    }
}
