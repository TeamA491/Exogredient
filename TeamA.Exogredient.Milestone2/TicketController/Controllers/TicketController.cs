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

namespace TicketController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        [HttpPost("getTickets")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTicketsAsync(TicketSearchRequest req)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch(Exception e)
            {
                // Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationService authorizationService = new AuthorizationService();
            UserDAO userDAO = new UserDAO(Constants.SQLConnection);
            SessionService sessionService = new SessionService(userDAO, authorizationService);
            AuthorizationManager authorizationManager = new AuthorizationManager(authorizationService, sessionService);
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

            Result<List<TicketRecord>> tickets = await ticketManager.GetTicketsAsync(jwtToken, req.filterParams);

            return Ok(tickets);
        }

        [HttpPost("updateTicketCategory")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketCategoryAsync(TicketUpdateRequest req)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationService authorizationService = new AuthorizationService();
            UserDAO userDAO = new UserDAO(Constants.SQLConnection);
            SessionService sessionService = new SessionService(userDAO, authorizationService);
            AuthorizationManager authorizationManager = new AuthorizationManager(authorizationService, sessionService);
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

            // Make sure we sent a correct category
            Constants.TicketCategories category;
            try
            {
                category = (Constants.TicketCategories)Enum.Parse(typeof(Constants.TicketCategories), req.fieldUpdate);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.TicketImproperCategory);
            }

            Result<bool> success = await ticketManager.UpdateTicketCategoryAsync(req.ticketID, category, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("updateTicketStatus")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketStatusAsync(TicketUpdateRequest req)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationService authorizationService = new AuthorizationService();
            UserDAO userDAO = new UserDAO(Constants.SQLConnection);
            SessionService sessionService = new SessionService(userDAO, authorizationService);
            AuthorizationManager authorizationManager = new AuthorizationManager(authorizationService, sessionService);
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

            // Make sure we sent a correct status
            Constants.TicketStatuses status;
            try
            {
                status = (Constants.TicketStatuses)Enum.Parse(typeof(Constants.TicketStatuses), req.fieldUpdate);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.TicketImproperStatus);
            }

            Result<bool> success = await ticketManager.UpdateTicketStatusAsync(req.ticketID, status, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("updateTicketReadStatus")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketReadStatusAsync(TicketUpdateRequest req)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationService authorizationService = new AuthorizationService();
            UserDAO userDAO = new UserDAO(Constants.SQLConnection);
            SessionService sessionService = new SessionService(userDAO, authorizationService);
            AuthorizationManager authorizationManager = new AuthorizationManager(authorizationService, sessionService);
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

            // Make sure we sent a correct read status
            Constants.TicketReadStatuses readStatus;
            try
            {
                readStatus = (Constants.TicketReadStatuses)Enum.Parse(typeof(Constants.TicketReadStatuses), req.fieldUpdate);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.TicketImproperReadStatus);
            }

            Result<bool> success = await ticketManager.UpdateTicketReadStatusAsync(req.ticketID, readStatus == Constants.TicketReadStatuses.Read, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("updateTicketFlagColor")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTicketFlagColorAsync(TicketUpdateRequest req)
        {
            StringValues jwtToken;
            try
            {
                Request.Headers.TryGetValue(Constants.JWTTokenHeader, out jwtToken);
            }
            catch (Exception e)
            {
                // Forbidden
                return Forbid(Constants.UnauthorizedRequest);
            }

            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationService authorizationService = new AuthorizationService();
            UserDAO userDAO = new UserDAO(Constants.SQLConnection);
            SessionService sessionService = new SessionService(userDAO, authorizationService);
            AuthorizationManager authorizationManager = new AuthorizationManager(authorizationService, sessionService);
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

            // Make sure we sent a correct flag color
            Constants.TicketFlagColors flagColor;
            try
            {
                flagColor = (Constants.TicketFlagColors)Enum.Parse(typeof(Constants.TicketFlagColors), req.fieldUpdate);
            }
            catch (Exception e)
            {
                return BadRequest(Constants.TicketImproperFlagColor);
            }

            Result<bool> success = await ticketManager.UpdateTicketFlagColorAsync(req.ticketID, flagColor, jwtToken.ToString());
            return Ok(success);
        }

        [HttpPost("submitTicket")]
        [Produces("application/json")]
        public async Task<IActionResult> SubmitTicket(NewTicketRequest req)
        {
            TicketDAO ticketDAO = new TicketDAO(Constants.SQLConnection);
            TicketService ticketService = new TicketService(ticketDAO);
            AuthorizationService authorizationService = new AuthorizationService();
            UserDAO userDAO = new UserDAO(Constants.SQLConnection);
            SessionService sessionService = new SessionService(userDAO, authorizationService);
            AuthorizationManager authorizationManager = new AuthorizationManager(authorizationService, sessionService);
            TicketManager ticketManager = new TicketManager(ticketService, authorizationManager);

            // Make sure we sent a correct category
            Constants.TicketCategories category;
            try
            {
                category = (Constants.TicketCategories)Enum.Parse(typeof(Constants.TicketCategories), req.category);
            }
            catch(Exception e)
            {
                return BadRequest(Constants.TicketImproperCategory);
            }

            Result<bool> success = await ticketManager.SubmitTicket(category, req.description);
            return Ok(success);
        }
    }
}
