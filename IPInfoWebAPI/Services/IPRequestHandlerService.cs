using IPInfoProviderLibrary;
using IPInfoWebAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public class IPRequestHandlerService : IIPRequestHandlerService
    {
        private readonly IIPInfoProvider _ipInfoProvider;
        private readonly IMemoryCache _memoryCache;

        private static readonly SemaphoreSlim GetSemaphore = new SemaphoreSlim(1, 1);

        public IPRequestHandlerService(IIPInfoProvider ipInfoProvider, IMemoryCache memoryCache)
        {
            _ipInfoProvider = ipInfoProvider;
            _memoryCache = memoryCache;
        }

        public IpDetail CheckCacheForIp(string ip)
        {
            var ipd = _memoryCache.Get<IpDetail>(ip);
            if (ipd != null)
            {
                return ipd;
            }
            else
            {
                return null;
            }
        }

        public async Task<IpDetail> CheckLibraryForIPAsync(string ip)
        {
            return await Task.Run(() =>
            {
                var ipd = _ipInfoProvider.GetDetails(ip);
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
            });
        }

        public async Task<IpDetail> UpdateCacheAsync(IpDetail ip)
        {
            // with the sliding expiration the key will expire if noone access it, but if someone does access it then it will stay for another minute in memory
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60));

            try
            {
                await GetSemaphore.WaitAsync();
                var ipd = CheckCacheForIp(ip.Ip); // Recheck to make sure it didn't populate before entering semaphore
                if (ipd != null)
                {
                    return ipd;
                }

                _memoryCache.Set(ip.Ip, ip, cacheEntryOptions);
            }
            finally
            {
                GetSemaphore.Release();
            }

            return ip;
        }

        public async Task<IEnumerable<IpDetail>> UpdateManyCacheAsync(IEnumerable<IpDetail> ips)
        {
            foreach (var ipd in ips)
            {
                await UpdateCacheAsync(ipd);
            }

            return ips;
        }
    }
}
