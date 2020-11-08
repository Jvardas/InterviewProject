using IPInfoWebAPI.Models;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public interface IIPRequestHandlerService
    {
        IpDetail CheckCacheForIp(string ip);
        IpDetail UpdateCache(IpDetail ip);
        Task<IpDetail> CheckLibraryForIP(string ip);
    }
}
