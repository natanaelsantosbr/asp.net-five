using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeLive)
        {
            if (response == null)
                return;

            var serializedResponse = JsonConvert.SerializeObject(response);


            await this._distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeLive
            });

            
        }

        public async Task<string> GetCacheResponseAsync(string cacheKey)
        {
            

            var cacheResponse = await this._distributedCache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;

            
        }
    }
}
