using CottonOnAPI.Common;
using CottonOnAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CottonOnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferASNController : ControllerBase
    {
        readonly Configurations config = new Configurations();

        [HttpGet]
        public ActionResult<IEnumerable<Slip>> GetSlips()
        {
            DataTable tb = new DataTable();
            tb = config.ExecuteQueryData(config.RPConnection, config.SelectSlips, null);
            List<Slip> slips = new List<Slip>();
            foreach (DataRow row in tb.Rows)
            {
                slips.Add(new Slip(row));
            }

            return slips.ToList();
            //return await _context.TodoItems.ToListAsync();
        }

        [HttpGet("{id}")]
        [Route("private-scoped")]
        [Authorize("read:messages")]
        public ActionResult<Slip> GetSlip(int id)
        {
            try
            {
                DataTable tb = new DataTable();

                tb = config.ExecuteQueryData(config.RPConnection, config.SelectSlipBySlipNo + string.Format(" WHERE slip_no = {0}", id), null); // paras);

                Slip slip = tb.Rows.Count == 0 ? null : new Slip(tb.Rows[0]);

                tb = config.ExecuteQueryData(config.RPConnection, config.SelectSlipDetailBySlipID + string.Format(" WHERE slip_sid = {0}", slip.slip_sid), null);

                return slip;
            }
            catch(Exception ex)
            {
                return null;
            }

            
            //return await _context.TodoItems.ToListAsync();
        }

        [HttpGet]
        [Route("private")]
        [Authorize]
        public JsonResult Private()
        {
            return new JsonResult(new
            {
                Message = "Hello from a private endpoint! You need to be authenticated to see this."
            });
        }

        [HttpGet]
        [Route("private-scoped")]
        [Authorize("read:messages")]
        public JsonResult  Scoped()
        {
            return new JsonResult(new
            {
                Message = "Hello from a private endpoint! You need to be authenticated and have a scope of read:messages to see this."
            });
        }

    }
}