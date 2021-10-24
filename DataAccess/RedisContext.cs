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

            if (_db == null)
            {
                ConfigurationOptions option = new ConfigurationOptions();
                option.Ssl = false;
                option.EndPoints.Add(host, port);
                var connect = ConnectionMultiplexer.Connect(option);
                _db = connect.GetDatabase();
            }

            return _db;
        }

        /// <summary>
        /// Anahtarda depolanan set değerinin tüm üyelerini döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            var rValue = _db.SetMembers(key);
            if (rValue.Length == 0)
                return default(T);

            var result = Deserialize<T>(rValue.ToStringArray());
            return result;
        }

        /// <summary>
        /// Belirtilen üyeyi anahtarda depolanan kümeden çıkarır.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Belirtilen üyeyi anahtarda saklanan kümeye ekler. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Set(string key, object data)
        {
            if (data == null)
                return false;

            var entryBytes = Serialize(data);
            return _db.SetAdd(key, entryBytes);
        }


    }
}
