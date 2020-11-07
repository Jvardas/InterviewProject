using IPInfoProviderLibrary;
using IPInfoWebAPI.Models;
using IPInfoWebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public class IPRequestHandlerService : IIPRequestHandlerService
    {
        private readonly IIPInfoProvider IPInfoProvider;

        public IPRequestHandlerService(IIPInfoProvider iPInfoProvider)
        {
            IPInfoProvider = iPInfoProvider;
        }

        public async Task<IpDetail> CheckCacheForIp(string ip)
        {
            throw new NotImplementedException();
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

        public async Task<bool> UpdateCache(IpDetail ip)
        {
            throw new NotImplementedException();
        }
    }
}
