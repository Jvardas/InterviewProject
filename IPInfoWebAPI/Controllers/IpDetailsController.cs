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
        private readonly IpInformationsContext _context;
        private readonly IIPRequestHandlerService _ipRequestHandlerService;
        private readonly IIPDetailsRepository _ipDetailsRepository;
        private readonly IMapper _mapper;

        public IpDetailsController(IpInformationsContext context, IIPRequestHandlerService ipRequestHandlerService, IIPDetailsRepository ipDetailsRepository, IMapper mapper)
        {
            _context = context;
            _ipRequestHandlerService = ipRequestHandlerService;
            _ipDetailsRepository = ipDetailsRepository;
            _mapper = mapper;
        }

        // GET: api/IpDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpDetailDTO>>> GetIpdetails()
        {
            List<IpDetailDTO> ipList = new List<IpDetailDTO>();
            
            var ips = await _ipDetailsRepository.GetIpDetailsAsync();
            List<IpDetail> ipd = ips.ToList();
            foreach (var ip in ipd)
            {
                ipList.Add(_mapper.Map<IpDetailDTO>(ip));
            }

            return ipList;
        }

        // GET: api/IpDetails/192.168.1.1
        [HttpGet("{ip}")]
        public async Task<ActionResult<IpDetailDTO>> GetIpDetail(string ip)
        {
            try
            {
                var ipDetailsFromCache = _ipRequestHandlerService.CheckCacheForIp(ip);
                if (ipDetailsFromCache != null)
                {
                    var ipdDTO = _mapper.Map<IpDetailDTO>(ipDetailsFromCache);
                    return ipdDTO;
                }

                var ipDetailsFromDb = await _ipDetailsRepository.GetIpDetailAsync(ip);
                if (ipDetailsFromDb != null)
                {
                    await _ipRequestHandlerService.UpdateCacheAsync(ipDetailsFromDb);
                    var ipdDTO = _mapper.Map<IpDetailDTO>(ipDetailsFromDb);
                    return ipdDTO;
                }

                var ipDetailsFromLibrary = await _ipRequestHandlerService.CheckLibraryForIPAsync(ip);
                if (ipDetailsFromLibrary != null)
                {
                    await _ipDetailsRepository.AddIpDetailsAsync(ipDetailsFromLibrary);
                    await _ipRequestHandlerService.UpdateCacheAsync(ipDetailsFromLibrary);
                    var ipdDTO = _mapper.Map<IpDetailDTO>(ipDetailsFromLibrary);
                    return ipdDTO;
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
        public async Task<ActionResult<IpDetailDTO>> PostIpDetail(IpDetail ipDetail)
        {
            try
            {
                await _ipDetailsRepository.AddIpDetailsAsync(ipDetail);
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
            var ipDetail = await _context.Ipdetails.FindAsync(ip);
            if (ipDetail == null)
            {
                return NotFound();
            }

            _context.Ipdetails.Remove(ipDetail);
            await _context.SaveChangesAsync();
            var ipdDTO = _mapper.Map<IpDetailDTO>(ipDetail);
            return ipdDTO;
        }

        private bool IpDetailExists(string ip)
        {
            return _context.Ipdetails.Any(e => e.Ip == ip);
        }
    }
}
