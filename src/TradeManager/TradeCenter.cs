using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCEXTENSIONTRADEAPILib;
using AccountClass;
using SymbolClass;

namespace TradeManager
{
    public class TradeCenter
    {
        public delegate void OnAccountStatus(Account aAccount);
        public delegate void OnOrderStatus(OrderStatusReport aReport);
        public delegate void OnFilledStatus(FilledOrderReport aReport);
        public delegate void OnMarginStatus(Account aAccount);
        public delegate void OnPositionStatus(Position aPosition);

        public static TradeCenter Instance = new TradeCenter();

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private TradeClass tc = new TradeClass();

        private int submitCount = 0;

        private OnOrderStatus onOrder = null;
        private OnFilledStatus onFilled = null;
        private OnMarginStatus onMargin = null;
        private OnPositionStatus onPosition = null;
        private OnAccountStatus onAccount = null;

        const int INFO_TYPE_ACCOUNTS = 0;
        const int INFO_TYPE_STK_MARGINS = 1;
        const int INFO_TYPE_FUT_MARGINS = 2;
        const int INFO_TYPE_STK_POSITIONS = 3;
        const int INFO_TYPE_FUT_POSITIONS = 4;
        const int INFO_TYPE_OPT_POSITIONS = 5;
        const int INFO_TYPE_ORDER_STATUS = 7;
        const int INFO_TYPE_FILLED_ORDERS = 8;

        private TradeCenter()
        {
            
        }

        public void init()
        {
            tc.OnDisconnected += new _ITradeEvents_OnDisconnectedEventHandler(OnDisconnected);
            tc.OnInfo += new _ITradeEvents_OnInfoEventHandler(OnInfo);
            int lrtn = tc.DoConnect();
            if (lrtn == 1)
            {
                logger.Info("Trade connected.");
                onConnected();
            }
            else
            {
                logger.Info("Trade connection failed.");
            }
        }

        private void onConnected()
        {
            updateMargin();
        }

        public void addOnAccountAction(OnAccountStatus anAction)
        {
            onAccount += anAction;
        }

        public void removeOnAccountAction(OnAccountStatus anAction)
        {
            onAccount -= anAction;
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

            logger.Info("OnInfo:" + nInfoType + "..." + nInfo);

            switch (nInfoType)
            {
                case INFO_TYPE_ACCOUNTS:
                    // function seems not support
                    // use margin report, that contain account info
                    logger.Info("Function is not support, use margin report");
                    break;
                case INFO_TYPE_STK_MARGINS:
                    logger.Info("Function is not support");
                    break;
                case INFO_TYPE_FUT_MARGINS://FutMargins    
                    for (int i = 0; i < nInfo; i++)
                    {
                        Account account = new Account();

                        account.time = tc.GetInfoString(nInfoType, i, 0);
                        account.broker = tc.GetInfoString(nInfoType, i, 1);
                        account.account = tc.GetInfoString(nInfoType, i, 2);
                        account.preBalance = tc.GetInfoString(nInfoType, i, 4);
                        account.depositWithraw = tc.GetInfoString(nInfoType, i, 5);
                        account.commission = tc.GetInfoString(nInfoType, i, 6);
                        account.closeProfit = tc.GetInfoString(nInfoType, i, 10);
                        account.balance = tc.GetInfoString(nInfoType, i, 11);
                        account.positionProfit = tc.GetInfoString(nInfoType, i, 12);
                        account.currMargin = tc.GetInfoString(nInfoType, i, 14);
                        account.Available = tc.GetInfoString(nInfoType, i, 18);
                        account.FrozenMargin = tc.GetInfoString(nInfoType, i, 50);
                        logger.Info("On Margin Status: " + account.getInfo());
                        if (onMargin != null)
                        {
                            onMargin(account);
                        }
                    }

                    break;
                case INFO_TYPE_STK_POSITIONS://StkPositions
                    logger.Info("Function is not support");
                    break;
                case INFO_TYPE_FUT_POSITIONS://FutPositions

                    //logger.Info("On FutPositions " + nInfo);

                    for (int i = 0; i < nInfo; i++)
                    {
                        Position aPosition = new Position();
                        aPosition.broker = tc.GetInfoString(nInfoType, i, 0);
                        aPosition.account = tc.GetInfoString(nInfoType, i, 1);
                        aPosition.symbolid = tc.GetInfoString(nInfoType, i, 2);
                        aPosition.symbolname = tc.GetInfoString(nInfoType, i, 3);
                        aPosition.month = tc.GetInfoString(nInfoType, i, 4);
                        aPosition.buysell = tc.GetInfoString(nInfoType, i, 5);
                        aPosition.qty = tc.GetInfoString(nInfoType, i, 6);
                        aPosition.price = tc.GetInfoString(nInfoType, i, 7);
                        aPosition.exchange = tc.GetInfoString(nInfoType, i, 8);
                        aPosition.billtype = tc.GetInfoString(nInfoType, i, 9);
                        aPosition.time = tc.GetInfoString(nInfoType, i, 10);
                        aPosition.symbol = tc.GetInfoString(nInfoType, i, 11);
                        logger.Info("On Position Status: " + aPosition.info());
                        if (onPosition != null)
                        {
                            onPosition(aPosition);
                        }
                    }

                    break;
                case INFO_TYPE_OPT_POSITIONS://OptPositions
                    logger.Info("Function is not support");
                    break;
                case INFO_TYPE_ORDER_STATUS://OrderStatus

                    #region OrderStatus
                    OrderStatusReport aStatusReport = new OrderStatusReport();
                    aStatusReport.account = tc.GetInfoString(nInfoType, nInfo, 0);
                    aStatusReport.orderNumber = tc.GetInfoString(nInfoType, nInfo, 1);
                    aStatusReport.status = tc.GetInfoString(nInfoType, nInfo, 11);
                    aStatusReport.iceid = tc.GetInfoString(nInfoType, nInfo, 25);
                    logger.Info("OrderStatusReport account " + aStatusReport.account);
                    aStatusReport.abbrname = SymbolManager.Instance.getAbbrname(aStatusReport.iceid);

                    aStatusReport.buySellStr = tc.GetInfoString(nInfoType, nInfo, 6);
                    aStatusReport.price = tc.GetInfoValue(nInfoType, nInfo, 7);
                    aStatusReport.lots = tc.GetInfoValue(nInfoType, nInfo, 6);
                    aStatusReport.filledNumber = tc.GetInfoValue(nInfoType, nInfo, 9);
                    aStatusReport.memo = tc.GetInfoString(nInfoType, nInfo, 24);
                    aStatusReport.cancealable = tc.GetInfoString(nInfoType, nInfo, 23);
                    aStatusReport.time = tc.GetInfoString(nInfoType, nInfo, 12);
                    luserkey = tc.GetInfoString(nInfoType, nInfo, 14);

                    //logger.Info("userkey before replace..." + luserkey);

                    if (String.Compare(aStatusReport.account, "8070-880937") == 0)
                    {
                        int aIdx = luserkey.IndexOf("]");
                        int bIdx = luserkey.IndexOf("[!@6]");
                        luserkey = luserkey.Substring(aIdx + 1, bIdx - aIdx - 1);
                    }

                    //logger.Info("userkey after replace..." + luserkey);

                    aStatusReport.userkey = luserkey;


                    logger.Info("On Order Status: " + aStatusReport.info());
                    if (onOrder != null)
                    {
                        onOrder(aStatusReport);
                    }
                    #endregion
                    break;
                case INFO_TYPE_FILLED_ORDERS://FilledOrders

                    #region FilledOrderStatus
                    FilledOrderReport aFilledReport = new FilledOrderReport();

                    aFilledReport.account = tc.GetInfoString(nInfoType, nInfo, 0);
                    aFilledReport.orderNumber = tc.GetInfoString(nInfoType, nInfo, 1);
                    aFilledReport.iceid = tc.GetInfoString(nInfoType, nInfo, 25);
                    aFilledReport.abbrname = SymbolManager.Instance.getAbbrname(aFilledReport.iceid);
                    //if (AccountManager.isRealAccount(aFilledReport.account))
                    //{
                    //    aFilledReport.abbrname = utility.getAbbrnameFromCTPID(aFilledReport.iceid);
                    //}
                    //else
                    //{
                    //    aFilledReport.abbrname = utility.getAbbrnameFromGQuoteID(aFilledReport.iceid);
                    //}

                    aFilledReport.buySellStr = tc.GetInfoString(nInfoType, nInfo, 6); ;
                    aFilledReport.filledNumber = tc.GetInfoValue(nInfoType, nInfo, 9);
                    aFilledReport.avgPrice = tc.GetInfoValue(nInfoType, nInfo, 10);
                    aFilledReport.memo = tc.GetInfoString(nInfoType, nInfo, 24);
                    aFilledReport.status = tc.GetInfoString(nInfoType, nInfo, 11);
                    luserkey = tc.GetInfoString(nInfoType, nInfo, 14);

                    //logger.Info("userkey before replace..." + luserkey);

                    if (String.Compare(aFilledReport.account, "8070-880937") == 0)
                    {
                        int aIdx = luserkey.IndexOf("]");
                        int bIdx = luserkey.IndexOf("[!@6]");
                        luserkey = luserkey.Substring(aIdx + 1, bIdx - aIdx - 1);
                    }

                    //logger.Info("userkey after replace..." + luserkey);

                    aFilledReport.userkey = luserkey;

                    aFilledReport.time = tc.GetInfoString(nInfoType, nInfo, 12);
                    logger.Info("On Filled Order Status: " + aFilledReport.info());
                    if (onFilled != null)
                    {
                        onFilled(aFilledReport);
                    }
                    #endregion
                    break;
            }

        }

        public void updateMargin()
        {
            AccountManager.clear();
            RedisAccountHandler.Instance.clear();
            tc.QueryInfo(2, "");
        }

        private void updatePosition()
        {
            tc.QueryInfo(4, "");
        }

        private void OnDisconnected()
        {
            //throw new NotImplementedException();
            logger.Info("Disconnected");
        }

        public void cancealFutureOrder(string aOrderNo)
        {
            string orderInfo = "TYPE=R,QTY=0,ORDER_NO=" + aOrderNo;
            tc.PlaceOrder(orderInfo);
            submitCount = submitCount + 1;
        }

        public void openFutureOrderLimit(string account, string symbol,
                                string buySell, string price, string userKey)
        {
            int lot = 1;
            placeFutureOrder("O", account, symbol, buySell, price, lot, userKey);
        }

        public void closeFutureOrderLimit(string account, string symbol,
                                string buySell, string price, string userKey)
        {
            int lot = 1;
            placeFutureOrder("C", account, symbol, buySell, price, lot, userKey);
        }



        private void placeFutureOrder(string strOpenClose, string strAccount, string strSymbol,
                                string strBuySell, string strPrice, int lot, string userKey)
        {
            string orderInfo;
            string targetType;
            string orderType;
            string priceFlag;
            string qty;
            string dayTrade;

            if (String.Compare(strPrice, "") == 0)
            {
                return;
            }

            if (String.Compare(strAccount, "") == 0)
            {
                return;
            }

            targetType = "F";

            if (AccountManager.isRealAccount(strAccount))
            {
                priceFlag = "L";
                orderType = "R";
            }
            else
            {
                priceFlag = "M";
                orderType = "I";
            }
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
