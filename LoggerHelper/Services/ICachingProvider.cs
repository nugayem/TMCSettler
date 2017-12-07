using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace LoggerHelper.Services
{
    public interface ICachingProvider
    {
        void AddItem(string key, object value);
        object GetItem(string key);
    }

    public static class CachingProvider 
    {
        private static readonly ILogger logger;
        public static  MemoryCache cache = new MemoryCache("CachingProvider");

        static readonly object padlock = new object();
        //public CachingProvider(ILogger logger)
        //{
        //    this.logger = logger;
        //}

        public static  void AddItem(string key, object value)
        {
            lock (padlock)
            {
                cache.Add(key, value, DateTimeOffset.MaxValue);
            }
        }

        public static void RemoveItem(string key)
        {
            lock (padlock)
            {
                cache.Remove(key);
            }
        }

        public static T  GetCachedData<T>(string key, bool remove = false)
        {
            lock (padlock)
            {
                var res = cache[key];

                if (res != null)
                {
                    if (remove == true)
                        cache.Remove(key);
                }
                else
                {
                    logger.LogFatalMessage("CachingProvider-GetItem: Don't contains key: " + key);
                }

                return (T)res;
            }
        }

    }

    public class TypedObjectCache<T> : MemoryCache where T : class
    {
        private CacheItemPolicy HardDefaultCacheItemPolicy = new CacheItemPolicy()
        {
            SlidingExpiration = new TimeSpan(0, 15, 0)
        };

        private CacheItemPolicy defaultCacheItemPolicy;

        public TypedObjectCache(string name, NameValueCollection nvc = null, CacheItemPolicy policy = null) : base(name, nvc)
        {
            defaultCacheItemPolicy = policy ?? HardDefaultCacheItemPolicy;
        }

        public void Set(string cacheKey, T cacheItem, CacheItemPolicy policy = null)
        {
            policy = policy ?? defaultCacheItemPolicy;
            if (true /* Ektron.Com.Helpers.Constants.IsCachingEnabled */ )
            {
                base.Set(cacheKey, cacheItem, policy);
            }
        }

        public void Set(string cacheKey, Func<T> getData, CacheItemPolicy policy = null)
        {
            this.Set(cacheKey, getData(), policy);
        }

        public bool TryGetAndSet(string cacheKey, Func<T> getData, out T returnData, CacheItemPolicy policy = null)
        {
            if (TryGet(cacheKey, out returnData))
            {
                return true;
            }
            returnData = getData();
            this.Set(cacheKey, returnData, policy);
            return returnData != null;
        }

        public bool TryGet(string cacheKey, out T returnItem)
        {
            returnItem = (T)this[cacheKey];
            return returnItem != null;
        }

    }
}
