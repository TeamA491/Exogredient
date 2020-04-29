using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Managers
{
    public class TicketManager
    {
        private readonly TicketService ticketService;
        private readonly LoggingManager loggingManager;
        private readonly SessionService sessionService;
        private readonly AuthorizationService authorizationService;
        private readonly AuthorizationManager authorizationManager;

        public TicketManager(TicketService ticketService, LoggingManager loggingManager, SessionService sessionService, AuthorizationService authorizationService, AuthorizationManager authorizationManager)
        {
            this.ticketService = ticketService;
            this.loggingManager = loggingManager;
            this.sessionService = sessionService;
            this.authorizationService = authorizationService;
            this.authorizationManager = authorizationManager;
        }

        /// <summary>
        /// Gets tickets based on a filter or all tickets if none is provided
        /// </summary>
        /// <param name="jwtToken">Authorization token</param>
        /// <param name="filterParams">Optional filter parameters</param>
        /// <returns>Result that holds a list of TicketRecords</returns>
        public async Task<Result<List<TicketRecord>>> GetTicketsAsync(string jwtToken, Dictionary<Constants.TicketSearchFilter, object> filterParams)
        {
            Result<List<TicketRecord>> result;

            // Authorize user
            bool isAuthorized = authorizationManager.AuthorizeUser(Constants.Operations.GetTickets.ToString(), jwtToken);
            if (!isAuthorized)
            {
                result = new Result<List<TicketRecord>>(Constants.TicketManagerUnauthorizedGetTickets);
                return result;
            }

            // TODO
            // LOGGING
            List<TicketRecord> tickets = new List<TicketRecord>();

            try
            {
                // If we don't have any filters, then just return all the tickets
                if (filterParams.Count == 0)
                {
                    tickets = await ticketService.GetAllTickets();
                }
                else
                {
                    tickets = await ticketService.GetTicketsByFilterAsync(filterParams);
                }

                // Save the result
                result = new Result<List<TicketRecord>>(Constants.TicketManagerSuccessFetchTickets);
                result.Data = tickets;
            }
            catch (Exception e)
            {
                result = new Result<List<TicketRecord>>(Constants.TicketManagerFailedFetchingTickets);
                result.ExceptionOccurred = true;
            }

            return result;
        }

        /// <summary>
        /// Updates a ticket's category
        /// </summary>
        /// <param name="ticketID">The id of the ticket to update</param>
        /// <param name="newCategory">The new category to replace</param>
        /// <param name="jwtToken">Authorization token</param>
        /// <returns>Success status</returns>
        public async Task<Result<bool>> UpdateTicketCategoryAsync(uint ticketID, Constants.TicketCategories newCategory, string jwtToken)
        {
            Result<bool> result;

            // Authorize user
            bool isAuthorized = authorizationManager.AuthorizeUser(Constants.Operations.UpdateTicket.ToString(), jwtToken);
            if (!isAuthorized)
            {
                result = new Result<bool>(Constants.TicketManagerUnauthorizedUpdateTickets);
                return result;
            }
            // TODO
            // LOGGING

            try
            {
                await ticketService.UpdateTicketCategoryAsync(ticketID, newCategory);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateCategory);
            }
            catch (Exception e)
            {
                result = new Result<bool>(Constants.TicketManagerFailedFetchingTickets);
                result.ExceptionOccurred = true;
            }

            return result;
        }

        /// <summary>
        /// Updates a ticket's status
        /// </summary>
        /// <param name="ticketID">The id of the ticket to update</param>
        /// <param name="newStatus">The new status to replace</param>
        /// <param name="jwtToken">Authorization token</param>
        /// <returns>Success status</returns>
        public async Task<Result<bool>> UpdateTicketStatusAsync(uint ticketID, Constants.TicketStatuses newStatus, string jwtToken)
        {
            Result<bool> result;

            // Authorize user
            bool isAuthorized = authorizationManager.AuthorizeUser(Constants.Operations.UpdateTicket.ToString(), jwtToken);
            if (!isAuthorized)
            {
                result = new Result<bool>(Constants.TicketManagerUnauthorizedUpdateTickets);
                return result;
            }

            // TODO
            // LOGGING

            try
            {
                bool success = await ticketService.UpdateTicketStatusAsync(ticketID, newStatus);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateStatus);

            }
            catch (Exception e)
            {
                result = new Result<bool>(Constants.TicketManagerFailedUpdateStatus);
                result.ExceptionOccurred = true;
            }

            return result;
        }

        /// <summary>
        /// Updates a ticket's read status
        /// </summary>
        /// <param name="ticketID">The id of the ticket to update</param>
        /// <param name="newReadStatus">The new read status to replace</param>
        /// <param name="jwtToken">Authorization token</param>
        /// <returns>Success status</returns>
        public async Task<Result<bool>> UpdateTicketReadStatusAsync(uint ticketID, bool newReadStatus, string jwtToken)
        {
            Result<bool> result;

            // Authorize user
            bool isAuthorized = authorizationManager.AuthorizeUser(Constants.Operations.UpdateTicket.ToString(), jwtToken);
            if (!isAuthorized)
            {
                result = new Result<bool>(Constants.TicketManagerUnauthorizedUpdateTickets);
                return result;
            }

            // TODO
            // LOGGING

            try
            {
                await ticketService.UpdateTicketReadStatusAsync(ticketID, newReadStatus);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateReadStatus);
            }
            catch (Exception e)
            {
                result = new Result<bool>(Constants.TicketManagerFailedUpdateReadStatus);
                result.ExceptionOccurred = true;
            }

            return result;
        }

        /// <summary>
        /// Updates a ticket's flag color
        /// </summary>
        /// <param name="ticketID">The id of the ticket to update</param>
        /// <param name="newFlagColor">The new flag color to replace</param>
        /// <param name="jwtToken">Authorization token</param>
        /// <returns>Success status</returns>
        public async Task<Result<bool>> UpdateTicketFlagColorAsync(uint ticketID, Constants.TicketFlagColors newFlagColor, string jwtToken)
        {
            Result<bool> result;

            // Authorize user
            bool isAuthorized = authorizationManager.AuthorizeUser(Constants.Operations.UpdateTicket.ToString(), jwtToken);
            if (!isAuthorized)
            {
                result = new Result<bool>(Constants.TicketManagerUnauthorizedUpdateTickets);
                return result;
            }

            // TODO
            // LOGGING

            try
            {
                await ticketService.UpdateTicketFlagColorAsync(ticketID, newFlagColor);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateFlagColor);
            }
            catch (Exception e)
            {
                result = new Result<bool>(Constants.TicketManagerFailedUpdateFlagColor);
                result.ExceptionOccurred = true;
            }

            return result;
        }

        /// <summary>
        /// Submits a ticket
        /// </summary>
        /// <param name="category">The category of the ticket</param>
        /// <param name="description">The description contained in the ticket</param>
        /// <returns>Success status</returns>
        public async Task<Result<bool>> SubmitTicket(Constants.TicketCategories category, string description)
        {
            // TODO
            // LOGGING
            Result<bool> result;

            try
            {
                await ticketService.SubmitTicketAsync(category, description);
                result = new Result<bool>(Constants.TicketManagerSuccessSubmitTicket);
            }
            catch (Exception e)
            {
                result = new Result<bool>(Constants.TicketManagerFailedSubmitTicket);
                result.ExceptionOccurred = true;
            }

            return result;
        }
    }
}
