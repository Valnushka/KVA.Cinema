using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KVA.Cinema.Utilities
{
    public class CacheManager
    {
        private readonly IMemoryCache cache;

        public CacheManager(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        public SelectList GetCachedSelectList(string cacheKey, Func<IEnumerable<object>> functionToGetData, string dataValueField, string dataTextField)
        {
            if (!cache.TryGetValue(cacheKey, out SelectList selectList))
            {
                IEnumerable<object> data = functionToGetData();
                selectList = new SelectList(data, dataValueField, dataTextField);

                var options = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                cache.Set(cacheKey, selectList, options);
            }

            return selectList;
        }

        public void RemoveFromCache(string cacheKey)
        {
            cache.Remove(cacheKey);
        }
    }
}
