using DataAccess.Abstract;
using DataAccess.Concrete;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class RedisContext : CacheHelper, ICacheManager
    {
        private static IDatabase _db;
        private static readonly string host = "localhost";
        private static readonly int port = 6379;

        public RedisContext()
        {
            CreateRedisDB();
        }

        private static IDatabase CreateRedisDB()
        {

            string ErrorMessage = "";
            if (_db == null)
            {

                try
                {

                    ConfigurationOptions option = new ConfigurationOptions();
                    option.Ssl = false;
                    option.EndPoints.Add(host, port);
                    var connect = ConnectionMultiplexer.Connect(option);
                    _db = connect.GetDatabase();
                }
                catch (Exception ex)
                {
                    throw new Exception("Maraba");
                }


            }

            return _db;
        }

        public T Get<T>(string key)
        {
            if (_db.IsConnected(key) == true)
            {
                //throw new Exception();
            }

            var rValue = _db.SetMembers(key);
            if (rValue.Length == 0)
                return default(T);

            var result = Deserialize<T>(rValue.ToStringArray());
            return result;
        }

        public bool IsSet(string key)
        {
            return _db.KeyExists(key);
        }

        public bool RemoveValue(string key, object data)
        {
            if (data == null)
                return false;

            var entryBytes = Serialize(data);
            return _db.SetRemove(key, entryBytes);
        }

        public bool Remove(string key)
        {
            return _db.KeyDelete(key);
        }

        public bool Set(string key, object data)
        {
            if (data == null)
                return false;

            var entryBytes = Serialize(data);
            return _db.SetAdd(key, entryBytes);


            //  expire ;
            // var expiresIn = TimeSpan.FromMinutes(cacheTime);

            //if (cacheTime > 0)
            //    _db.KeyExpire(key, expiresIn);
        }


    }
}
