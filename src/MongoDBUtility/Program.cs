using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "mongodb://localhost";
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase database = server.GetDatabase("test");
            using (server.RequestStart(database))
            {
                // a series of operations that must be performed on the same connection
            }
        }
    }
}
