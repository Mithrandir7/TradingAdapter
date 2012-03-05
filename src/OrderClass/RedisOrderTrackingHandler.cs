using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderInformation;
using UtilityClass;

namespace OrderClass
{
    public class RedisOrderTrackingHandler
    {
        public static RedisOrderTrackingHandler Instance = new RedisOrderTrackingHandler();

        private RedisUtil redisUtility;

        private RedisOrderTrackingHandler()
        {
          
        }

        public void init()
        {
            redisUtility = RedisUtil.getNewInstance();
        }

        public void push(Order aOrder)
        {                
            updateDB(aOrder);
        }

        public void removeOrderFromDatabase(Order order)
        {
            if (order.getState() == OrderState.GiveUp | order.getState() == OrderState.Canceled | order.getState() == OrderState.Closed)
            {
                delete(order);
            }            
        }

        private void delete(Order aOrder)
        {
            string header = "OrderID:" + aOrder.getOrderID() + ":*";
            List<string> lkeys = redisUtility.search(header);
            if (lkeys.Count > 0)
            {
                redisUtility.del(lkeys.ToArray<string>());
            }
        }

        private void updateDB(Order aOrder)
        {
            OrderInfo orderinfo = aOrder.getOrderInfo();
            OrderTrackingInfo tracking = aOrder.getOrderTracking();

            string header = "OrderID:" + aOrder.getOrderID() + ":";

            redisUtility.set(header + "state", aOrder.getState().ToString());
            redisUtility.set(header + "entryprice", tracking.entryPz.ToString());
            redisUtility.set(header + "entryoi", tracking.entryOI.ToString());            
            redisUtility.set(header + "runup", tracking.maxrunup.ToString());
            redisUtility.set(header + "maxdrawdown", tracking.maxdrawdown.ToString());
            redisUtility.set(header + "filledtime", tracking.filledTime.ToShortTimeString());
            redisUtility.set(header + "profit", tracking.currentProfit.ToString());
            redisUtility.set(header + "profitpercent", tracking.currentProfitPercent.ToString());

            redisUtility.set(header + "symbol", orderinfo.abbrName);
            redisUtility.set(header + "account",orderinfo.account);
            redisUtility.set(header + "position", orderinfo.position.ToString());

            redisUtility.set(header + "par:profittake", aOrder.getProfitTakeParStr());
            redisUtility.set(header + "par:hardstop", aOrder.getHardstopParStr());
            redisUtility.set(header + "par:protect", aOrder.getProtectorParStr());
            redisUtility.set(header + "par:daytrade", aOrder.getDayTradeParStr());

        }
      

    }
}
