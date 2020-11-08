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
        private IpInformationsContext Context { get; }

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

        public async Task UpdateIpDetailsAsync(IEnumerable<IpDetail> ipDetails)
        {
            foreach (var ipd in ipDetails)
            {
                if (Context.Ipdetails.Any(e => e.Ip == ipd.Ip))
                {
                    Context.Entry(ipd).State = EntityState.Modified;
                }
                else
                {
                    Context.Ipdetails.Add(ipd);
                }
            }

            await Context.SaveChangesAsync();
        }
    }
}
