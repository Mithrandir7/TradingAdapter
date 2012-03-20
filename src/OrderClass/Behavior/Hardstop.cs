using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderInformation;
using DataManager;

namespace OrderClass
{
    public class Hardstop
    {

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Order order;
        private bool isEnable = false;
        private double stoppercent=1;

        public Hardstop(Order aOrder)
        {
            order = aOrder;
        }


        public string getParStr()
        {
            return Convert.ToString(stoppercent);
        }

        public void enable(double aStopPercent)
        {
            stoppercent = aStopPercent;
            isEnable = true;
        }


        private Boolean hasLogged = false;

        public void check(TickQuote aTick)
        {
            if (!isEnable)
            {
                return;
            }

            if (!hasLogged)
            {
                logger.Info("OrderId : " + order.getOrderID() + " : hardstop check enabled");
                hasLogged = true;
            }

            if (order.getState() == OrderState.Filled)
            {
                double currentProfit = order.getOrderTracking().currentProfitPercent;
                if (currentProfit < (-1*stoppercent))
                {
                    order.closingOrder("HardStop");
                }
            }            
        }
    }
}
