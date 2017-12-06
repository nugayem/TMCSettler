using System;
using System.Collections.Generic;
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

    public class CachingProvider 
    {
        private readonly ILogger logger;
        public MemoryCache cache = new MemoryCache("CachingProvider");

        static readonly object padlock = new object();
        public CachingProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual void AddItem(string key, object value)
        {
            lock (padlock)
            {
                cache.Add(key, value, DateTimeOffset.MaxValue);
            }
        }

        public virtual void RemoveItem(string key)
        {
            lock (padlock)
            {
                cache.Remove(key);
            }
        }

        public virtual object GetItem(string key, bool remove)
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

                return res;
            }
        }

    }
}
