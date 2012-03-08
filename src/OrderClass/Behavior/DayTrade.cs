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
        private int daytradeHHMMSS;

        public DayTrade(Order aOrder)
        {
            order = aOrder;
        }

        public void enable()
        {
            initPar();
            isEnable = true;
        }

        private void initPar()
        {
            daytradeHHMMSS = SymbolManager.Instance.getDayTradeExitHHMMSS(order.getAbbrName());

            //overwrite by account.xml
            String account = order.getOrderInfo().account;

            String daytradeHHMMSSStr =
                AccountClass.AccountXmlReader.Instance.getAccountAttribute(account, "daytradeExitTime");

            if (String.Compare(daytradeHHMMSSStr, "") != 0)
            {
                try
                {
                    daytradeHHMMSS = int.Parse(daytradeHHMMSSStr);

                }
                catch (FormatException e)
                {
                    daytradeHHMMSS = SymbolManager.Instance.getDayTradeExitHHMMSS(order.getAbbrName());
                }
            }
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
                

                if (Convert.ToInt32(aTick.time) > daytradeHHMMSS)
                {
                    order.closingOrder("Daytrade");
                }
            }            
        }

    }
}
