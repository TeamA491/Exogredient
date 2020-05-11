using System;
using System.Data;
using TeamA.Exogredient.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeamA.Exogredient.DataHelpers;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    /// <summary>
    /// Contains functions to access data from DAL
    /// </summary>
    public class TicketService
    {
        private readonly TicketDAO ticketDAO;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ticketDAO">Copy of ticket data access object</param>
        public TicketService(TicketDAO ticketDAO)
        {
            this.ticketDAO = ticketDAO;
        }

        /// <summary>
        /// Will return all tickets that meet the search criteria
        /// </summary>
        /// <param name="filterParams">A dictionary containing search criteria</param>
        /// <returns>Array of TicketRecords that meet the search criteria</returns>
        public async Task<List<TicketRecord>> GetTicketsByFilterAsync(Dictionary<string, string> filterParams)
        {
            Dictionary<Constants.TicketSearchFilter, object> validatedParams = new Dictionary<Constants.TicketSearchFilter, object>();

            // Go through each filter and make sure we provided existing ones
            foreach (KeyValuePair<string, string> pair in filterParams)
            {
                // Make sure we supplied a TicketSearchFilter enum for the key
                bool success = Enum.TryParse(pair.Key, out Constants.TicketSearchFilter searchFilter);
                if (!success)
                    throw new ArgumentException(Constants.TicketSearchFilterDNE);

                validatedParams.Add(searchFilter, pair.Value);
            }

            List<DataRow> ticketsRaw = await ticketDAO.FilterTicketsAsync(validatedParams);
            return FormatDataRow(ref ticketsRaw);
        }

        /// <summary>
        /// Returns all tickets in the database
        /// </summary>
        /// <returns>A list of TicketRecord objects</returns>
        public async Task<List<TicketRecord>> GetAllTicketsAsync()
        {
            List<DataRow> ticketsRaw = await ticketDAO.GetAllTicketsAsync();
            return FormatDataRow(ref ticketsRaw);
        }

        /// <summary>
        /// Returns all the categories in the database
        /// </summary>
        /// <returns>A list of TicketRecord objects</returns>
        public async Task<List<TicketRecord>> GetAllTicketCategoriesAsync()
        {
            List<DataRow> categoriesRaw = await ticketDAO.GetAllCategoriesAsync();
            return FormatDataRow(ref categoriesRaw);
        }

        /// <summary>
        /// Returns all the flag colors in the database
        /// </summary>
        /// <returns>A list of TicketRecord objects</returns>
        public async Task<List<TicketRecord>> GetAllFlagColorsAsync()
        {
            List<DataRow> flagColorsRaw = await ticketDAO.GetAllFlagColorsAsync();
            return FormatDataRow(ref flagColorsRaw);
        }

        /// <summary>
        /// Returns all ticket statuses in the database
        /// </summary>
        /// <returns>A list of TicketRecord objects</returns>
        public async Task<List<TicketRecord>> GetAllTicketStatuses()
        {
            List<DataRow> ticketStatusesRaw = await ticketDAO.GetAllTicketStatusesAsync();
            return FormatDataRow(ref ticketStatusesRaw);
        }

        /// <summary>
        /// Updates a ticket's status
        /// </summary>
        /// <param name="ticketID">The id of the ticket</param>
        /// <param name="newStatus">The new status that we want it to have</param>
        /// <returns>Whether we successfully changed the status or not</returns>
        public async Task<bool> UpdateTicketStatusAsync(long ticketID, Constants.TicketStatuses newStatus)
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
        public async Task<bool> UpdateTicketCategoryAsync(long ticketID, Constants.TicketCategories newCategory)
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
        public async Task<bool> UpdateTicketReadStatusAsync(long ticketID, bool newReadStatus)
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
        public async Task<bool> UpdateTicketFlagColorAsync(long ticketID, Constants.TicketFlagColors newFlagColor)
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
            TicketRecord ticketRecord = new TicketRecord(TimeUtilityService.CurrentUnixTime(),
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
        private List<TicketRecord> FormatDataRow(ref List<DataRow> data)
        {
            List<TicketRecord> tickets = new List<TicketRecord>();

            // Construct the data
            foreach (DataRow row in data)
            {
                tickets.Add(
                    new TicketRecord((long)row[Constants.TicketDAOSubmitTimestampColumn],
                    (string)row[Constants.TicketDAOCategoryColumn],
                    (string)row[Constants.TicketDAOStatusColumn],
                    (string)row[Constants.TicketDAOFlagColorColumn],
                    (string)row[Constants.TicketDAODescriptionColumn],
                    (bool)row[Constants.TicketDAOIsReadColumn])
                );
            }

            return tickets;
        }
    }
}
