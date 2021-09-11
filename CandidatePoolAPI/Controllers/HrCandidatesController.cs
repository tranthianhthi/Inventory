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
    public class HrCandidatesController : ControllerBase
    {
        private readonly ACFCInventoryContext _context;

        public HrCandidatesController(ACFCInventoryContext context)
        {
            _context = context;
        }

        // GET: api/HrCandidates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HrCandidate>>> GetHrCandidate()
        {
            return await _context.HrCandidate.ToListAsync();
        }

        // GET: api/HrCandidates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HrCandidate>> GetHrCandidate(int id)
        {
            var hrCandidate = await _context.HrCandidate.FindAsync(id);

            if (hrCandidate == null)
            {
                return NotFound();
            }

            return hrCandidate;
        }

        // PUT: api/HrCandidates/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHrCandidate(int id, HrCandidate hrCandidate)
        {
            if (id != hrCandidate.Id)
            {
                return BadRequest();
            }

            _context.Entry(hrCandidate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HrCandidateExists(id))
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

        // POST: api/HrCandidates
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<HrCandidate>> PostHrCandidate(HrCandidate hrCandidate)
        {
            _context.HrCandidate.Add(hrCandidate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHrCandidate", new { id = hrCandidate.Id }, hrCandidate);
        }

        // DELETE: api/HrCandidates/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HrCandidate>> DeleteHrCandidate(int id)
        {
            var hrCandidate = await _context.HrCandidate.FindAsync(id);
            if (hrCandidate == null)
            {
                return NotFound();
            }

            _context.HrCandidate.Remove(hrCandidate);
            await _context.SaveChangesAsync();

            return hrCandidate;
        }

        private bool HrCandidateExists(int id)
        {
            return _context.HrCandidate.Any(e => e.Id == id);
        }
    }
}
