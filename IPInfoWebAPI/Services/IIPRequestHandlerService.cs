using IPInfoWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public interface IIPRequestHandlerService
    {
        IpDetail CheckCacheForIp(string ip);
        Task<IpDetail> UpdateCacheAsync(IpDetail ip);
        Task<IpDetail> CheckLibraryForIPAsync(string ip);
        Task<IEnumerable<IpDetail>> UpdateManyCacheAsync(IEnumerable<IpDetail> ips);
    }
}
