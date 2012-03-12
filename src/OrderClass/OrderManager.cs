using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderInformation;

namespace OrderClass
{
    public class OrderManager
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        public static OrderManager Instance = new OrderManager();

        private Dictionary<int, Order> orders = new Dictionary<int, Order>();
        
        private OrderManager()
        {

        }

        public void init()
        {
            RedisOrderCmdHandler.Instance.init();
            loadOrderFromRedis();
        }

        public bool isOrderIDExist(int aOrderID)
        {
            return orders.ContainsKey(aOrderID);
        }

        public Dictionary<int, Order> getOrders()
        {
            return orders;
        }

        public void push(OrderCmd aCmd)
        {
            Order order;
            if (orders.ContainsKey(aCmd.orderInfo.orderId))
            {
                // order alreday exist
                logger.Info("push:orderIdExist:"+aCmd.orderInfo.orderId);
                order = orders[aCmd.orderInfo.orderId];
                //orderid, account, symbol, and position can not change   
                if (aCmd.closed)
                {
                    order.closingOrder("OrderCmd");
                }
                else
                {
                    updateOrder(order, aCmd);
                }
            }
            else
            {
                logger.Info("push:nreOrderId:" + aCmd.orderInfo.orderId);
                // create new order
                OrderTrackingInfo orderTrackingInfo = new OrderTrackingInfo(OrderState.WaitingSubmit);
                orderTrackingInfo.orderId = aCmd.orderInfo.orderId;

                order = new Order(aCmd.orderInfo, orderTrackingInfo, aCmd.orderBehaviorParameters);
                addOrder(order);
                updateOrder(order, aCmd);
                order.active();
                order.fireOrder("NewOrderCmdRecieved");    
            }                        
        }

        public void loadOrderFromRedis()
        {
            List<int> orderIdList = getOrderIdListFromRedisDB();

            foreach (int aId in orderIdList)
            {
                OrderInfo orderInfo = new OrderInfo();
                orderInfo.loadFromRedis(aId);

                OrderTrackingInfo orderTrackingInfo = new OrderTrackingInfo();
                orderTrackingInfo.loadFromRedis(aId);

                OrderBehaviorParameters orderBehaviorParameters = new OrderBehaviorParameters();
                orderBehaviorParameters.loadFromRedis(aId);

                Order order = new Order(orderInfo, orderTrackingInfo, orderBehaviorParameters);
                if (orderTrackingInfo.orderState == OrderState.Filled)
                {
                    order.active();
                }
                addOrder(order);
            }
        }

        private List<int> getOrderIdListFromRedisDB()
        {
            List<String> ol = UtilityClass.RedisUtil.Instance.search("OrderId:*");
            List<int> lrtn = new List<int>();
            foreach (String str in ol)
            {
                lrtn.Add(int.Parse(str.Split(':')[1]));
            }
            return lrtn;
        }


        private void addOrder(Order order)
        {
            orders.Add(order.getOrderID(), order);
            UtilityClass.RedisUtil.Instance.set("OrderId:" + order.getOrderID().ToString().Trim(), "");
        }

        private void updateOrder(Order order, OrderCmd aCmd)
        {
            // set hardstop
            if (aCmd.orderBehaviorParameters.hardstop > 0)
            {
                order.invokeHardstop(aCmd.orderBehaviorParameters.hardstop);
            }
            // set daytrade
            if (aCmd.orderBehaviorParameters.daytrade)
            {
                order.invokeDayTrade();
            }
            // set profit take
            if (aCmd.orderBehaviorParameters.profittakepercent > 0)
            {
                order.invokeProfitTake(aCmd.orderBehaviorParameters.profittakepercent);
            }
            // set profit protect
            if (aCmd.orderBehaviorParameters.protectiontrigger > 0 & aCmd.orderBehaviorParameters.protection > 0)
            {
                order.invokeProtector(aCmd.orderBehaviorParameters.protectiontrigger, aCmd.orderBehaviorParameters.protection);
            }
        }

    }
}
