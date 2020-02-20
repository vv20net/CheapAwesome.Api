using CheapAwesome.Core.Common.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheapAwesome.Core.Services.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly MemoryCacheSettings Settings;
        private readonly MemoryCache Cache;

        public MemoryCacheService(MemoryCacheSettings settings)
        {
            Settings = settings;

            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = settings.SizeLimit
            });
        }

        public T GetOrAdd<T>(
            string key, 
            Func<T> producer, 
            Func<T, bool> validator, 
            TimeSpan expiration) where T : class
        {
            if (!Settings.Enabled)
                return producer();

            var value = Cache.Get(key);

            if (validator((T)value))
                return (T)value;

            var item = producer();

            if (item != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetAbsoluteExpiration(expiration);

                Cache.Set(key, item, cacheEntryOptions);
            }

            return item;
        }
    }
}
