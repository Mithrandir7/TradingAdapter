using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymbolClass;

namespace DataManager
{
    public class DataCenter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, SymbolData> datum = new Dictionary<string, SymbolData>();

        public static DataCenter Instance = new DataCenter();

        private DataCenter()
        {            
                
        }

        public void init()
        {
            List<string> abbrList = SymbolManager.Instance.getAbbrnameList();
            foreach (string abbrname in abbrList)
            {
                //logger.Info(abbrname + " in dataCenter...");
                datum.Add(abbrname, new SymbolData(abbrname));
            }
            QuoteAdapter.Instant.addOnTickAction(OnTickAction);        
            initOIChart();
        }

        public void initOIChart()
        {
            foreach (SymbolData adf in datum.Values)
            {
                adf.initOiChart();
            }
        }

        public void OnTickAction(TickQuote atick)
        {
            SymbolData ldatainfo = getDataInfo(atick.abbrname);
            //logger.Info("push data into dataInfo..." + atick.abbrname + "..." + ldatainfo.getAbbrname());
            ldatainfo.push(atick);
        }

        public double getLastTrade(string abbrname)
        {
            SymbolData ldatainfo = getDataInfo(abbrname);
            return (ldatainfo.getLastTrade());
        }

        public int getLastOI(string abbrname)
        {
            SymbolData ldatainfo = getDataInfo(abbrname);
            return (ldatainfo.getLastOI());
        }

        public double getDelta(string abbrname, int nSecs)
        {
            SymbolData ldatainfo = getDataInfo(abbrname);
            return (ldatainfo.getDelta(nSecs));
        }

        public double getOIDelta(string abbrname, int nSecs)
        {
            SymbolData ldatainfo = getDataInfo(abbrname);
            return (ldatainfo.getOIDelta(nSecs));
        }

        private SymbolData getDataInfo(string abbrname)
        {
            if (datum.ContainsKey(abbrname))
            {
                //logger.Info("DEBUG : " + abbrname + "existed");
                return (datum[abbrname]);
            }
            else
            {
                //logger.Info("DEBUG : " + abbrname + "not existed");
                datum.Add(abbrname, new SymbolData(abbrname));
                return (datum[abbrname]);
            }
        }
    }
}
