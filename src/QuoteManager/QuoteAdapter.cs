using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCEXTERNSIONQUOTEAPILib;
using System.IO;
using SymbolClass;


namespace DataManager
{
    public class QuoteAdapter
    {
        public delegate void OnTickData(TickQuote aReport);

        public static QuoteAdapter Instant = new QuoteAdapter();

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);     
   
        private QuoteClass qc;        
        private OnTickData onTick = null;

        private QuoteAdapter()
        {                        
        }


        public void init()
        {
            logger.Info("start QuoteAdapter...");
            qc = new QuoteClass();
            logger.Info("quoteClass Initialized...");
            qc.OnData += new _IQuoteEvents_OnDataEventHandler(OnData);
            qc.OnDisconnected += new _IQuoteEvents_OnDisconnectedEventHandler(OnDisconnected);
            connect();

            List<string> subList = SymbolManager.Instance.getSubSymbolList();

            if (subList != null)
            {
                foreach (string id in subList)
                {
                    qc.Subscribe(id);
                    logger.Info("QuoteCenter subscribe symbol " + id);
                }
            }
            else
            {
                logger.Info("subList is null");
            }
        }


        public string symbolLookup(string strPattern)
        {            
            return qc.SymbolLookup(strPattern);            
        }

        public void addOnTickAction(OnTickData anAction)
        {
            onTick += anAction;
        }

        public void removeOnTickAction(OnTickData anAction)
        {
            onTick -= anAction;
        }

        private void OnData(string strSymbol, int nStatus, int nCount)
        {
            string lsymbol = qc.SymbolGetStringData(strSymbol, -1, 0);
            string labbrname = SymbolManager.Instance.getAbbrname(strSymbol);
            double ltime = qc.SymbolGetValueData(strSymbol, -1, 2);
            double ltrade = qc.SymbolGetValueData(strSymbol, -1, 3) / 1000000;
            double lvolume = qc.SymbolGetValueData(strSymbol, -1, 4);
            double lbid = qc.SymbolGetValueData(strSymbol, -1, 30) / 1000000;
            double lask = qc.SymbolGetValueData(strSymbol, -1, 50) / 1000000;
            double loi = qc.SymbolGetValueData(strSymbol, -1, 91);

            if (lvolume < 0)
            {
                return;
            }

            if (ltrade < 0)
            {
                return;
            }

            TickQuote atick = new TickQuote(labbrname, ltime, ltrade, lvolume, lbid, lask, loi);
            //logger.Info(atick.info());
            if (onTick == null)
            {
                return;
            }
            onTick(atick);
        }

        private void connect()
        {
            int rtn = qc.DoConnect();
            if (rtn == 1)
            {
                logger.Info("Connect Succeed");
            }
            else
            {
                logger.Info("Connect Failed");
            }
        }

        private void OnDisconnected()
        {
            logger.Info("TC Disconnected");
        }

        private int GetType(String strSymbol)
        {
            //分析strsymbol來判斷是何種資料
            string[] SubstrSymbol = strSymbol.Split('.');
            string strTail = SubstrSymbol[SubstrSymbol.Length - 1];
            string strPrefix = SubstrSymbol[0];
            switch (strTail)
            {
                case "1KS":
                    return 1;
                case "5KS":
                    return 2;
                case "30KS":
                    return 3;
                case "DKS":
                    return 4;
                case "TICKERS":
                    return 5;
                case "PRICES":
                    return 6;
            }

            switch (strPrefix)
            {
                case "1KS":
                    return 7;
                case "5KS":
                    return 8;
                case "30KS":
                    return 9;
                case "DKS":
                    return 10;
                default:
                    return 0;
            }

            //return 1;

        }

    }
}
