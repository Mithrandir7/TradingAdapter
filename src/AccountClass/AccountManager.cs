using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountClass
{
    public class AccountManager
    {
        private static List<string> accountFingerPrint = new List<string>();

        private static Dictionary<string, Account> accounts = new Dictionary<string, Account>();

        public static void add(Account aAccount)
        {
            if (!accounts.ContainsKey(aAccount.account))
            {
                accounts.Add(aAccount.account,aAccount);
            }
        }

        public static void addPosition(Position aPosition)
        {
            if (accounts.ContainsKey(aPosition.account))
            {
                accounts[aPosition.account].addPosition(aPosition);
            }
        }

        public static void clear()
        {
            accounts.Clear();
        }

        public static Dictionary<string, Account> getAccounts()
        {
            return accounts;
        }

        private static bool isRealTrade = false;
        private static string realAccount = "880937";

        static AccountManager()
        {
            if (isRealTrade)
            {
                accountFingerPrint.Add(realAccount);
            }
        }

        public static void setFingerPrint(List<string> aFP)
        {
            foreach (string lstr in aFP)
            {
                accountFingerPrint.Add(lstr);
            }
        }

        public static List<string> getAllValidAccount()
        {
            List<string> lrtn = new List<string>();
            foreach (string lstr in accounts.Keys)
            {
                if (isValidAccount(lstr))
                {
                    lrtn.Add(lstr);
                }
            }
            return lrtn;
        }


        public static List<string> getAccountByPattern(string aPattern)
        {
            List<string> lrtn = new List<string>();
            foreach (string lstr in accounts.Keys)
            {
                if (lstr.Contains(aPattern))
                {
                    lrtn.Add(lstr);
                }
            }
            return lrtn;
        }

        public static bool isRealAccount(string aAccount)
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

        public static bool isValidAccount(string aAccount)
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
