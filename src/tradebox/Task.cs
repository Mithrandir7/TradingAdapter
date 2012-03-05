using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradebox
{
    [Serializable]
    public enum TaskType
    {
        BreakUpLong, // 1
        BreakDownLong, // 2
        BreakUpShort, //3
        BreakDownShort, //4
        LongAdd, //5
        ShortAdd, //6
        Unknown
    }

    [Serializable]
    public class Task
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private tradeCenter tc;
        private quoteCenter qc;
        private DataCenter dc;

        private Dictionary<int, TaskInfo> tasks = new Dictionary<int, TaskInfo>();
        private Dictionary<int, Order> orders = new Dictionary<int, Order>();
        private bool activate = false;

        public void activation()
        {
            activate = true;
        }

        public void inActivation()
        {
            activate = false;
        }

        public Task(quoteCenter aQc, tradeCenter aTc, DataCenter aDc)
        {
            tc = aTc;
            qc = aQc;
            dc = aDc;

            demoTask(); // remove in production
            initOrder();
            qc.addOnTickAction(OnTick);

        }

        ~Task()
        {
            qc.removeOnTickAction(OnTick);
        }

        private void OnTick(TickReport aTick)
        {
            if (!activate)
            {
                return;
            }
            // order entry condition monitor
        }

        private void initOrder()
        {
            if (tasks.Keys.Count <= 0)
            {
                return;
            }

            foreach (TaskInfo x in tasks.Values)
            {
                OrderInfo oinfo = new OrderInfo(x.accountID, x.abbrname, x.position, x.priceTor);
                Order lod = new Order(x.orderid, tc, qc, dc, oinfo);
                orders.Add(x.orderid, lod);
            }
        }

        

        private void demoTask()
        {
            TaskInfo ti = new TaskInfo(0, "rb", 3, 1, 1, 10, 100, -1); ;
            tasks.Add(ti.orderid, ti);
        }
    }
}
