using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderInformation;
namespace OrderClass
{
    public class OrderCmd
    {
        //basic information
        public OrderInfo orderInfo;

        //behavior parameters
        public OrderBehaviorParameters orderBehaviorParameters;

        //closed command
        public bool closed = false;

        public OrderCmd()
        {
            orderInfo = new OrderInfo();
            orderBehaviorParameters = new OrderBehaviorParameters();
        }

        public string getInfo()
        {
            return "OrderID:" + orderInfo.orderId + " / " +
                "account:" + orderInfo.account + " / " +
                "symbol:" + orderInfo.abbrName + " / " +
                "position:" + orderInfo.position + " / " +
                "daytrade:" + orderBehaviorParameters.daytrade + " / " +
                "profittakepercent:" + orderBehaviorParameters.profittakepercent + " / " +
                "protectiontrigger:" + orderBehaviorParameters.protectiontrigger + " / " +
                "protection:" + orderBehaviorParameters.protection + " / " +
                "hardstop:" + orderBehaviorParameters.hardstop;
        }
    }

 
}
