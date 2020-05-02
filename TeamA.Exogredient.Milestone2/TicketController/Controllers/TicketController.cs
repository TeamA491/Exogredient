using System;
using System.Linq;
using TeamA.Exogredient.DAL;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TicketController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        [HttpPost("getTickets")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTicketsAsync(TicketSearchRequest req)
        {
            string jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch(Exception e)
            {
                // TODO RETURN 403
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationManager authorizationManager = new AuthorizationManager();
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

            List<TicketRecord> tickets = await ticketManager.GetTicketsAsync(jwtToken, req.filterParams);

            return Ok("inside ticket");
        }

        [HttpPost("")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketCategoryAsync(TicketUpdateRequest req)
        {
            string jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // TODO RETURN 403
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationManager authorizationManager = new AuthorizationManager();
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);
        }

        [HttpPost("")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketStatusAsync(TicketUpdateRequest req)
        {
            string jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // TODO RETURN 403
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationManager authorizationManager = new AuthorizationManager();
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);
        }

        [HttpPost("")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketReadStatusAsync(TicketUpdateRequest req)
        {
            string jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // TODO RETURN 403
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationManager authorizationManager = new AuthorizationManager();
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);
        }

        [HttpPost("")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketFlagColorAsync(TicketUpdateRequest req)
        {
            string jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // TODO RETURN 403
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationManager authorizationManager = new AuthorizationManager();
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);
        }

        [HttpPost("")]
        [Produces("application/json")]
        public async Task<IActionResult> SubmitTicket(NewTicketRequest req)
        {
            string jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // TODO RETURN 403
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationManager authorizationManager = new AuthorizationManager();
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);
        }
    }
}
