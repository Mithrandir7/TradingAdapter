using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityClass;

namespace OrderInformation
{
    public class OrderBehaviorParameters
    {
        //behavior parameters with default value

        public int orderId;
        public bool daytrade = false;
        public double profittakepercent = -9999;
        public double protectiontrigger = -9999;
        public double protection = -9999;
        public double hardstop = -9999;

        private String getRedisHeader()
        {
            return "OrderBehaviorParameters:" + orderId.ToString().Trim() + ":";
        }

        public void saveOnRedis()
        {
            RedisUtil.Instance.set(getRedisHeader() + "daytrade",daytrade.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "profittakepercent", profittakepercent.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "protectiontrigger", protectiontrigger.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "protection", protection.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "hardstop", hardstop.ToString());
        }

        public void loadFromRedis(int aOrderId)
        {
            orderId = aOrderId;
            daytrade = RedisUtil.Instance.getBool(getRedisHeader() + "daytrade");
            profittakepercent = RedisUtil.Instance.getDouble(getRedisHeader() + "profittakepercent");
            protectiontrigger = RedisUtil.Instance.getDouble(getRedisHeader() + "protectiontrigger");
            protection = RedisUtil.Instance.getDouble(getRedisHeader() + "protection");
            hardstop = RedisUtil.Instance.getDouble(getRedisHeader() + "hardstop");
        }


    }
}
