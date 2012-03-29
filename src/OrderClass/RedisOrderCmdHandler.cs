using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AccountClass;
using UtilityClass;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;
using TCMonitor;

namespace OrderClass
{
    public class RedisOrderCmdHandler
    {
        public static RedisOrderCmdHandler Instance = new RedisOrderCmdHandler();

        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string channelname = "placeorder";
        private string [] channelnames = {"placeorder"};
        private RedisClient redisClient;

        private RedisOrderCmdHandler()
        {
               
        }

        public void init()
        {
            redisClient = RedisUtil.getRedisClientInstance();
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSub)); 
        }

        public string getChannelname()
        {
            return channelname;
        }


        private void doSub(object parameter)
        {
            IRedisSubscription subClient = redisClient.CreateSubscription();
            subClient.OnSubscribe += new Action<string>(onSub);
            subClient.OnMessage += new Action<string,string>(onOrderMsg);
            subClient.SubscribeToChannels(channelnames);
        }


        private void onSub(string channel)
        {
            logger.Info("onSubMsg : channel = #" + channel+"#");
        }

        private void onOrderMsg(string channel, string value)
        {
        
            if (String.Compare(channel, channelnames[0])!=0)
            {
                return;
            }

            logger.Info("onOrderMsg : placeOrder message recieved = " + value);

            OrderCmd orderCmd = new OrderCmd();

            string[] items = value.Split(';');

            #region
            foreach (string item in items)
            {
                string[] lArr = item.Split('=');
                if (lArr.Length == 2)
                {
                    string key = lArr[0].ToLower().Trim();
                    switch (key)
                    {
                        case "orderid":
                            orderCmd.orderInfo.orderId = Convert.ToInt32(lArr[1]);
                            orderCmd.orderBehaviorParameters.orderId = orderCmd.orderInfo.orderId;
                            break;
                        case "account":
                            orderCmd.orderInfo.account = lArr[1].Trim();
                            break;
                        case "symbol":
                            orderCmd.orderInfo.abbrName = lArr[1].ToLower().Trim();
                            break;
                        case "position":
                            orderCmd.orderInfo.position = Convert.ToInt32(lArr[1]);
                            break;
                        case "daytrade":
                            orderCmd.orderBehaviorParameters.daytrade = Convert.ToBoolean(lArr[1]);
                            break;
                        case "closed":
                            orderCmd.closed = Convert.ToBoolean(lArr[1]);
                            break;
                        case "profittakepercent":
                            orderCmd.orderBehaviorParameters.profittakepercent = Convert.ToDouble(lArr[1]);
                            break;
                        case "protectiontrigger":
                            orderCmd.orderBehaviorParameters.protectiontrigger = Convert.ToDouble(lArr[1]);
                            break;
                        case "protection":
                            orderCmd.orderBehaviorParameters.protection = Convert.ToDouble(lArr[1]);
                            break;
                        case "hardstop":
                            orderCmd.orderBehaviorParameters.hardstop = Convert.ToDouble(lArr[1]);
                            break;
                        default:                            
                            break;
                    }
                }
            }

            #endregion

            if (orderCmdValidation(orderCmd))
            {
                logger.Info("onOrderMsg ==> " + orderCmd.getInfo());
                //check touchance active or not

                if (TCMonitor.TouchanceMonitor.Instance.isTouchanceExist())
                {
                    OrderManager.Instance.push(orderCmd);
                }
                else
                {
                    logger.Info("Touchance is not running in this machine, command ignored.");
                }
            }

        }

        private bool orderCmdValidation(OrderCmd aCmd)
        {
            bool isOrderExist = OrderManager.Instance.isOrderIDExist(aCmd.orderInfo.orderId);

            if (isOrderExist)
            {                               
                return true;                
            }
            else
            {
                if (Math.Abs(aCmd.orderInfo.position) != 1)
                {
                    return false;
                }

                if (!AccountClass.AccountManager.Instance.isValidAccount(aCmd.orderInfo.account))
                {
                    return false;
                }

                if (!SymbolClass.SymbolManager.Instance.isValidSymbol(aCmd.orderInfo.abbrName))
                {
                    return false;
                }
                return true;
            }
            
        }
    }
}
