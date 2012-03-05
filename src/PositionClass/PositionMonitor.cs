using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeManager;
using AccountClass;

namespace PositionClass
{
    public class PositionMonitor
    {

        public static PositionMonitor Instance = new PositionMonitor();

        private PositionMonitor()
        {
            
        }

        public void init()
        {
            TradeCenter.Instance.addOnPositionAction(onPosition);
        }

        private void onPosition(Position aPosition)
        {
            AccountManager.addPosition(aPosition);
        }


    }
}
