using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderInformation;
using DataManager;

namespace OrderClass
{
    public class ProfitTake
    {
        private Order order;
        private bool isEnable = false;
        private double percentTake = 1;

        public ProfitTake(Order aOrder)
        {
            order = aOrder;
        }

        public string getParStr()
        {
            return Convert.ToString(percentTake);
        }        

        public void enable(double aPercentTake)
        {
            percentTake = aPercentTake;
            isEnable = true;
        }

        public void check(TickQuote aTick)
        {
            if (!isEnable)
            {
                return;
            }

            if (order.getState() == OrderState.Filled)
            {
                if (order.getOrderTracking().currentProfitPercent > 0.01*percentTake)
                {
                    order.closingOrder("ProfitTake");
                }
            }
        }
    }
}
