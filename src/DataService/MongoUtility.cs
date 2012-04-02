using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataService
{
    public class MongoUtility
    {

        public static MongoUtility Instance = new MongoUtility();

        private MongoUtility()
        {

        }

        MongoServer server;

        public void init()
        {
            string connectionString = "mongodb://localhost";
            server = MongoServer.Create(connectionString);
        }

        public MongoCollection getCollection(string dbName, string collectionName)
        {
            MongoDatabase db = server.GetDatabase(dbName);
            return db.GetCollection(collectionName);
        }



        public ~MongoUtility()
        {

        }


    }
}
