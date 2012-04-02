using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SymbolSearch;
using Tickdata;
using log4net;

namespace HistoricalData
{
    class Program
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {            
            MongoTXO.Instance.init();
            MongoTXO.Instance.updateYestoday();            
        }        
    }
}
