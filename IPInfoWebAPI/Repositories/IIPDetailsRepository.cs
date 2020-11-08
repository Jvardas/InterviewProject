using IPInfoWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Repositories
{
    public interface IIPDetailsRepository
    {
        Task<IEnumerable<IpDetail>> GetIpDetailsAsync();
        Task<IpDetail> GetIpDetailAsync(string ip);
        Task<int> AddIpDetailsAsync(IpDetail ipDetail);
        Task<IEnumerable<IpDetail>> UpdateIpDetailsAsync(IEnumerable<IpDetail> ipDetails);
    }
}
