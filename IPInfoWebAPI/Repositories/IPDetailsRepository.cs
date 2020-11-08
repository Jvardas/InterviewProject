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
            return await _context.Ipdetails.FindAsync(ip);
        }

        public async Task<IEnumerable<IpDetail>> GetIpDetailsAsync()
        {
            return await _context.Ipdetails.ToListAsync();
        }

        public async Task<IEnumerable<IpDetail>> UpdateIpDetailsAsync(IEnumerable<IpDetail> ipDetails)
        {
            try
            {
                foreach (var ipd in ipDetails)
                {
                    if (_context.Ipdetails.Any(e => e.Ip == ipd.Ip))
                    {
                        _context.Entry(ipd).State = EntityState.Modified;
                    }
                    else
                    {
                        _context.Ipdetails.Add(ipd);
                    }
                }

                await _context.SaveChangesAsync();
                return ipDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
