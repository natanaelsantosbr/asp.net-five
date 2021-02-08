using Microsoft.AspNetCore.Mvc;
using Natanael.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Controllers.V1
{
    [ApiKeyAuth]
    public class SecretController : Controller
    {
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("I have no secrets");
        }


    }
}
