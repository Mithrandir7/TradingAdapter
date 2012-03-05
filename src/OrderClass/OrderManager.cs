using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderClass
{
    public class OrderManager
    {
        public static OrderManager Instance = new OrderManager();

        private Dictionary<int, Order> orders = new Dictionary<int, Order>();
        
        private OrderManager()
        {

        }

        public void init()
        {
            RedisOrderCmdHandler.Instance.init();
            RedisOrderTrackingHandler.Instance.init();
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
            if (orders.ContainsKey(aCmd.orderid))
            {
                order = orders[aCmd.orderid];
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
                order = new Order(aCmd.orderid,aCmd.account,aCmd.symbol,aCmd.position);
                orders.Add(aCmd.orderid,order);
                updateOrder(order, aCmd);
                order.active();
                order.fireOrder("OrderCmdRecieved");
                // new order can not closed
            }                        
        }

        private void updateOrder(Order order, OrderCmd aCmd)
        {
            // set hardstop
            if (aCmd.hardstop > 0)
            {
                order.invokeHardstop(aCmd.hardstop);
            }
            // set daytrade
            if (aCmd.daytrade)
            {
                order.invokeDayTrade();
            }
            // set profit take
            if (aCmd.profittakepercent > 0)
            {
                order.invokeProfitTake(aCmd.profittakepercent);
            }
            // set profit protect
            if (aCmd.protectiontrigger > 0 & aCmd.protection > 0)
            {
                order.invokeProtector(aCmd.protectiontrigger, aCmd.protection);
            }
        }
    }
}
