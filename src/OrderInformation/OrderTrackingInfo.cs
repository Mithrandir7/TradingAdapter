using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderInformation
{
    public class OrderTrackingInfo
    {
        public long orderId { get; set; }
        public double entryPz { get; set; }
        public double closedPz { get; set; }
        public int entryOI { get; set; }
        public int closedOI { get; set; }
        public double currentProfit { get; set; }
        public double currentProfitPercent { get; set; }
        public double maxrunup { get; set; }
        public double maxdrawdown { get; set; }
        public DateTime filledTime { get; set; }
        public DateTime closedTime { get; set; }
        public DateTime closingTime { get; set; }

        public OrderTrackingInfo()
        {
            maxrunup = 0;
            maxdrawdown = 0;
        }

    }
}
