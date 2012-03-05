using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis;

namespace com.youholder.redis
{
    public class RedisClientFactory
    {        

        public RedisClientFactory()
        {
            
        }

        public static RedisClient getNewInstance()
        {
            return new RedisClient();
        }

    }
}
