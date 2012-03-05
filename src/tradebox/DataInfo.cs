using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradebox
{
    public class DataInfo
    {
        
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private string abbrname;
        private List<int> time = new List<int>();
        private List<double> trade = new List<double>();
        private List<int> volume = new List<int>();
        private List<int> oi = new List<int>();


        public double getLastTrade()
        {
            if (trade.Count > 0)
            {
                return (trade.Last());
            }
            else
            {
                return (0);
            }
        }


        public DataInfo(string aName)
        {
            abbrname = aName;
        }

        public string getAbbrname()
        {
            return (abbrname);
        }

        public void push(TickReport aTick)
        {
            int intTime = Convert.ToInt32(aTick.time);

            if (time.Count == 0)
            {
                time.Add(intTime);
                trade.Add(aTick.trade);
                oi.Add(Convert.ToInt32(aTick.oi));
                volume.Add(Convert.ToInt32(aTick.volume));
            }
            else
            {
                int ltime = time.Last();
                int lastIdx = time.Count - 1;             
                //logger.Info("data in " + ltime.ToString());

                if (intTime == ltime)
                {
                    trade[lastIdx] = aTick.trade;
                    oi[lastIdx] = Convert.ToInt32(aTick.oi);
                    volume[lastIdx] = volume[lastIdx] + Convert.ToInt32(aTick.volume);
                }
                else
                {
                    time.Add(intTime);
                    trade.Add(aTick.trade);
                    oi.Add(Convert.ToInt32(aTick.oi));
                    volume.Add(Convert.ToInt32(aTick.volume));
                }
            }

        }




    }
}
