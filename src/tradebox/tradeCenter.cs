using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCEXTENSIONTRADEAPILib;

namespace tradebox
{
    

    public class tradeCenter
    {
        public delegate void OnOrderStatus(OrderStatusReport aReport);
        public delegate void OnFilledStatus(FilledOrderReport aReport);
        public delegate void OnMarginStatus(MarginReport aReport);
        public delegate void OnPositionStatus(PositionReport aReport);

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public TradeClass tc = new TradeClass();
       
        private int submitCount = 0;

        private OnOrderStatus onOrder = null;
        private OnFilledStatus onFilled = null;
        private OnMarginStatus onMargin = null;
        private OnPositionStatus onPosition = null;

        const int INFO_TYPE_ACCOUNTS = 0;
        const int INFO_TYPE_STK_MARGINS = 1;
        const int INFO_TYPE_FUT_MARGINS = 2;
        const int INFO_TYPE_STK_POSITIONS = 3;
        const int INFO_TYPE_FUT_POSITIONS = 4;
        const int INFO_TYPE_OPT_POSITIONS = 5;
        const int INFO_TYPE_ORDER_STATUS = 7;
        const int INFO_TYPE_FILLED_ORDERS = 8;

        public tradeCenter(){
            tc.OnDisconnected += new _ITradeEvents_OnDisconnectedEventHandler(OnDisconnected);
            tc.OnInfo += new _ITradeEvents_OnInfoEventHandler(OnInfo);
            int lrtn =  tc.DoConnect();
            if(lrtn==1){
                logger.Info("Trade connected.");
            }else{
                logger.Info("Trade connection failed.");
            }
        }

        public void addOnOrderAction(OnOrderStatus anAction)
        {
            onOrder += anAction;        
        }

        public void removeOnOrderAction(OnOrderStatus anAction)
        {
            onOrder -= anAction;
        }

        public void addOnFilledAction(OnFilledStatus anAction)
        {
            onFilled += anAction;
        }

        public void removeOnFilledAction(OnFilledStatus anAction)
        {
            onFilled -= anAction;
        }


        public void addOnMarginAction(OnMarginStatus anAction)
        {
            onMargin += anAction;
        }

        public void removeOnMarginAction(OnMarginStatus anAction)
        {
            onMargin -= anAction;
        }


        public void addOnPositionAction(OnPositionStatus anAction)
        {
            onPosition += anAction;
        }

        public void removeOnPositionAction(OnPositionStatus anAction)
        {
            onPosition -= anAction;
        }

        private void OnInfo(int nInfoType, int nInfo)
        {
            //throw new NotImplementedException();
            string luserkey;

            switch (nInfoType)
            {
                case  INFO_TYPE_ACCOUNTS:
                    String AccountInfo = "";
                    for (int i = 0; i < nInfo; i++)
                    {
                        AccountInfo = "Account:" + tc.GetInfoString(nInfoType, i, 0) + "\n";
                        AccountInfo = AccountInfo + "Name:" + tc.GetInfoString(nInfoType, i, 1) + "\n";
                        AccountInfo = AccountInfo + "Type:" + tc.GetInfoString(nInfoType, i, 2) + "\n";
                        AccountInfo = AccountInfo + "Broker:" + tc.GetInfoString(nInfoType, i, 3) + "\n";
                        AccountInfo = AccountInfo + "LoginID:" + tc.GetInfoString(nInfoType, i, 4) + "\n";
                        AccountInfo = AccountInfo + "Session:" + tc.GetInfoString(nInfoType, i, 5) + "\n";
                        logger.Info(AccountInfo);
                    }
                    break;
                case INFO_TYPE_STK_MARGINS:
                    logger.Info("Function is not support");                                        
                    break;
                case INFO_TYPE_FUT_MARGINS://FutMargins    
                    
                    for (int i = 0; i < nInfo; i++)
                    {
                        MarginReport aMargineReport = new MarginReport();    

                        aMargineReport.time =  tc.GetInfoString(nInfoType, i, 0);
                        aMargineReport.broker = tc.GetInfoString(nInfoType, i, 1);
                        aMargineReport.account = tc.GetInfoString(nInfoType, i, 2);
                        aMargineReport.preBalance = tc.GetInfoString(nInfoType, i, 4);
                        aMargineReport.depositWithraw = tc.GetInfoString(nInfoType, i, 5);
                        aMargineReport.commission = tc.GetInfoString(nInfoType, i, 6);
                        aMargineReport.closeProfit = tc.GetInfoString(nInfoType, i, 10);
                        aMargineReport.balance = tc.GetInfoString(nInfoType, i, 11);
                        aMargineReport.positionProfit = tc.GetInfoString(nInfoType, i, 12);
                        aMargineReport.currMargin = tc.GetInfoString(nInfoType, i, 14);
                        aMargineReport.Available = tc.GetInfoString(nInfoType, i, 18);
                        aMargineReport.FrozenMargin = tc.GetInfoString(nInfoType, i, 50);
                        logger.Info("On Margin Status: " + aMargineReport.info());
                        onMargin(aMargineReport);      
                    }
                    break;
                case INFO_TYPE_STK_POSITIONS://StkPositions
                    logger.Info("Function is not support");
                    break;
                case INFO_TYPE_FUT_POSITIONS://FutPositions
                    for (int i = 0; i < nInfo;i++ )
                    {
                        PositionReport aPosReport = new PositionReport();
                        aPosReport.broker = tc.GetInfoString(nInfoType, i, 0);
                        aPosReport.account = tc.GetInfoString(nInfoType, i, 1);
                        aPosReport.symbolid = tc.GetInfoString(nInfoType, i, 2);
                        aPosReport.symbolname = tc.GetInfoString(nInfoType, i, 3);
                        aPosReport.month = tc.GetInfoString(nInfoType ,i, 4);
                        aPosReport.buysell = tc.GetInfoString(nInfoType, i, 5);
                        aPosReport.qty =  tc.GetInfoString(nInfoType, i, 6);
                        aPosReport.price = tc.GetInfoString(nInfoType, i, 7);
                        aPosReport.exchange = tc.GetInfoString(nInfoType, i, 8);
                        aPosReport.billtype = tc.GetInfoString(nInfoType, i, 9);
                        aPosReport.time = tc.GetInfoString(nInfoType, i, 10);
                        aPosReport.symbol = tc.GetInfoString(nInfoType, i, 11);
                        logger.Info("On Position Status: " + aPosReport.info());
                        onPosition(aPosReport);   
                    }
                    break;
                case INFO_TYPE_OPT_POSITIONS://OptPositions
                    logger.Info("Function is not support");
                    break;
                case INFO_TYPE_ORDER_STATUS://OrderStatus

                    OrderStatusReport aStatusReport = new OrderStatusReport();                 
                    aStatusReport.account = tc.GetInfoString(nInfoType, nInfo, 0);
                    aStatusReport.orderNumber = tc.GetInfoString(nInfoType, nInfo, 1);
                    aStatusReport.status = tc.GetInfoString(nInfoType, nInfo, 11);
                    aStatusReport.iceid = tc.GetInfoString(nInfoType, nInfo, 25);
                    aStatusReport.abbrname = utility.getAbbrnameFromICEID(aStatusReport.iceid);
                    aStatusReport.buySellStr = tc.GetInfoString(nInfoType, nInfo, 6);
                    aStatusReport.price = tc.GetInfoValue(nInfoType, nInfo, 7);
                    aStatusReport.lots = tc.GetInfoValue(nInfoType, nInfo, 6);
                    aStatusReport.filledNumber = tc.GetInfoValue(nInfoType, nInfo, 9);
                    aStatusReport.memo = tc.GetInfoString(nInfoType, nInfo, 24);
                    aStatusReport.cancealable = tc.GetInfoString(nInfoType, nInfo, 23);
                    aStatusReport.time = tc.GetInfoString(nInfoType, nInfo, 12);
                    luserkey = tc.GetInfoString(nInfoType, nInfo, 14);

                    logger.Info("userkey before replace..." + luserkey);

                    if (String.Compare(aStatusReport.account, "8070-880937") == 0)
                    {
                        int aIdx = luserkey.IndexOf("]");
                        int bIdx = luserkey.IndexOf("[!@6]");
                        luserkey = luserkey.Substring(aIdx + 1, bIdx - aIdx - 1);
                    }

                    logger.Info("userkey after replace..." + luserkey);
      
                    aStatusReport.userkey = luserkey;


                    logger.Info("On Order Status: "+aStatusReport.info());
                    onOrder(aStatusReport);                    
                    break;
                case INFO_TYPE_FILLED_ORDERS://FilledOrders
                    FilledOrderReport aFilledReport = new FilledOrderReport();

                    aFilledReport.account = tc.GetInfoString(nInfoType, nInfo, 0);
                    aFilledReport.orderNumber = tc.GetInfoString(nInfoType, nInfo, 1);
                    aFilledReport.iceid = tc.GetInfoString(nInfoType, nInfo, 25);
                    aFilledReport.abbrname = utility.getAbbrnameFromICEID(aFilledReport.iceid);
                    aFilledReport.buySellStr = tc.GetInfoString(nInfoType, nInfo, 6); ;
                    aFilledReport.filledNumber = tc.GetInfoValue(nInfoType, nInfo, 9);
                    aFilledReport.avgPrice = tc.GetInfoValue(nInfoType, nInfo, 10);
                    aFilledReport.memo = tc.GetInfoString(nInfoType, nInfo, 24);
                    aFilledReport.status = tc.GetInfoString(nInfoType, nInfo, 11);
                    luserkey = tc.GetInfoString(nInfoType, nInfo, 14);

                    logger.Info("userkey before replace..." + luserkey);

                    if (String.Compare(aFilledReport.account, "8070-880937") == 0)
                   {
                       int aIdx = luserkey.IndexOf("]");
                       int bIdx = luserkey.IndexOf("[!@6]");
                       luserkey = luserkey.Substring(aIdx + 1, bIdx - aIdx - 1);                                          
                    }

                    logger.Info("userkey after replace..." + luserkey);

                    aFilledReport.userkey = luserkey;

                    aFilledReport.time = tc.GetInfoString(nInfoType, nInfo, 12);
                    logger.Info("On Filled Order Status: "+aFilledReport.info());
                    onFilled(aFilledReport);       
                    break;
            }

        }

        public void updateMargin()
        {
            tc.QueryInfo(2, "");
        }

        public void updatePosition()
        {
            tc.QueryInfo(4, "");
        }

        private void OnDisconnected()
        {
            //throw new NotImplementedException();
            logger.Info("Disconnected");
        }

        public void cancealFutureOrder(string aOrderNo){
            string orderInfo  = "TYPE=R,QTY=0,ORDER_NO=" + aOrderNo;
            tc.PlaceOrder(orderInfo);
            submitCount = submitCount + 1;
        }
        
        public void openFutureOrderLimit(string account, string symbol, 
                                string buySell, string price ,int lot, string userKey){
            placeFutureOrderLimit("O", account, symbol, buySell, price, lot, userKey);
        }
           
        public void closeFutureOrderLimit(string account, string symbol, 
                                string buySell, string price ,int lot, string userKey){
            placeFutureOrderLimit("C", account, symbol, buySell, price, lot, userKey);
        }

        private void placeFutureOrderLimit(string strOpenClose, string strAccount, string strSymbol, 
                                string strBuySell, string strPrice, int lot, string userKey){
            string orderInfo;
            string targetType;
            string orderType;
            string priceFlag;
            string qty;
            string dayTrade;

            if(String.Compare(strPrice,"")==0){
                return;
            }
              
            if(String.Compare(strAccount,"")==0){
                return;
            }

            targetType = "F";
            orderType = "R";
            priceFlag = "L";
            qty = Convert.ToString(lot);
            dayTrade = "N";

            orderInfo = "Type=" + targetType +
                    ",Account=" + strAccount +
                    ",Symbol=" + strSymbol +
                    ",order_type=" + orderType +
                    ",buy_sell=" + strBuySell +
                    ",price=" + strPrice +
                    ",price_flag=" + priceFlag +
                    ",qty=" + qty +
                    ",open_close=" + strOpenClose.Trim() +
                    ",USER_KEY=" + userKey +
                    ",day_Trade=" + dayTrade;

            logger.Info("PlaceOrder : orderInfo" + orderInfo);
            tc.PlaceOrder(orderInfo);
            submitCount = submitCount + 1;

        }
        

    }
}
