using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityClass;

namespace AccountClass
{
    public class AccountManager
    {
        private List<string> accountFingerPrint = new List<string>();

        private Dictionary<string, Account> accounts = new Dictionary<string, Account>();

        public static AccountManager Instance = new AccountManager();

        public void add(Account aAccount)
        {
            if (!accounts.ContainsKey(aAccount.account))
            {
                accounts.Add(aAccount.account,aAccount);
            }
        }

        public void addPosition(Position aPosition)
        {
            if (accounts.ContainsKey(aPosition.account))
            {
                accounts[aPosition.account].addPosition(aPosition);
            }
        }

        public void clear()
        {
            accounts.Clear();
        }

        public  Dictionary<string, Account> getAccounts()
        {
            return accounts;
        }

        private bool isRealTrade = false;
        private string realAccount = "880937";

        private AccountManager()
        {

            
        }

        public void init()
        {
            foreach (String ac in AccountXmlReader.Instance.getAccountList())
            {
                accountFingerPrint.Add(ac);
            }

            if (isRealTrade)
            {
                accountFingerPrint.Add(realAccount);
            }

            saveOnRedis();
        }

        private void saveOnRedis()
        {
            foreach(String aStr in accountFingerPrint){
                RedisUtil.Instance.set("Account:"+aStr, "");
            }
        }

        //public void setFingerPrint(List<string> aFP)
        //{
        //    foreach (string lstr in aFP)
        //    {
        //        accountFingerPrint.Add(lstr);
        //    }
        //}

        //public  List<string> getAllValidAccount()
        //{
        //    List<string> lrtn = new List<string>();
        //    foreach (string lstr in accounts.Keys)
        //    {
        //        if (isValidAccount(lstr))
        //        {
        //            lrtn.Add(lstr);
        //        }
        //    }
        //    return lrtn;
        //}


        //public  List<string> getAccountByPattern(string aPattern)
        //{
        //    List<string> lrtn = new List<string>();
        //    foreach (string lstr in accounts.Keys)
        //    {
        //        if (lstr.Contains(aPattern))
        //        {
        //            lrtn.Add(lstr);
        //        }
        //    }
        //    return lrtn;
        //}

        public  bool isRealAccount(string aAccount)
        {
            if (String.Compare(aAccount, realAccount) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public  bool isValidAccount(string aAccount)
        {
            foreach (string lstr in accountFingerPrint)
            {
                if (aAccount.Contains(lstr))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
