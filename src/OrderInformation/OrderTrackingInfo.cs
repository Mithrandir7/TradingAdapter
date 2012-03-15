using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityClass;

namespace OrderInformation
{
    public class OrderTrackingInfo
    {
        public OrderState orderState { get; set; }
        public long orderId { get; set; }
        public double entryPz { get; set; }
        public double closedPz { get; set; }
        public int entryOI { get; set; }
        public int closedOI { get; set; }
        public double currentProfit { get; set; }
        public double currentProfitPercent { get; set; }
        public double maxrunup { get; set; }
        public double maxdrawdown { get; set; }
        public DateTime filledTime { get; set; }
        public DateTime closedTime { get; set; }
        public DateTime closingTime { get; set; }

        public OrderTrackingInfo()
        {
            orderState = OrderState.Unknown;
            maxrunup = 0;
            maxdrawdown = 0;
        }

        public OrderTrackingInfo(OrderState aOrderState)
        {
            orderState = aOrderState;
        }

        private String getRedisHeader()
        {
            return "OrderTrackingInfo:" + orderId.ToString().Trim() + ":";
        }

        public void saveOnRedis()
        {
            RedisUtil.Instance.set(getRedisHeader() + "orderState", orderState.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "entryPz", entryPz.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "closedPz", closedPz.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "entryOI", entryOI.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "closedOI", closedOI.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "currentProfit", currentProfit.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "currentProfitPercent", currentProfitPercent.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "maxrunup", maxrunup.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "maxdrawdown", maxdrawdown.ToString());
            RedisUtil.Instance.set(getRedisHeader() + "filledTime", DateTimeFunc.DateTimeToString(filledTime));
            RedisUtil.Instance.set(getRedisHeader() + "closedTime", DateTimeFunc.DateTimeToString(closedTime));
            RedisUtil.Instance.set(getRedisHeader() + "closingTime", DateTimeFunc.DateTimeToString(closingTime));
        }

        public void loadFromRedis(int aOrderId)
        {
            orderId = aOrderId;
            
            orderState = (OrderState)Enum.Parse(typeof(OrderState), RedisUtil.Instance.get(getRedisHeader() + "orderState"));
            entryPz = RedisUtil.Instance.getDouble(getRedisHeader() + "entryPz");
            closedPz = RedisUtil.Instance.getDouble(getRedisHeader() + "closedPz");
            entryOI = RedisUtil.Instance.getInt(getRedisHeader() + "entryOI");
            closedOI = RedisUtil.Instance.getInt(getRedisHeader() + "closedOI");
            currentProfit = RedisUtil.Instance.getDouble(getRedisHeader() + "currentProfit");
            currentProfitPercent = RedisUtil.Instance.getDouble(getRedisHeader() + "currentProfitPercent");
            maxrunup = RedisUtil.Instance.getDouble(getRedisHeader() + "maxrunup");
            maxdrawdown = RedisUtil.Instance.getDouble(getRedisHeader() + "maxdrawdown");
            filledTime = DateTimeFunc.StringToDateTime(RedisUtil.Instance.get(getRedisHeader() + "filledTime"));
            closedTime = DateTimeFunc.StringToDateTime(RedisUtil.Instance.get(getRedisHeader() + "closedTime"));
            closingTime = DateTimeFunc.StringToDateTime(RedisUtil.Instance.get(getRedisHeader() + "closingTime"));

        }

    }
}
