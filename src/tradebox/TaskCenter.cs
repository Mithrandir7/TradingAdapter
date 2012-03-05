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
    public class TaskCenter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TradeCenter tc;
        private QuoteCenter qc;
        private DataCenter dc;

        private Dictionary<int, TaskInfo> tasks = new Dictionary<int, TaskInfo>();
        private Dictionary<int, Order> orders = new Dictionary<int, Order>();

        public void activation()
        {
            foreach (Order x in orders.Values)
            {
                x.active();
            }
            
        }

        public void inActivation()
        {
            foreach (Order x in orders.Values)
            {
                x.inActive();
            }
        }


        public Order getOrderByOrderId(int aOrderId)
        {
            if(orders.ContainsKey(aOrderId))
            {
                return(orders[aOrderId]);
            }else
            {
                return null;
            }
        }

        public TaskCenter(QuoteCenter aQc, TradeCenter aTc, DataCenter aDc)
        {
            tc = aTc;
            qc = aQc;
            dc = aDc;

            //demoTask(); // remove in production
            initOrder();            

        }

        public void readTaskFromFile()
        {
            tasks.Clear();
            orders.Clear();


            if (!par.isFileExist())
            {
                logger.Info("no task exist...");
                return;
            }

            string lstr = par.readFile();
            string[] lines = lstr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string datstring = lines[0];
            
            int dayint = Convert.ToInt32(datstring);
            if (dayint != utility.getYYYYMMDD())
            {
                logger.Info("time stamp error...no task load...");
                return;
            }

            logger.Info("today day :" + datstring);

            for (int i = 1; i < lines.Length; i++)
            {
                if (!lines[i].Contains('#') & lines[i].Trim().Length>1)
                {
                    string taskString = lines[i].Trim();
                    string[] parArr = taskString.Split(',');
                    if (parArr.Length == 12)
                    {
                        logger.Info("readTask :" + taskString);
                        int accountId = Convert.ToInt32(parArr[0]);
                        string abbrname = parArr[1].Trim().ToLower();
                        int pzTor = Convert.ToInt32(parArr[2]);
                        int orderId = Convert.ToInt32(parArr[3]);
                        int typeId = Convert.ToInt32(parArr[4]);
                        double del = Convert.ToDouble(parArr[5]);
                        int delOI = Convert.ToInt32(parArr[6]);
                        int parentId = Convert.ToInt32(parArr[7]);
                        int after = Convert.ToInt32(parArr[8]);
                        int before = Convert.ToInt32(parArr[9]);
                        int nSecs = Convert.ToInt32(parArr[11]);
                        bool isDt;
                        if (String.Compare(parArr[10].ToLower(), "true") == 0)
                        {
                            isDt = true;
                        }
                        else
                        {
                            isDt = false;
                        }
                        TaskInfo ti = new TaskInfo(accountId, abbrname, pzTor, orderId, typeId, del, delOI, parentId, after, before, isDt, nSecs);
                        logger.Info("task added :" + ti.info());
                        tasks.Add(ti.orderid, ti);
                    }
                    
                }
            }
            initOrder();

        }

        ~TaskCenter()
        {
            
        }

     
        private void initOrder()
        {
            if (tasks.Keys.Count <= 0)
            {
                return;
            }

            foreach (TaskInfo x in tasks.Values)
            {
                Order lod = new Order(this, tc, qc, dc, x);
                orders.Add(x.orderid, lod);
            }
        }

        

        private void demoTask()
        {
            TaskInfo ti = new TaskInfo(0, "rb", 3, 1, 1, 10.0, 100, -1,93000,140000,true,300);
            
            logger.Info("Debug : add demo task.");
            tasks.Add(ti.orderid, ti);
        }
    }
}
