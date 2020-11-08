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
        private readonly IpInformationsContext Context;

        public IPDetailsRepository(IpInformationsContext context)
        {
            Context = context;
        }

        public async Task<int> AddIpDetailsAsync(IpDetail ipDetail)
        {
            Context.Ipdetails.Add(ipDetail);

            return await Context.SaveChangesAsync();
        }

        public async Task<IpDetail> GetIpDetailAsync(string ip)
        {
            return await Context.Ipdetails.FindAsync(ip);
        }

        public async Task<IEnumerable<IpDetail>> GetIpDetailsAsync()
        {
            return await Context.Ipdetails.ToListAsync();
        }

        public Task<bool> UpdateIpDetailAsync(IpDetail ipDetail)
        {
            throw new NotImplementedException();
        }
    }
}
