using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccountClass;
using TradeManager;

namespace MarginClass
{
    public class MarginMonitor
    {
        public static MarginMonitor Instance = new MarginMonitor();

        private MarginMonitor()
        {
            
        }

        public void init()
        {
            TradeCenter.Instance.addOnMarginAction(onMarginReport);
        }

        private void onMarginReport(Account aAccount)
        {
            AccountManager.Instance.add(aAccount);
            RedisAccountHandler.Instance.push(aAccount);
        }

        public void update()
        {
            TradeCenter.Instance.updateMargin();
        }
    }
}
