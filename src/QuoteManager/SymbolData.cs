using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataManager
{
    public class SymbolData
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private String abbrname;
        private List<int> time = new List<int>();
        private List<double> trade = new List<double>();
        private List<double> high = new List<double>();
        private List<double> low = new List<double>();
        private List<int> volume = new List<int>();
        private List<int> oi = new List<int>();
        //private OIChart oichart = null;

        public SymbolData(string aName)
        {
            abbrname = aName;
        }

        public void initOiChart()
        {
            //List<string> observedList = SymbolManager.oiPlotSymbolList;

            //logger.Info(abbrname + " DataInfo created! " + String.Compare(this.abbrname, "if"));
            //if (observedList.Contains(abbrname))
            //{
                //logger.Info(abbrname + " ..............Chart created!");
                //oichart = new OIChart(abbrname);
            //}
        }

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

        public int getLastOI()
        {
            if (oi.Count > 0)
            {
                return (oi.Last());
            }
            else
            {
                return (0);
            }
        }

        public string getAbbrname()
        {
            return (abbrname);
        }

        public double getOIDelta(int nSecs)
        {
            if (trade.Count < (nSecs + 1))
            {
                return (0);
            }
            else
            {
                return (oi.Last() - getAvgOI(nSecs));
            }
        }

        public double getDelta(int nSecs)
        {
            if (trade.Count < (nSecs + 1))
            {
                return (0);
            }
            else
            {
                return (trade.Last() - getAvgTrade(nSecs));
            }
        }

        private double getAvgOI(int nSecs)
        {
            if (trade.Count < (nSecs + 1))
            {
                return (0);
            }
            else
            {
                return (oi.GetRange(oi.Count - nSecs, nSecs).Average());
            }
        }


        private double getAvgTrade(int nSecs)
        {
            if (trade.Count < (nSecs + 1))
            {
                return (0);
            }
            else
            {
                return (trade.GetRange(trade.Count - nSecs, nSecs).Average());
            }
        }

        public void push(TickQuoteCn aTick)
        {
            int intTime = Convert.ToInt32(aTick.time);

            if (time.Count == 0)
            {
                time.Add(intTime);
                trade.Add(aTick.trade);
                high.Add(aTick.trade);
                low.Add(aTick.trade);
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
                    high[lastIdx] = Math.Max(aTick.trade, high[lastIdx]);
                    low[lastIdx] = Math.Min(aTick.trade, low[lastIdx]);
                    oi[lastIdx] = Convert.ToInt32(aTick.oi);
                    volume[lastIdx] = volume[lastIdx] + Convert.ToInt32(aTick.volume);
                    RedisQuoteDataHandler.Instance.insertQuote(abbrname, ltime, high[lastIdx], low[lastIdx], volume[lastIdx], oi[lastIdx]);                    
                }
                else
                {
                    time.Add(intTime);
                    trade.Add(aTick.trade);
                    high.Add(aTick.trade);
                    low.Add(aTick.trade);
                    oi.Add(Convert.ToInt32(aTick.oi));
                    volume.Add(Convert.ToInt32(aTick.volume));

                    //if (oichart != null)
                    //{
                    //    oichart.addData(aTick);
                    //}
                }
            }
        }
    }
}
