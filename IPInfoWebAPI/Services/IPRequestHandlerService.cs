using IPInfoProviderLibrary;
using IPInfoWebAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public class IPRequestHandlerService : IIPRequestHandlerService
    {
        private readonly IIPInfoProvider IPInfoProvider;
        private readonly IMemoryCache MemoryCache;

        public IPRequestHandlerService(IIPInfoProvider iPInfoProvider, IMemoryCache memoryCache)
        {
            IPInfoProvider = iPInfoProvider;
            MemoryCache = memoryCache;
        }

        public IpDetail CheckCacheForIp(string ip)
        {
            var ipd = MemoryCache.Get<IpDetail>(ip);
            if (ipd != null)
            {
                return ipd;
            }
            else
            {
                return null;
            }
        }

        public async Task<IpDetail> CheckLibraryForIP(string ip)
        {
            var ipd = IPInfoProvider.GetDetails(ip);
            if (ipd != null)
            {
                var ipDetail = new IpDetail
                {
                    Ip = ip,
                    City = ipd.City,
                    Country = ipd.Country,
                    Continent = ipd.Continent,
                    Latitude = (decimal?)ipd.Latitude,
                    Longitude = (decimal?)ipd.Longitude
                };

                return ipDetail;
            }
            else
            {
                return null;
            }
        }

        public IpDetail UpdateCache(IpDetail ip)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60));
    
            return MemoryCache.Set(ip.Ip, ip, cacheEntryOptions);
        }
    }
}
