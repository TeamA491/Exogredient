using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

// TODO UPDATE JWT TOKEN IN CONTROLLER IN COOKIES

namespace TeamA.Exogredient.Managers
{
    /// <summary>
    /// Calls services as well as enforces business rules
    /// </summary>
    public class TicketManager
    {
        private readonly TicketService ticketService;
        private readonly AuthorizationManager authorizationManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ticketService">Copy of the ticket service</param>
        /// <param name="authorizationManager">Copy of the authorization manager</param>
        public TicketManager(TicketService ticketService, AuthorizationManager authorizationManager)
        {
            this.ticketService = ticketService;
            this.authorizationManager = authorizationManager;
        }

        /// <summary>
        /// Gets tickets based on a filter or all tickets if none is provided
        /// </summary>
        /// <param name="jwtToken">Authorization token</param>
        /// <param name="filterParams">Optional filter parameters</param>
        /// <returns>Result that holds a list of TicketRecords</returns>
        public async Task<Result<List<TicketRecord>>> GetTicketsAsync(string jwtToken, Dictionary<string, string> filterParams)
        {
            List<TicketRecord> tickets;
            Result<List<TicketRecord>> result;

            try
            {
                AuthorizeUser(Constants.Operations.GetTickets, ref jwtToken);

                // If we don't have any filters, then just return all the tickets
                if (filterParams.Count == 0)
                {
                    tickets = await ticketService.GetAllTicketsAsync();
                }
                else
                {
                    tickets = await ticketService.GetTicketsByFilterAsync(filterParams);
                }

                // Save the result
                result = new Result<List<TicketRecord>>(Constants.TicketManagerSuccessFetchTickets)
                {
                    Data = tickets
                };
            }
            catch
            {
                // Save error
                result = new Result<List<TicketRecord>>(Constants.TicketManagerFailedFetchingTickets)
                {
                    ExceptionOccurred = true
                };
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
        public async Task<Result<bool>> UpdateTicketCategoryAsync(long ticketID, Constants.TicketCategories newCategory, string jwtToken)
        {
            Result<bool> result;
            try
            {
                AuthorizeUser(Constants.Operations.UpdateTicket, ref jwtToken);

                await ticketService.UpdateTicketCategoryAsync(ticketID, newCategory);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateCategory);
            }
            catch
            {
                // Save error
                result = new Result<bool>(Constants.TicketManagerFailedFetchingTickets)
                {
                    ExceptionOccurred = true
                };
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
        public async Task<Result<bool>> UpdateTicketStatusAsync(long ticketID, Constants.TicketStatuses newStatus, string jwtToken)
        {
            Result<bool> result;
            try
            {
                AuthorizeUser(Constants.Operations.UpdateTicket, ref jwtToken);

                bool success = await ticketService.UpdateTicketStatusAsync(ticketID, newStatus);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateStatus);
            }
            catch
            {
                // Save error
                result = new Result<bool>(Constants.TicketManagerFailedUpdateStatus)
                {
                    ExceptionOccurred = true
                };
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
        public async Task<Result<bool>> UpdateTicketReadStatusAsync(long ticketID, bool newReadStatus, string jwtToken)
        {
            Result<bool> result;
            try
            {
                AuthorizeUser(Constants.Operations.UpdateTicket, ref jwtToken);

                await ticketService.UpdateTicketReadStatusAsync(ticketID, newReadStatus);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateReadStatus);
            }
            catch
            {
                // Save error
                result = new Result<bool>(Constants.TicketManagerFailedUpdateReadStatus)
                {
                    ExceptionOccurred = true
                };
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
        public async Task<Result<bool>> UpdateTicketFlagColorAsync(long ticketID, Constants.TicketFlagColors newFlagColor, string jwtToken)
        {
            Result<bool> result;
            try
            {
                AuthorizeUser(Constants.Operations.UpdateTicket, ref jwtToken);
                
                await ticketService.UpdateTicketFlagColorAsync(ticketID, newFlagColor);
                result = new Result<bool>(Constants.TicketManagerSuccessUpdateFlagColor);
            }
            catch
            {
                // Save error
                result = new Result<bool>(Constants.TicketManagerFailedUpdateFlagColor)
                {
                    ExceptionOccurred = true
                };
            }

            return result;
        }

        /// <summary>
        /// Gets all the ticket categories
        /// </summary>
        /// <returns>List of ticket categories</returns>
        public Result<List<string>> GetTicketCategories()
        {
            Result<List<string>> result = new Result<List<string>>(Constants.TicketManagerSuccessGetCategories)
            {
                Data = Enum.GetNames(typeof(Constants.TicketCategories)).ToList()
            };
            return result;
        }

        /// <summary>
        /// Gets all the ticket flag colors
        /// </summary>
        /// <returns>List of ticket colors</returns>
        public Result<List<string>> GetTicketFlagColors()
        {
            Result<List<string>> result = new Result<List<string>>(Constants.TicketManagerSuccessGetFlagColors)
            {
                Data = Enum.GetNames(typeof(Constants.TicketFlagColors)).ToList()
            };
            return result;
        }

        /// <summary>
        /// Gets all ticket statuses
        /// </summary>
        /// <returns>List of ticket statuses</returns>
        public Result<List<string>> GetTicketStatuses()
        {
            Result<List<string>> result = new Result<List<string>>(Constants.TicketManagerSuccessGetStatuses)
            {
                Data = Enum.GetNames(typeof(Constants.TicketStatuses)).ToList()
            };
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
            Result<bool> result;

            try
            {
                await ticketService.SubmitTicketAsync(category, description);
                result = new Result<bool>(Constants.TicketManagerSuccessSubmitTicket);
            }
            catch
            {
                // Save error
                result = new Result<bool>(Constants.TicketManagerFailedSubmitTicket)
                {
                    ExceptionOccurred = true
                };
            }

            return result;
        }

        /// <summary>
        /// Authorizes a user based on their jwt token
        /// </summary>
        /// <param name="operation">The operation the user requested</param>
        /// <param name="jwtToken">The jwt token of the user</param>
        private void AuthorizeUser(Constants.Operations operation, ref string jwtToken)
        {
            bool isAuthorized = authorizationManager.AuthorizeUser(operation.ToString(), ref jwtToken);
            if (!isAuthorized)
                throw new ArgumentException(Constants.TicketManagerUnauthorizedUpdateTickets);
        }
    }
}
