using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IPInfoWebAPI.Models;
using IPInfoProviderLibrary;
using IPInfoWebAPI.Repositories;
using IPInfoWebAPI.Services;

namespace IPInfoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpDetailsController : ControllerBase
    {
        private readonly IpInformationsContext _context;
        private readonly IIPRequestHandlerService IPRequestHandlerService;
        private readonly IIPDetailsRepository IPDetailsRepository;

        public IpDetailsController(IpInformationsContext context, IIPRequestHandlerService iPRequestHandlerService, IIPDetailsRepository iPDetailsRepository)
        {
            _context = context;
            IPRequestHandlerService = iPRequestHandlerService;
            IPDetailsRepository = iPDetailsRepository;
        }

        // GET: api/IpDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpDetail>>> GetIpdetails()
        {
            return Ok(await IPDetailsRepository.GetIpDetailsAsync());
        }

        // GET: api/IpDetails/192.168.1.1
        [HttpGet("{ip}")]
        public async Task<ActionResult<IpDetail>> GetIpDetail(string ip)
        {
            try
            {
                var ipDetailsFromCache = IPRequestHandlerService.CheckCacheForIp(ip);
                if (ipDetailsFromCache != null)
                {
                    return Ok(ipDetailsFromCache);
                }

                var ipDetailsFromDb = await IPDetailsRepository.GetIpDetailAsync(ip);
                if (ipDetailsFromDb != null)
                {
                    return ipDetailsFromDb;
                }

                var ipDetailsFromLibrary = IPRequestHandlerService.CheckLibraryForIP(ip);
                if (ipDetailsFromLibrary != null)
                {
                    await IPDetailsRepository.AddIpDetailsAsync(ipDetailsFromLibrary.Result);
                    await IPRequestHandlerService.UpdateCache(ipDetailsFromLibrary.Result);
                    return Ok(ipDetailsFromLibrary.Result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/IpDetails/192.168.1.1
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{ip}")]
        public async Task<IActionResult> PutIpDetail(string ip, IpDetail ipDetail)
        {
            if (ip != ipDetail.Ip)
            {
                return BadRequest();
            }

            _context.Entry(ipDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IpDetailExists(ip))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/IpDetails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<IpDetail>> PostIpDetail(IpDetail ipDetail)
        {
            try
            {
                await IPDetailsRepository.AddIpDetailsAsync(ipDetail);
            }
            catch (DbUpdateException)
            {
                if (IpDetailExists(ipDetail.Ip))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetIpDetail", new { id = ipDetail.Ip }, ipDetail);
        }

        // DELETE: api/IpDetails/192.168.1.1
        [HttpDelete("{ip}")]
        public async Task<ActionResult<IpDetail>> DeleteIpDetail(string ip)
        {
            var ipDetail = await _context.Ipdetails.FindAsync(ip);
            if (ipDetail == null)
            {
                return NotFound();
            }

            _context.Ipdetails.Remove(ipDetail);
            await _context.SaveChangesAsync();

            return ipDetail;
        }

        private bool IpDetailExists(string ip)
        {
            return _context.Ipdetails.Any(e => e.Ip == ip);
        }
    }
}
