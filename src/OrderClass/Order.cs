using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderInformation;
using TradeManager;
using SymbolClass;
using UserkeyManager;
using DataManager;

namespace OrderClass
{
    #region
    // publish placeorder orderid=1;account=1999_2-0000049;symbol=if;position=1;daytrade=true;profittakepercent=0.5;protectiontrigger=0;protection=30;hardstop=0.8;closed=false
    // create order instance by using Order(aOrderID,account,abbrname,position){
    // Interface for order
    //   activate
    //   inActivate
    //   fireOrder
    //   closingOrder
    //   invokeDaytrade
    //   invokeHardstop(stopPercent)
    //   invokeProfitTake(takePercent)
    //   invokeProtector(aPercentRunupTrigger,aPercentDrawdown)
    #endregion

    public class Order
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private OrderInfo orderinfo;
        private OrderTrackingInfo tracking;
        private OrderBehaviorParameters behaviorPars;

        private OrderStatusReport orderStatusReport;
        private FilledOrderReport filledReport, closedFilledReport;

        private bool activation = false;

        private int secondsForFilled = 5;

        private DayTrade daytrade;
        private ProfitTake profitTake;
        private Protector protector;
        private Hardstop hardstop;

        public Order(OrderInfo aOrderInfo, 
            OrderTrackingInfo aTracking, OrderBehaviorParameters aBehaviorPars)
        {
            orderinfo = aOrderInfo;
            behaviorPars = aBehaviorPars;
            tracking = aTracking;
            changeState(aTracking.orderState);

            orderinfo.saveOnRedis();
            behaviorPars.saveOnRedis();
            tracking.saveOnRedis();

            daytrade = new DayTrade(this);
            profitTake = new ProfitTake(this);
            protector = new Protector(this);
            hardstop = new Hardstop(this);
        }

        ~Order() //destructor
        {
            removeOnAction();
        }

        private string getIdentifiString()
        {
            return ("OrderID..." +  orderinfo.orderId + "...");
        }

        public void active()
        {
            if (!activation)
            {
                logger.Info(getIdentifiString() + "userkey..." + orderinfo.getUserkey() + "..activated");
                activation = true;
                addOnAction();
            }
        }

        private void removeOnAction()
        {
            TradeCenter.Instance.removeOnOrderAction(OnOrder);
            TradeCenter.Instance.removeOnFilledAction(OnFilled);
            QuoteAdapter.Instant.removeOnTickAction(OnTick);
        }

        private void addOnAction()
        {
            TradeCenter.Instance.addOnOrderAction(OnOrder);
            TradeCenter.Instance.addOnFilledAction(OnFilled);
            QuoteAdapter.Instant.addOnTickAction(OnTick);
        }

        public void inActive()
        {
            if (activation)
            {
                logger.Info(getIdentifiString() + "userkey..." + orderinfo.getUserkey() + "..inActivated");
                activation = false;
                removeOnAction();
            }
        }

        private void OnTick(TickQuote aReport)
        {
            if (!activation)
            {
                return;
            }

            //logger.Info(orderinfo.userkey + " OnTicks..." + orderinfo.abbrName + "..." + aReport.abbrname);

            if (String.Compare(orderinfo.abbrName, aReport.abbrname) != 0)
            { // not bolone to this order
                return;
            }

            if (tracking.orderState == OrderState.WaitingFilled)
            {
                checkNonFilledLife();
            }

            if (tracking.orderState == OrderState.Filled)
            {
                updateTracking(aReport);
                protector.check(aReport);
                daytrade.check(aReport);
                profitTake.check(aReport);
                hardstop.check(aReport);
            }
            
        }

        public int getOrderID()
        {
            return orderinfo.orderId;
        }

        public OrderInfo getOrderInfo()
        {
            return orderinfo;
        }

        public OrderState getState()
        {
            return tracking.orderState;
        }

        public string getAbbrName()
        {
            return orderinfo.abbrName;
        }

        public OrderTrackingInfo getOrderTracking()
        {
            return tracking;
        }

        private void updateTracking(TickQuote aTick)
        {
            if (tracking.orderState == OrderState.Filled)
            {
                if (orderinfo.position > 0)
                {
                    tracking.currentProfit = aTick.trade - tracking.entryPz;
                }
                else if (orderinfo.position < 0)
                {
                    tracking.currentProfit = tracking.entryPz - aTick.trade;
                }
                tracking.currentProfitPercent = (tracking.currentProfit / tracking.entryPz);
                tracking.maxrunup = Math.Max(tracking.maxrunup, tracking.currentProfit);
                tracking.maxdrawdown = Math.Max(tracking.maxdrawdown, tracking.maxrunup - tracking.currentProfit);
                tracking.saveOnRedis();
            }
        }

        private void checkNonFilledLife()
        {
            if (tracking.orderState == OrderState.WaitingFilled)
            {
                DateTime cancelTime = tracking.filledTime.AddSeconds(secondsForFilled);
                if (DateTime.Now > cancelTime)
                {
                    logger.Info(getIdentifiString() + "WaitingFilled timeout... cancel order...");
                    cancel();
                }
            }
        }

        private void OnFilled(FilledOrderReport aReport)
        {
            if (String.Compare(orderinfo.getUserkey(), aReport.userkey) == 0)
            {
                filledReport = aReport;
                if (Convert.ToInt32(filledReport.filledNumber) == 1)
                {
                    tracking.filledTime = DateTime.Now;
                    changeState(OrderState.Filled);
                    tracking.entryPz = filledReport.avgPrice;
                    tracking.entryOI = DataCenter.Instance.getLastOI(orderinfo.abbrName);
                }
            }

            if (String.Compare(orderinfo.getUserkeyClosed(), aReport.userkey) == 0)
            {
                closedFilledReport = aReport;
                if (Convert.ToInt32(closedFilledReport.filledNumber) == 1)
                {
                    tracking.closedTime = DateTime.Now;
                    tracking.closedPz = closedFilledReport.avgPrice;
                    tracking.closedOI = DataCenter.Instance.getLastOI(orderinfo.abbrName);
                    changeState(OrderState.Closed);
                }
            }
        }

        private void OnOrder(OrderStatusReport aReport)
        {
            logger.Info(aReport.info());
            if (String.Compare(orderinfo.getUserkey(), aReport.userkey) == 0)
            {
                orderStatusReport = aReport;
                if (tracking.orderState == OrderState.Submitted)
                {
                    if (String.Compare(aReport.status, "委托成功") == 0)
                    {
                        changeState(OrderState.WaitingFilled);
                    }

                    if (String.Compare(aReport.status, "委托失败") == 0)
                    {
                        changeState(OrderState.OrderFailed);
                    }
                }

                if (tracking.orderState == OrderState.CancealOrder)
                {
                    if (String.Compare(aReport.status, "委托删单成功") == 0)
                    {
                        changeState(OrderState.Canceled);
                    }
                }
            }

        }

        private void changeState(OrderState aState)
        {
            logger.Info(getIdentifiString() + "userkey..." + orderinfo.getUserkey() + ".from ." + tracking.orderState.ToString() + "..to.." + aState);
            tracking.orderState = aState;
            tracking.saveOnRedis();
        }

        public string getProfitTakeParStr()
        {
            return profitTake.getParStr();
        }

        public string getHardstopParStr()
        {
            return hardstop.getParStr();
        }

        public string getDayTradeParStr()
        {
            return daytrade.getParStr();
        }

        public string getProtectorParStr()
        {
            return protector.getParStr();
        }

        public void fireOrder(string reason)
        {
            if (tracking.orderState != OrderState.WaitingSubmit)
            {
                return;
            }
            string bsstring;
            if (orderinfo.position > 0)
            {
                bsstring = "B";
            }
            else if (orderinfo.position < 0)
            {
                bsstring = "S";
            }
            else
            {
                bsstring = "";
                changeState(OrderState.GiveUp);
            }

            int lot = Math.Abs(orderinfo.position);

            double pz = -99999999999;
            double price = DataCenter.Instance.getLastTrade(orderinfo.abbrName);
            double ticksize = SymbolManager.Instance.getTickSize(orderinfo.abbrName);

            if (orderinfo.position > 0)
            {
                pz = price + SymbolManager.Instance.getOb95Tick(orderinfo.abbrName) * ticksize;
            }
            else if (orderinfo.position < 0)
            {
                pz = price - SymbolManager.Instance.getOb95Tick(orderinfo.abbrName) * ticksize;
            }

            if (tracking.orderState != OrderState.GiveUp)
            {
                logger.Info(getIdentifiString() + reason + " submitted an order");
                TradeCenter.Instance.openFutureOrderLimit(orderinfo.account,
                    orderinfo.getIceId(), bsstring,
                    Convert.ToString(pz),
                    orderinfo.getUserkey());
                logger.Info(getIdentifiString() + "order submit -> " + info());
            }
            else
            {
                logger.Info(getIdentifiString() + "sumbit an invalid order.");
            }

            changeState(OrderState.Submitted);

        }

        private string info()
        {
            //TODO
            return "ToDo"; //(orderinfo.info());
        }

        public void closingOrder(string reason)
        {
            if (tracking.orderState != OrderState.Filled)
            {
                return;
            }

            string bsstring;
            if (orderinfo.position > 0)
            {
                bsstring = "S";
            }
            else if (orderinfo.position < 0)
            {
                bsstring = "B";
            }
            else
            {
                bsstring = "";
                logger.Info(getIdentifiString() + "Error! BS String exception!");
                changeState(OrderState.GiveUp);
            }

            int lot = Math.Abs(orderinfo.position);

            double pz = -99999999999;
            double ticksize = SymbolManager.Instance.getTickSize(orderinfo.abbrName);
            double currTrade = DataCenter.Instance.getLastTrade(orderinfo.abbrName);

            if (orderinfo.position > 0)
            {
                pz = currTrade - SymbolManager.Instance.getOb99Tick(orderinfo.abbrName) * ticksize;
            }
            else if (orderinfo.position < 0)
            {
                pz = currTrade + SymbolManager.Instance.getOb99Tick(orderinfo.abbrName) * ticksize;
            }

            if (tracking.orderState != OrderState.GiveUp)
            {
                changeState(OrderState.WaitingClose);
                tracking.closingTime = DateTime.Now;
                TradeCenter.Instance.closeFutureOrderLimit(orderinfo.account,
                    orderinfo.getIceId(), bsstring,
                    Convert.ToString(pz),
                    orderinfo.getUserkeyClosed());
                logger.Info(getIdentifiString() + "order closing reason -> " + reason);
                logger.Info(getIdentifiString() + "order closing submit -> " + info());
            }
        }

        private void cancel()
        {
            if (tracking.orderState != OrderState.WaitingFilled)
            {
                logger.Info(getIdentifiString() + "Cancel : Order is not in canceable state.");
                return;
            }
            changeState(OrderState.CancealOrder);
            if (String.Compare(orderStatusReport.orderNumber, "") != 0)
            {
                TradeCenter.Instance.cancealFutureOrder(orderStatusReport.orderNumber);
            }
        }

        public void invokeDayTrade()
        {
            daytrade.enable();
        }

        public void invokeProfitTake(double takePercent)
        {
            profitTake.enable(takePercent);
        }

        public void invokeHardstop(double stopPercent)
        {
            hardstop.enable(stopPercent);
        }

        public void invokeProtector(double aPercentRunupTrigger, double aPercentDrawdown)
        {
            protector.enable(aPercentRunupTrigger, aPercentDrawdown);
        }


    }
}
