using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymbolClass;
using OrderInformation;
using DataManager;

namespace OrderClass
{

    public class DayTrade
    {
        private Order order;
        private bool isEnable = false;

        public DayTrade(Order aOrder)
        {
            order = aOrder;
        }

        public void enable()
        {
            isEnable = true;
        }

        public string getParStr()
        {
            return Convert.ToString(isEnable);
        }

        public void check(TickQuote aTick)
        {
            if (!isEnable)
            {
                return;
            }

            if (order.getState() == OrderState.Filled)
            {
                int daytradeHHMMSS = SymbolManager.Instance.getDayTradeExitHHMMSS(order.getAbbrName());
                if (Convert.ToInt32(aTick.time) > daytradeHHMMSS)
                {
                    order.closingOrder("Daytrade");
                }
            }            
        }

    }
}
