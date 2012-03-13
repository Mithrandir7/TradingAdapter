using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;
using UtilityClass;
using SymbolClass;
using AccountClass;
using DataManager;
using OrderClass;

namespace tradebox
{
    public class RandomEntryController
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static RandomEntryController Instance = new RandomEntryController();

        private List<string> accounts = new List<string>();
        private Dictionary<int,string> cmds = new Dictionary<int,string>();

        private RedisClient redisClient = new RedisClient();
        private string channelName;
        private int todayYYYYMMDD;

        private RandomEntryController()
        {
               
        }

        public void init()
        {
            accounts = AccountXmlReader.Instance.getAccountList();
            QuoteAdapter.Instant.addOnTickAction(OnTick);
            channelName = RedisOrderCmdHandler.Instance.getChannelname();
            todayYYYYMMDD = UtilityClass.DateTimeFunc.getYYYYMMDD();
            generatRandomOrderCmd();
            showCmds();         
        }


        private void OnTick(TickQuote aReport)
        {
            //logger.Info("Ontick:" + aReport.time);
            // cmds execute by tick;
            if(aReport.abbrname!="if")
            {
                return;
            }

            List<int> cmdExecuted = new List<int>();

            foreach ( KeyValuePair<int, string> pair in cmds)
            {
                if(pair.Key<aReport.time){
                    cmdExecuted.Add(pair.Key);                  
                    redisClient.PublishMessage(channelName,pair.Value);
                }
            }

            foreach (int cmdrm in cmdExecuted)
            {
                cmds.Remove(cmdrm);
            }

        }

        private void showCmds()
        {
            foreach (KeyValuePair<int, string> pair in cmds)
            {
                logger.Info("showCmds:"+pair.Key+"/"+pair.Value);
            }
        }

        private void generatRandomOrderCmd()
        {
            if (accounts.Count <= 0)
            {
                return;
            }

            int accountIndex = 1;
            foreach (string lac in accounts)
            {
                int lid = 1000*todayYYYYMMDD+accountIndex;
                string lprotection = getRandomProtection().ToString().Trim();
                string lpos = getRandomPosition().ToString().Trim();
                string lpt = Convert.ToString(0.1 * getRandomProfitTaker());
                string lhs = Convert.ToString(0.01 * getRandomHardstop());
                string lcmd = "orderid=" + lid.ToString().Trim() + ";account=" + lac + ";symbol=if;position="+lpos+";daytrade=true;" + 
                    "profittakepercent="+lpt+";protectiontrigger=0;protection="+lprotection+";hardstop="+lhs+";closed=false";

                int let = getRandomEntryTime();
                if(let<141500 & let>91600)
                {
                    cmds.Add(let,lcmd);
                    accountIndex = accountIndex +1;
                }
            }            
            //publish placeorder orderid=1;account=1999_2-0000049;symbol=if;position=1;daytrade=true;profittakepercent=0.5;protectiontrigger=0;protection=30;hardstop=0.8;closed=false
            
        }


        private int getRandomEntryTime()
        {
            int hhmmss = DateTimeFunc.getHHMMSS();
            int lsec = DateTimeFunc.hhmmss2Secs(hhmmss)+Misc.getRandom(600,6000);
            return DateTimeFunc.secs2hhmmss(lsec);
        }

        private int getRandomHardstop()
        {
            return Misc.getRandom(8, 15);
        }

        private int getRandomProfitTaker()
        {
            return Misc.getRandom(10, 30);
        }

        private int getRandomProtection()
        {
            return Misc.getRandom(25, 40);
        }
            

        private int getRandomPosition()
        {
            if (Misc.getRandom(0, 1000) > 500)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        


    }
}
