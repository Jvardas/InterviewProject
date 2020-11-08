using IPInfoWebAPI.Models;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public interface IIPRequestHandlerService
    {
        IpDetail CheckCacheForIp(string ip);
        Task<IpDetail> UpdateCacheAsync(IpDetail ip);
        Task<IpDetail> CheckLibraryForIPAsync(string ip);
    }
}
