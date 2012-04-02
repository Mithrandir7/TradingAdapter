using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace Tickdata
{
    public class TickQuoteTw
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public DateTime datetime { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double volume { get; set; }
        public double oi { get; set; }        
    }
}
