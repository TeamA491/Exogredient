using System;
using System.Data;
using TeamA.Exogredient.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public class TicketService
    {
        private readonly TicketDAO ticketDAO;

        public TicketService(TicketDAO ticketDAO)
        {
            this.ticketDAO = ticketDAO;
        }

        /// <summary>
        /// Will return all tickets that meet the search criteria
        /// </summary>
        /// <param name="filterParams">A dictionary containing search criteria</param>
        /// <returns>Array of TicketRecords that meet the search criteria</returns>
        public async Task<List<TicketRecord>> GetTicketsByFilterAsync(Dictionary<Constants.TicketSearchFilter, object> filterParams)
        {
            List<DataRow> ticketsRaw = await ticketDAO.FilterTicketsAsync(filterParams);
            FormatDataRow(ref ticketsRaw, out List<TicketRecord> tickets);
            return tickets;
        }

        /// <summary>
        /// Returns all tickets in the database
        /// </summary>
        /// <returns>A list of TicketRecord objects</returns>
        public async Task<List<TicketRecord>> GetAllTickets()
        {
            List<DataRow> ticketsRaw = await ticketDAO.GetAllTickets();
            FormatDataRow(ref ticketsRaw, out List<TicketRecord> tickets);
            return tickets;
        }

        /// <summary>
        /// Updates a ticket's status
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newStatus">The new status that we want it to have</param>
        /// <returns>Whether we successfully changed the status or not</returns>
        public async Task<bool> UpdateTicketStatusAsync(uint ticketID, Constants.TicketStatuses newStatus)
        {
            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException(Constants.TicketUpdateNotExists);

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, null, newStatus.ToString(), null, null);
            await ticketDAO.UpdateAsync(ticketRecord);

            return true;
        }

        /// <summary>
        /// Updates a ticket's category
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newCategory">The new ticket category to use</param>
        /// <returns>Whether the ticket category succesfully changed or not</returns>
        public async Task<bool> UpdateTicketCategoryAsync(uint ticketID, Constants.TicketCategories newCategory)
        {
            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException(Constants.TicketUpdateNotExists);

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, newCategory.ToString(), null, null, null);
            await ticketDAO.UpdateAsync(ticketRecord);

            return true;
        }

        /// <summary>
        /// Updates a ticket's read status
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newReadStatus">The new read status to use</param>
        /// <returns>Whether the read status succesfully changed or not</returns>
        public async Task<bool> UpdateTicketReadStatusAsync(uint ticketID, bool newReadStatus)
        {
            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException(Constants.TicketUpdateNotExists);

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, null, null, null, null, newReadStatus);
            await ticketDAO.UpdateAsync(ticketRecord);

            return true;
        }

        /// <summary>
        /// Updates a ticket's flag color
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newFlagColor">The new flag color to use</param>
        /// <returns>Whether the flag color succesfully changed or not</returns>
        public async Task<bool> UpdateTicketFlagColorAsync(uint ticketID, Constants.TicketFlagColors newFlagColor)
        {
            // Make sure the ticket exists first
            bool ticketExists = await ticketDAO.CheckTicketExistenceAsync(ticketID);
            if (!ticketExists)
                throw new ArgumentException(Constants.TicketUpdateNotExists);

            // Update the ticket
            TicketRecord ticketRecord = new TicketRecord(ticketID, null, null, newFlagColor.ToString(), null);
            await ticketDAO.UpdateAsync(ticketRecord);

            return true;
        }

        /// <summary>
        /// Submits a ticket to the system
        /// </summary>
        /// <param name="category">The category of the ticket</param>
        /// <param name="description">The description inside the ticket</param>
        /// <returns>Whether the ticket was saved or not</returns>
        public async Task<bool> SubmitTicketAsync(Constants.TicketCategories category, string description)
        {
            TicketRecord ticketRecord = new TicketRecord((uint)TimeUtilityService.CurrentUnixTime(),
                                                        category.ToString(),
                                                        Constants.TicketStatuses.Unresolved.ToString(),
                                                        Constants.TicketFlagColors.None.ToString(),
                                                        description);
            await ticketDAO.CreateAsync(ticketRecord);

            return true;
        }

        /// <summary>
        /// Used to convert DataRow to TicketRecord
        /// </summary>
        /// <param name="data">The raw data to be formatted</param>
        /// <param name="tickets">Where to save the formatted data</param>
        private void FormatDataRow(ref List<DataRow> data, out List<TicketRecord> tickets)
        {
            tickets = new List<TicketRecord>();

            // Construct the data
            foreach (DataRow row in data)
            {
                tickets.Add(
                    new TicketRecord((uint)row[Constants.TicketDAOSubmitTimestampColumn],
                    (string)row[Constants.TicketDAOCategoryColumn],
                    (string)row[Constants.TicketDAOStatusColumn],
                    (string)row[Constants.TicketDAOFlagColorColumn],
                    (string)row[Constants.TicketDAODescriptionColumn],
                    (bool)row[Constants.TicketDAOIsReadColumn])
                );
            }
        }
    }
}
