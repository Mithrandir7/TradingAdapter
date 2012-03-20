using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataManager;

namespace OrderClass
{
    public class Protector
    {

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Order order;
        private bool isEnable = false;
        private double runupTriggerPercent = 1;  // base on entry
        private double drawdownExitPercent = 30; // base on runup

        public Protector(Order aOrder)
        {
            order = aOrder;
        }

        public string getParStr()
        {
            return Convert.ToString(runupTriggerPercent) + "/" + Convert.ToString(drawdownExitPercent);
        }

        public void enable(double aPercentRunupTrigger, double aPercentDrawdownExit)
        {
            runupTriggerPercent = aPercentRunupTrigger;
            drawdownExitPercent = aPercentDrawdownExit;
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
                logger.Info("OrderId : " + order.getOrderID() + " : protector check enabled");
                hasLogged = true;
            }

            double runupPercent = order.getOrderTracking().maxrunup / order.getOrderTracking().entryPz;
            double drawdownPercent = (order.getOrderTracking().maxrunup - order.getOrderTracking().currentProfit) / order.getOrderTracking().maxrunup;
            if (runupPercent > runupTriggerPercent)
            {
                if (drawdownPercent > drawdownExitPercent)
                {
                    order.closingOrder("ProfitProtect");
                }
            }                          
        }
    }
}
