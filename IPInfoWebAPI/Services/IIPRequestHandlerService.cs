using IPInfoWebAPI.Models;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public interface IIPRequestHandlerService
    {
        Task<IpDetail> CheckCacheForIp(string ip);
        Task<bool> UpdateCache(IpDetail ip);
        Task<IpDetail> CheckLibraryForIP(string ip);
    }
}
