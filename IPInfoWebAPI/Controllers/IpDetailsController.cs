using AutoMapper;
using IPInfoWebAPI.DataTransferObjects;
using IPInfoWebAPI.Models;
using IPInfoWebAPI.Repositories;
using IPInfoWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
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

        // POST: api/UpdateIpDetailsWithBatches/[IpDetails1, IpDetails2,..., IpDetailsN]
        [HttpPost]
        public async Task<ActionResult<Guid>> UpdateIpDetailsWithBatches(IEnumerable<IpDetail> ipDetails)
        {
            throw new NotImplementedException();
        }
    }
}
