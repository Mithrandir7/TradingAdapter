using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymbolClass;
using UserkeyManager;
using UtilityClass;

namespace OrderInformation
{
    public class OrderInfo
    {

        public int orderId { get; set; }
        public string account { get; set; }
        public int position { get; set; }
        public string abbrName { get; set; }

        private string userkey { get; set; }
        private string closedUserkey { get; set; }


        private String getRedisHeader()
        {
            return "OrderInfo:" + orderId.ToString().Trim() + ":";
        }

        public void saveOnRedis()
        {
            RedisUtil.Instance.set(getRedisHeader() + "account", account);
            RedisUtil.Instance.set(getRedisHeader() + "position", position.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "abbrName", abbrName);
            RedisUtil.Instance.set(getRedisHeader() + "userkey", userkey);
            RedisUtil.Instance.set(getRedisHeader() + "closedUserkey", closedUserkey);
        }

        public void loadFromRedis(int aOrderId)
        {
            orderId = aOrderId;
            account = RedisUtil.Instance.get(getRedisHeader() + "account");
            position = RedisUtil.Instance.getInt(getRedisHeader() + "position");
            abbrName = RedisUtil.Instance.get(getRedisHeader() + "abbrName");
            userkey = RedisUtil.Instance.get(getRedisHeader() + "userkey");
            closedUserkey = RedisUtil.Instance.get(getRedisHeader() + "closedUserkey");

        }

        public OrderInfo()
        {
            userkey = UserkeyManager.UserkeyFactory.Instance.getUserkey();
            closedUserkey = userkey + "_closed";            
        }

        public String getUserkey()
        {
            return userkey;
        }

        public String getUserkeyClosed()
        {
            return closedUserkey;
        }

        public String getIceId()
        {
            return SymbolManager.Instance.getTradeSymbol(abbrName, account);
        }


        //public string info()
        //{
        //    string infoStr = abbrName + "/" + userkey + "/" + account + "/" + ICEID + "/" + Convert.ToString(position);
        //    return (infoStr);
        //}


  

    }
}
