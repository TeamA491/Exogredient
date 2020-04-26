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

        public TicketManager(TicketService ticketService, LoggingManager loggingManager, SessionService sessionService, AuthorizationService authorizationService)
        {
            this.ticketService = ticketService;
            this.loggingManager = loggingManager;
            this.sessionService = sessionService;
            this.authorizationService = authorizationService;
        }

        public async Task<List<TicketRecord>> GetTicketsAsync(string jwtToken, Dictionary<Constants.TicketSearchFilter, object> filterParams)
        {
            // TODO
            // AUTHZ
            // LOGGING
            // EXCEPTION MANAGEMENT
            List<TicketRecord> tickets = new List<TicketRecord>();

            try
            {
                // If we don't have any filters, then just return all the tickets
                if (filterParams.Count == 0)
                {
                    // TODO
                    //authorizationService.UserHasPermissionForOperation();
                    tickets = await ticketService.GetAllTickets();
                }
                else
                {
                    tickets = await ticketService.GetTicketsByFilterAsync(filterParams).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {

            }

            return tickets;
        }

        public async Task<bool> UpdateTicketCategoryAsync(uint ticketID, Constants.TicketCategories newCategory)
        {
            // TODO
            // AUTHZ
            // LOGGING
            // EXCEPTION MANAGEMENT
            // INPUT VALIDATION

            try
            {
                await ticketService.UpdateTicketCategoryAsync(ticketID, newCategory).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }

            return true;
        }

        public async Task<bool> UpdateTicketStatusAsync(uint ticketID, Constants.TicketStatuses newStatus)
        {
            // TODO
            // AUTHZ
            // LOGGING
            // EXCEPTION MANAGEMENT

            try
            {
                await ticketService.UpdateTicketStatusAsync(ticketID, newStatus).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }

            return true;
        }

        public async Task<bool> UpdateTicketReadStatusAsync(uint ticketID, bool newReadStatus)
        {
            // TODO
            // AUTHZ
            // LOGGING
            // EXCEPTION MANAGEMENT

            try
            {
                await ticketService.UpdateTicketReadStatusAsync(ticketID, newReadStatus).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }

            return true;
        }

        public async Task<bool> UpdateTicketFlagColorAsync(uint ticketID, Constants.TicketFlagColors newFlagColor)
        {
            // TODO
            // AUTHZ
            // LOGGING
            // EXCEPTION MANAGEMENT
            // INPUT VALIDATION

            try
            {
                await ticketService.UpdateTicketFlagColorAsync(ticketID, newFlagColor).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }

            return true;
        }

        public async Task<bool> SubmitTicket(Constants.TicketCategories category, string description)
        {
            // TODO
            // AUTHZ
            // LOGGING
            // EXCEPTION MANAGEMENT

            try
            {
                await ticketService.SubmitTicketAsync(category, description).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }

            return true;
        }
    }
}
