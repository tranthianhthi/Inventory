using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandidatePoolAPI;
using CandidatePoolAPI.Models;

namespace CandidatePoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrCandidateStatusController : ControllerBase
    {
        private readonly ACFCInventoryContext _context;

        public HrCandidateStatusController(ACFCInventoryContext context)
        {
            _context = context;
        }

        // GET: api/HrCandidateStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HrCandidateStatus>>> GetHrCandidateStatus()
        {
            return await _context.HrCandidateStatus.ToListAsync();
        }

        // GET: api/HrCandidateStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HrCandidateStatus>> GetHrCandidateStatus(int id)
        {
            var hrCandidateStatus = await _context.HrCandidateStatus.FindAsync(id);

            if (hrCandidateStatus == null)
            {
                return NotFound();
            }

            return hrCandidateStatus;
        }

        // PUT: api/HrCandidateStatus/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHrCandidateStatus(int id, HrCandidateStatus hrCandidateStatus)
        {
            if (id != hrCandidateStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(hrCandidateStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HrCandidateStatusExists(id))
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

        // POST: api/HrCandidateStatus
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<HrCandidateStatus>> PostHrCandidateStatus(HrCandidateStatus hrCandidateStatus)
        {
            _context.HrCandidateStatus.Add(hrCandidateStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHrCandidateStatus", new { id = hrCandidateStatus.Id }, hrCandidateStatus);
        }

        // DELETE: api/HrCandidateStatus/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HrCandidateStatus>> DeleteHrCandidateStatus(int id)
        {
            var hrCandidateStatus = await _context.HrCandidateStatus.FindAsync(id);
            if (hrCandidateStatus == null)
            {
                return NotFound();
            }

            _context.HrCandidateStatus.Remove(hrCandidateStatus);
            await _context.SaveChangesAsync();

            return hrCandidateStatus;
        }

        private bool HrCandidateStatusExists(int id)
        {
            return _context.HrCandidateStatus.Any(e => e.Id == id);
        }
    }
}
