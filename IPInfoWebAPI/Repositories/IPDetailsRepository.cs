using IPInfoProviderLibrary;
using IPInfoWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Repositories
{
    public class IPDetailsRepository : IIPDetailsRepository
    {
        private readonly IpInformationsContext _context;

        public IPDetailsRepository(IpInformationsContext context)
        {
            _context = context;
        }

        public async Task<int> AddIpDetailsAsync(IpDetail ipDetail)
        {
            _context.Ipdetails.Add(ipDetail);

            return await _context.SaveChangesAsync();
        }

        public async Task<IpDetail> GetIpDetailAsync(string ip)
        {
            var ipDetail = await _context.Ipdetails.FindAsync(ip);

            return ipDetail;
        }

        public async Task<IEnumerable<IpDetail>> GetIpDetailsAsync()
        {
            return await _context.Ipdetails.ToListAsync();
        }

        public Task<bool> UpdateIpDetailAsync(IpDetail ipDetail)
        {
            throw new NotImplementedException();
        }
    }
}
