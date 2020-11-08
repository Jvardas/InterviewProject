using AutoMapper;
using IPInfoWebAPI.DataTransferObjects;
using IPInfoWebAPI.Models;
using IPInfoWebAPI.Repositories;
using IPInfoWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpDetailsController : ControllerBase
    {
        private readonly IpInformationsContext Context;
        private readonly IIPRequestHandlerService IPRequestHandlerService;
        private readonly IIPDetailsRepository IPDetailsRepository;
        private readonly IMapper Mapper;

        public IpDetailsController(IpInformationsContext context, IIPRequestHandlerService iPRequestHandlerService, IIPDetailsRepository iPDetailsRepository, IMapper mapper)
        {
            Context = context;
            IPRequestHandlerService = iPRequestHandlerService;
            IPDetailsRepository = iPDetailsRepository;
            Mapper = mapper;
        }

        // GET: api/IpDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpDetailDTO>>> GetIpdetails()
        {
            List<IpDetailDTO> ipList = new List<IpDetailDTO>();
            
            var ips = await IPDetailsRepository.GetIpDetailsAsync();
            List<IpDetail> ipd = ips.ToList();
            foreach (var ip in ipd)
            {
                ipList.Add(Mapper.Map<IpDetailDTO>(ip));
            }

            return ipList;
        }

        // GET: api/IpDetails/192.168.1.1
        [HttpGet("{ip}")]
        public async Task<ActionResult<IpDetailDTO>> GetIpDetail(string ip)
        {
            try
            {
                var ipDetailsFromCache = IPRequestHandlerService.CheckCacheForIp(ip);
                if (ipDetailsFromCache != null)
                {
                    var ipdDTO = Mapper.Map<IpDetailDTO>(ipDetailsFromCache);
                    return ipdDTO;
                }

                var ipDetailsFromDb = await IPDetailsRepository.GetIpDetailAsync(ip);
                if (ipDetailsFromDb != null)
                {
                    IPRequestHandlerService.UpdateCache(ipDetailsFromDb);
                    var ipdDTO = Mapper.Map<IpDetailDTO>(ipDetailsFromDb);
                    return ipdDTO;
                }

                var ipDetailsFromLibrary = IPRequestHandlerService.CheckLibraryForIP(ip);
                if (ipDetailsFromLibrary.Result != null && ipDetailsFromLibrary.Exception == null)
                {
                    await IPDetailsRepository.AddIpDetailsAsync(ipDetailsFromLibrary.Result);
                    IPRequestHandlerService.UpdateCache(ipDetailsFromLibrary.Result);
                    var ipdDTO = Mapper.Map<IpDetailDTO>(ipDetailsFromLibrary.Result);
                    return ipdDTO;
                }
                else if(ipDetailsFromLibrary.Exception != null)
                {
                    throw new Exception();
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

            Context.Entry(ipDetail).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
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
        public async Task<ActionResult<IpDetailDTO>> PostIpDetail(IpDetail ipDetail)
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
        public async Task<ActionResult<IpDetailDTO>> DeleteIpDetail(string ip)
        {
            var ipDetail = await Context.Ipdetails.FindAsync(ip);
            if (ipDetail == null)
            {
                return NotFound();
            }

            Context.Ipdetails.Remove(ipDetail);
            await Context.SaveChangesAsync();
            var ipdDTO = Mapper.Map<IpDetailDTO>(ipDetail);
            return ipdDTO;
        }

        private bool IpDetailExists(string ip)
        {
            return Context.Ipdetails.Any(e => e.Ip == ip);
        }
    }
}
