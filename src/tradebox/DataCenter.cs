using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradebox
{
    public class DataCenter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, DataInfo> datum = new Dictionary<string, DataInfo>();
        private quoteCenter qc;

        public DataCenter(quoteCenter aQC)
        {
            qc = aQC;
            init();
            qc.addOnTickAction(OnTickAction);


        }

        private void init()
        {
            List<string> abbrList = utility.getAbbrnameList();
            foreach (string abbrname in abbrList)
            {
                //logger.Info(abbrname + " in dataCenter...");
                datum.Add(abbrname, new DataInfo(abbrname));
            }   
        }

        public void OnTickAction(TickReport atick)
        {
            DataInfo ldatainfo = getDataInfo(atick.abbrname);
            //logger.Info("push data into dataInfo..." + atick.abbrname + "..." + ldatainfo.getAbbrname());
            ldatainfo.push(atick);
        }

        public double getLastTrade(string abbrname)
        {
            DataInfo ldatainfo = getDataInfo(abbrname);
            return (ldatainfo.getLastTrade());
        }

        private DataInfo getDataInfo(string abbrname)
        {
            
            if (datum.ContainsKey(abbrname))
            {
                //logger.Info("DEBUG : " + abbrname + "existed");
                return (datum[abbrname]);
            }
            else
            {
                //logger.Info("DEBUG : " + abbrname + "not existed");
                datum.Add(abbrname, new DataInfo(abbrname));
                return (datum[abbrname]);
            }
        }





    }
}
