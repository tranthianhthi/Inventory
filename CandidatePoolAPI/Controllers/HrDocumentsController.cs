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
    public class HrDocumentsController : ControllerBase
    {
        private readonly ACFCInventoryContext _context;

        public HrDocumentsController(ACFCInventoryContext context)
        {
            _context = context;
        }

        // GET: api/HrDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HrDocument>>> GetHrDocument()
        {
            return await _context.HrDocument.ToListAsync();
        }

        // GET: api/HrDocuments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HrDocument>> GetHrDocument(int id)
        {
            var hrDocument = await _context.HrDocument.FindAsync(id);

            if (hrDocument == null)
            {
                return NotFound();
            }

            return hrDocument;
        }

        // PUT: api/HrDocuments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHrDocument(int id, HrDocument hrDocument)
        {
            if (id != hrDocument.Id)
            {
                return BadRequest();
            }

            _context.Entry(hrDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HrDocumentExists(id))
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

        // POST: api/HrDocuments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<HrDocument>> PostHrDocument(HrDocument hrDocument)
        {
            _context.HrDocument.Add(hrDocument);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHrDocument", new { id = hrDocument.Id }, hrDocument);
        }

        // DELETE: api/HrDocuments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HrDocument>> DeleteHrDocument(int id)
        {
            var hrDocument = await _context.HrDocument.FindAsync(id);
            if (hrDocument == null)
            {
                return NotFound();
            }

            _context.HrDocument.Remove(hrDocument);
            await _context.SaveChangesAsync();

            return hrDocument;
        }

        private bool HrDocumentExists(int id)
        {
            return _context.HrDocument.Any(e => e.Id == id);
        }
    }
}
