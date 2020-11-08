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
        private readonly IIPRequestHandlerService _ipRequestHandlerService;
        private readonly IIPDetailsRepository _ipDetailsRepository;
        private readonly IMapper _mapper;
        private readonly IBatchRequestHandlerService _batchRequestHandlerService;

        public IpDetailsController(IIPRequestHandlerService ipRequestHandlerService, IIPDetailsRepository ipDetailsRepository, IMapper mapper, IBatchRequestHandlerService batchRequestHandlerService)
        {
            _ipRequestHandlerService = ipRequestHandlerService;
            _ipDetailsRepository = ipDetailsRepository;
            _mapper = mapper;
            _batchRequestHandlerService = batchRequestHandlerService;
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

        // GET: api/IpDetails/getipdetails/192.168.1.1
        [HttpGet("getipdetails/{ip}")]
        public async Task<ActionResult<IpDetailDTO>> GetIpDetail(string ip)
        {
            try
            {
                // ensure that I will only search for IPs and I will not have random strings in my Database
                if(!System.Text.RegularExpressions.Regex.IsMatch(ip, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$"))
                {
                    return NotFound();
                }

                // first check the cache
                var ipDetailsFromCache = _ipRequestHandlerService.CheckCacheForIp(ip);
                if (ipDetailsFromCache != null)
                {
                    var ipdDTO = _mapper.Map<IpDetailDTO>(ipDetailsFromCache);
                    return ipdDTO;
                }

                // then check the db. If it is in there then update cache and return
                var ipDetailsFromDb = await _ipDetailsRepository.GetIpDetailAsync(ip);
                if (ipDetailsFromDb != null)
                {
                    await _ipRequestHandlerService.UpdateCacheAsync(ipDetailsFromDb);
                    var ipdDTO = _mapper.Map<IpDetailDTO>(ipDetailsFromDb);
                    return ipdDTO;
                }

                // lastly get the IP from library. If its found then update both db and cache
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

        // POST: api/IpDetails/updateipsinbatch
        [HttpPost]
        [Route("updateipsinbatch")]
        public ActionResult<Guid> UpdateIpDetailsWithBatches([FromBody] IEnumerable<IpDetail> ipDetails)
        {
            var guid = _batchRequestHandlerService.IpDetailsUpdateJob(ipDetails);
            return guid;
        }

        // GET: api/IpDetails/jobprogress/00000000-0000-0000-0000-000000000000
        [HttpGet("jobprogress/{updateJobGuid:Guid}")]
        public ActionResult<string> GetUpdateJobProgress(Guid updateJobGuid)
        {
            var progress = _batchRequestHandlerService.GetUpdateJobProgress(updateJobGuid);
            if (progress != null)
            {
                return progress;
            }
            else
            {
                // as 404 I get only the guids that are not created by my service and not all those that have already been done
                return NotFound();
            }
        }
    }
}
