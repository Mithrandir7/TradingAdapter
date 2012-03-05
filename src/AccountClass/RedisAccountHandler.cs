using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountClass
{
    public class RedisAccountHandler
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static RedisAccountHandler Instance = new RedisAccountHandler();

        private RedisAccountHandler()
        {

        }
        

        public void push(Account aAccount)
        {
            string lkey = "ACCOUNT:" + aAccount.account;
            UtilityClass.RedisUtil.Instance.set(lkey, aAccount.getInfo());            
        }


        public void clear()
        {
            logger.Info("RedisAccountHandler : clear account records");
            string header = "ACCOUNT:*";
            List<string> lkeys = UtilityClass.RedisUtil.Instance.search(header);
            if (lkeys.Count > 0)
            {
                UtilityClass.RedisUtil.Instance.del(lkeys.ToArray<string>());
            }
        }

    }
}
