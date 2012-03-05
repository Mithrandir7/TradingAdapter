using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCEXTERNSIONQUOTEAPILib;

namespace tradebox
{
    public class quoteCenter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private QuoteClass qc = new QuoteClass();
        public delegate void OnTickData(TickReport aReport);

        private OnTickData onTick = null;

        public quoteCenter()
        {
            qc.OnData += new _IQuoteEvents_OnDataEventHandler(OnData);
            qc.OnDisconnected += new _IQuoteEvents_OnDisconnectedEventHandler(OnDisconnected);

            connect();

            qc.Subscribe("CTP.CFFEX.IF.HOT");
            qc.Subscribe("CTP.SHFE.rb.HOT");
            qc.Subscribe("CTP.SHFE.cu.HOT");
            qc.Subscribe("CTP.SHFE.au.HOT");
            qc.Subscribe("CTP.SHFE.ru.HOT");
            qc.Subscribe("CTP.SHFE.zn.HOT");
            qc.Subscribe("CTP.SHFE.al.HOT");

            qc.Subscribe("CTP.CZCE.CF.HOT");
            qc.Subscribe("CTP.CZCE.ER.HOT");
            qc.Subscribe("CTP.CZCE.WS.HOT");
            qc.Subscribe("CTP.CZCE.RO.HOT");
            qc.Subscribe("CTP.CZCE.TA.HOT");
            qc.Subscribe("CTP.CZCE.SR.HOT");

            qc.Subscribe("CTP.DCE.y.HOT");
            qc.Subscribe("CTP.DCE.j.HOT");
            qc.Subscribe("CTP.DCE.l.HOT");
            qc.Subscribe("CTP.DCE.m.HOT");
            qc.Subscribe("CTP.DCE.p.HOT");
            qc.Subscribe("CTP.DCE.a.HOT");
            qc.Subscribe("CTP.DCE.c.HOT");
            qc.Subscribe("CTP.DCE.v.HOT");
            
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
            string labbrname = utility.getAbbrnameFromICEID(lsymbol);
            double ltime = qc.SymbolGetValueData(strSymbol, -1, 2);
            double ltrade = qc.SymbolGetValueData(strSymbol, -1, 3) / 1000000;
            double lvolume = qc.SymbolGetValueData(strSymbol, -1, 6);
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

            TickReport atick = new TickReport(labbrname,ltime,ltrade,lvolume,lbid,lask,loi);
            logger.Info(atick.info());
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
