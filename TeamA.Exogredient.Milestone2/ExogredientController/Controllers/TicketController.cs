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
using Microsoft.Extensions.Primitives;

namespace ExogredientController.Controllers
{
    [Route("api/ticket")]
    [ApiController]
    public class TicketController : Controller
    {
        private static readonly TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
        private static readonly TicketService ticketService = new TicketService(ticketDAO);
        private static readonly AuthorizationService authorizationService = new AuthorizationService();
        private static readonly UserDAO userDAO = new UserDAO(Constants.SQLConnection);
        private static readonly SessionService sessionService = new SessionService(userDAO, authorizationService);
        private static readonly AuthorizationManager authorizationManager = new AuthorizationManager(authorizationService, sessionService);
        private static readonly TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

        [HttpGet("getTickets")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTicketsAsync([FromQuery]Dictionary<string, string> filterParams)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch
            {
                // 403 - Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            Result<List<TicketRecord>> tickets = await ticketManager.GetTicketsAsync(jwtToken, filterParams);
            return Ok(tickets);
        }

        [HttpPost("updateTicketCategory")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketCategoryAsync(long ticketID, string fieldUpdate)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch
            {
                // 403 - Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            // Make sure we sent a correct category
            Constants.TicketCategories category;
            try
            {
                category = (Constants.TicketCategories)Enum.Parse(typeof(Constants.TicketCategories), fieldUpdate);
            }
            catch
            {
                // 400 - Bad Request
                return BadRequest(Constants.TicketImproperCategory);
            }

            Result<bool> success = await ticketManager.UpdateTicketCategoryAsync(ticketID, category, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("updateTicketStatus")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketStatusAsync(long ticketID, string fieldUpdate)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch
            {
                // 403 - Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            // Make sure we sent a correct status
            Constants.TicketStatuses status;
            try
            {
                status = (Constants.TicketStatuses)Enum.Parse(typeof(Constants.TicketStatuses), fieldUpdate);
            }
            catch
            {
                // 400 - Bad Request
                return BadRequest(Constants.TicketImproperStatus);
            }

            Result<bool> success = await ticketManager.UpdateTicketStatusAsync(ticketID, status, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("updateTicketReadStatus")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketReadStatusAsync(long ticketID, string fieldUpdate)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch
            {
                // 403 - Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            // Make sure we sent a correct read status
            Constants.TicketReadStatuses readStatus;
            try
            {
                readStatus = (Constants.TicketReadStatuses)Enum.Parse(typeof(Constants.TicketReadStatuses), fieldUpdate);
            }
            catch
            {
                // 400 - Bad Request
                return BadRequest(Constants.TicketImproperReadStatus);
            }

            Result<bool> success = await ticketManager.UpdateTicketReadStatusAsync(ticketID, readStatus == Constants.TicketReadStatuses.Read, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("updateTicketFlagColor")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketFlagColorAsync(long ticketID, string fieldUpdate)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch
            {
                // 403 - Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            // Make sure we sent a correct flag color
            Constants.TicketFlagColors flagColor;
            try
            {
                flagColor = (Constants.TicketFlagColors)Enum.Parse(typeof(Constants.TicketFlagColors), fieldUpdate);
            }
            catch
            {
                // 400 - Bad Request
                return BadRequest(Constants.TicketImproperFlagColor);
            }

            Result<bool> success = await ticketManager.UpdateTicketFlagColorAsync(ticketID, flagColor, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("submitTicket")]
        [Produces("application/json")]
        public async Task<IActionResult> SubmitTicket(string category, string description)
        {
            // Make sure we sent a correct category
            Constants.TicketCategories ticketCategory;
            try
            {
                ticketCategory = (Constants.TicketCategories)Enum.Parse(typeof(Constants.TicketCategories), category);
            }
            catch
            {
                // 400 - Bad Request
                return BadRequest(Constants.TicketImproperCategory);
            }

            Result<bool> success = await ticketManager.SubmitTicket(ticketCategory, description);
            return Ok(success);
        }
    }
}
