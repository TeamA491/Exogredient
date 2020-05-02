using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicketController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketSystemController : ControllerBase
    {


        [HttpGet("GetTicket")]
        [Produces("Aplication/json")]
        public IActionResult doesnmatter()
        {


            
            return Ok("inside ticket");
        }



    }
}