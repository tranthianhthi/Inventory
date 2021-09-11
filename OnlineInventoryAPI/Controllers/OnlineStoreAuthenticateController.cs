using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineInventoryAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineInventoryAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiExplorerSettings(GroupName = "CandidateAPI")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class OnlineStoreAuthenticateController : ControllerBase
    {
        public IActionResult GetAuthenticationInfo(string storeCode, string platform)
        {
            OnlineStoreAuthenticate onlineStoreAuthenticate = null;

            if (string.Compare(platform.ToLower(), "lazada") != 0 || string.Compare(platform.ToLower(), "shopee") != 0)
                return BadRequest();

            return Ok();
        }
    }
}
