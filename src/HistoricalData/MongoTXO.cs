using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Tickdata;
using UtilityClass;
using SymbolSearch;
using System.Threading;

namespace HistoricalData
{
    public class MongoTXO
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            SymbolSearch.SymbolSearch.Instance.init();
            TickData.Instance.init();
            MongoUtility.Instance.init();
        }


        private TickQuoteTw getLatest(string iceId)
        {
            // if not exist, return null

            iceId = iceId.Replace(".", "_");

            var sortAscending = SortBy.Ascending("datetime");
            var sortDescending = SortBy.Descending("datetime");

            //TickQuoteTw minDocument = MongoUtility.Instance.getCollection(symbolGroup, iceId).FindAllAs<TickQuoteTw>().SetSortOrder(sortAscending).SetLimit(1).First();
            TickQuoteTw maxDocument;
            try
            {
                maxDocument = MongoUtility.Instance.getCollection(symbolGroup, iceId).FindAllAs<TickQuoteTw>().SetSortOrder(sortDescending).SetLimit(1).First();
            }
            catch (System.InvalidOperationException e)
            {
                maxDocument = null;
                logger.Info("getLatest : data not exist : " + iceId);
            }
            return maxDocument;

        }

        public void updateTXO(string iceid, string yyyymmdd)
        {          
            List<TickQuoteTw> quotes = TickData.Instance.getQuotes(iceid, yyyymmdd);
            MongoTXO.Instance.push(iceid, quotes);           
        }

        public void updateTXO(string yyyymmdd)
        {
            List<Symbol> symbols = SymbolSearch.SymbolSearch.Instance.getTXOSymbols();
            foreach (Symbol s in symbols)
            {
                string iceid = s.iceid;
                logger.Info("update data : "+iceid+" on "+yyyymmdd);                
                List<TickQuoteTw> quotes = TickData.Instance.getQuotes(iceid, yyyymmdd);
                MongoTXO.Instance.push(s.iceid, quotes);
                Thread.Sleep(30000); // 30 seconds
            }
        }

        public void updateYestoday()
        {
            string yyyymmdd = DateTimeFunc.getYesterdayYYYYMMDD().ToString();
            updateTXO(yyyymmdd);
        }

        private void push(string iceSymbol, List<TickQuoteTw> quotes)
        {
            iceSymbol = iceSymbol.Replace(".", "_");
            foreach (TickQuoteTw quote in quotes)
            {
                MongoUtility.Instance.getCollection(symbolGroup, iceSymbol).Save<TickQuoteTw>(quote);
            }

            MongoUtility.Instance.getCollection(symbolGroup, iceSymbol).EnsureIndex(
                new IndexKeysBuilder().Ascending("datetime"), IndexOptions.SetUnique(true));
        }










    }
}
