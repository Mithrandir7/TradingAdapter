using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityClass;

namespace UserkeyManager
{
    public class UserkeyFactory
    {
        public static UserkeyFactory Instance = new UserkeyFactory();
     
        private UserkeyFactory()
        {
        }

        public String getUserkey()
        {
            string dt = UtilityClass.DateTimeFunc.getYYYYMMDD().ToString();          
            return ("tradebox_" + dt + Convert.ToString(getUserkeyIndex()));
        }


        private long getUserkeyIndex()
        {
            long retInt = 1;
                        
            String dbkey = "userkeyManager.index";

            List<string> lkeys = UtilityClass.RedisUtil.Instance.search(dbkey);

            if (lkeys.Count == 0)
            {
                UtilityClass.RedisUtil.Instance.set(dbkey, retInt.ToString());
            }
            else
            {
                long cValue = UtilityClass.RedisUtil.Instance.getLong(dbkey);
                retInt = retInt + 1;
                UtilityClass.RedisUtil.Instance.set(dbkey, retInt.ToString());
            }
            return retInt;
        }
    }
}
