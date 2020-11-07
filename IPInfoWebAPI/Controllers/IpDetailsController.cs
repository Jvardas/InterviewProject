using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IPInfoWebAPI.Models;

namespace IPInfoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpDetailsController : ControllerBase
    {
        private readonly IpInformationsContext _context;

        public IpDetailsController(IpInformationsContext context)
        {
            _context = context;
        }

        // GET: api/IpDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpDetail>>> GetIpdetails()
        {
            return await _context.Ipdetails.ToListAsync();
        }

        // GET: api/IpDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IpDetail>> GetIpDetail(int id)
        {
            var ipDetail = await _context.Ipdetails.FindAsync(id);

            if (ipDetail == null)
            {
                return NotFound();
            }

            return ipDetail;
        }

        // PUT: api/IpDetails/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIpDetail(int id, IpDetail ipDetail)
        {
            if (id != ipDetail.Id)
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
                if (!IpDetailExists(id))
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
            _context.Ipdetails.Add(ipDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IpDetailExists(ipDetail.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetIpDetail", new { id = ipDetail.Id }, ipDetail);
        }

        // DELETE: api/IpDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IpDetail>> DeleteIpDetail(int id)
        {
            var ipDetail = await _context.Ipdetails.FindAsync(id);
            if (ipDetail == null)
            {
                return NotFound();
            }

            _context.Ipdetails.Remove(ipDetail);
            await _context.SaveChangesAsync();

            return ipDetail;
        }

        private bool IpDetailExists(int id)
        {
            return _context.Ipdetails.Any(e => e.Id == id);
        }
    }
}
