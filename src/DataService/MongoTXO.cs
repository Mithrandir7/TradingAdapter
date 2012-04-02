using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Tickdata;

namespace DataService
{
    public class MongoTXO
    {

        public static MongoTXO Instance = new MongoTXO();

        private string symbolGroup = "TXO"; // mongodb database name


        private MongoTXO()
        {

        }

        ~MongoTXO()
        {

        }

        public void init()
        {
            MongoUtility.Instance.init();
        }

        public void push(string iceSymbol, TickQuoteTw quote)
        {
            MongoUtility.Instance.getCollection(symbolGroup, iceSymbol).Save<TickQuoteTw>(quote);
        }

        public void push(string iceSymbol, List<TickQuoteTw> quotes)
        {
            foreach (TickQuoteTw quote in quotes)
            {
                MongoUtility.Instance.getCollection(symbolGroup, iceSymbol).Save<TickQuoteTw>(quote);
            }
            //MongoUtility.Instance.getCollection(symbolGroup, iceSymbol).EnsureIndex(
        }









    }
}
