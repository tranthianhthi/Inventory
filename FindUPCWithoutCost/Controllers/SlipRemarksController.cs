using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindUPCWithoutCost.Models;

namespace FindUPCWithoutCost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlipRemarksController : ControllerBase
    {
        private readonly InfologContext _context;

        public SlipRemarksController(InfologContext context)
        {
            _context = context;
        }

        // GET: api/SlipRemarks
        [HttpGet]
        public IEnumerable<SlipRemarks> GetSlipRemarkItems()
        {
            return _context.SlipRemarkItems;
        }

        // GET: api/SlipRemarks/OrderNo/S200200042
        [HttpGet("OrderNo/{order_no}")]
        public List<SlipRemarks> GetSlipRemarkItems(string order_no)
        {
            var q = from cust in _context.Model.FindEntityType(typeof(interface_acfc_shipment_confirm_download))
                    join dist in _context.so on interface_acfc_shipment_confirm_download.City equals so.id = interface_acfc_shipment_confirm_download.so_id
                    select new SlipRemarks() { Id = SortedDictionary.id, Remarks = interface_acfc_shipment_confirm_download.remarks, Order_No = so.order_no };

            var todoItems = q.ToList();

            if (todoItem == null)
            {
                //return NotFound();
            }

            return todoItem;
        }

        // GET: api/SlipRemarks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSlipRemarks([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slipRemarks = await _context.SlipRemarkItems.FindAsync(id);

            if (slipRemarks == null)
            {
                return NotFound();
            }

            return Ok(slipRemarks);
        }

        // PUT: api/SlipRemarks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlipRemarks([FromRoute] long id, [FromBody] SlipRemarks slipRemarks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != slipRemarks.Id)
            {
                return BadRequest();
            }

            _context.Entry(slipRemarks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlipRemarksExists(id))
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

        // POST: api/SlipRemarks
        [HttpPost]
        public async Task<IActionResult> PostSlipRemarks([FromBody] SlipRemarks slipRemarks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SlipRemarkItems.Add(slipRemarks);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSlipRemarks", new { id = slipRemarks.Id }, slipRemarks);
        }

        // DELETE: api/SlipRemarks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlipRemarks([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slipRemarks = await _context.SlipRemarkItems.FindAsync(id);
            if (slipRemarks == null)
            {
                return NotFound();
            }

            _context.SlipRemarkItems.Remove(slipRemarks);
            await _context.SaveChangesAsync();

            return Ok(slipRemarks);
        }

        private bool SlipRemarksExists(long id)
        {
            return _context.SlipRemarkItems.Any(e => e.Id == id);
        }
    }
}